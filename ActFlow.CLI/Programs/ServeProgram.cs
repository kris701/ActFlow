using ActFlow.Archiver;
using ActFlow.CLI.Helpers;
using ActFlow.CLI.Models;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Web;
using ToolsSharp;

namespace ActFlow.CLI.Programs
{
	public static class ServeProgram
	{
		public static async Task Run(ServeOptions opts)
		{
			ConsoleHelpers.WriteLineColor("Checking paths...", ConsoleColor.Blue);
			var configFile = File.ReadAllText(opts.ConfigPath);

			ConsoleHelpers.WriteLineColor("Loading plugins...", ConsoleColor.Blue);
			PluginLoader.LoadPlugins();

			ConsoleHelpers.WriteLineColor("Parsing Config...", ConsoleColor.Blue);
			var workers = JsonSerializer.Deserialize<List<IWorker>>(configFile, Constants.SerializerOpts);
			if (workers == null)
				throw new Exception("Config is malformed!");

			ConsoleHelpers.WriteLineColor("Initializing engines...", ConsoleColor.Blue);
			IActFlowEngine engine = new ActFlowEngine(workers)
			{
				ActivityLimiter = opts.Limiter,
				PersistentDirectory = opts.PersistentDirectory,
				TemporaryDirectory = opts.RunnerDirectory,
				CompletedDirectory = opts.CompletedDirectory
			};
			await engine.Initialize();
			IWorkflowArchive archive = new WorkflowArchive()
			{
				CompletedDirectory = opts.CompletedDirectory
			};
			await archive.Initialize();

			HttpListener listener = new HttpListener();
			listener.Prefixes.Add(opts.Host);
			listener.Start();
			ConsoleHelpers.WriteLineColor($"Now listening on '{opts.Host}'", ConsoleColor.Green);
			while (true)
			{
				HttpListenerResponse? resp = null;
				try
				{
					HttpListenerContext ctx = await listener.GetContextAsync();
					HttpListenerRequest req = ctx.Request;
					resp = ctx.Response;

					if (req.Url != null)
					{
						ConsoleHelpers.WriteLineColor($"\tRequest on '{req.Url.AbsolutePath}'", ConsoleColor.DarkGray);

						if ((req.HttpMethod == "POST") && (req.Url.AbsolutePath == "/run"))
							await HandleRunRequest(engine, req, resp);
						else if ((req.HttpMethod == "POST") && (req.Url.AbsolutePath == "/queue"))
							await HandleQueueRequest(engine, req, resp);
						else if ((req.HttpMethod == "GET") && (req.Url.AbsolutePath == "/results"))
							await HandleResultsRequest(engine, archive, req, resp);
						else if ((req.HttpMethod == "PATCH") && (req.Url.AbsolutePath == "/input"))
							await HandleHumanInputRequest(engine, req, resp);
					}

					resp.Close();
				}
				catch (Exception ex)
				{
					ConsoleHelpers.WriteLineColor("Unknown error: " + ex.Message, ConsoleColor.Red);
					if (resp != null)
					{
						await WriteResponse(resp, ex.Message);
						resp.Close();
					}
				}
			}
		}

		private static async Task HandleRunRequest(IActFlowEngine engine, HttpListenerRequest req, HttpListenerResponse resp)
		{
			var body = GetRequestPostData(req);
			if (body == null)
				return;
			var workflow = JsonSerializer.Deserialize<Workflow>(body, Constants.SerializerOpts);
			if (workflow == null)
				return;

			var result = await engine.ExecuteAsync(workflow);
			await WriteResponse(resp, result);
		}

		private static async Task HandleQueueRequest(IActFlowEngine engine, HttpListenerRequest req, HttpListenerResponse resp)
		{
			var body = GetRequestPostData(req);
			if (body == null)
				return;
			var workflow = JsonSerializer.Deserialize<Workflow>(body, Constants.SerializerOpts);
			if (workflow == null)
				return;

			var result = engine.Execute(workflow);
			await WriteResponse(resp, result);
		}

		private static async Task HandleResultsRequest(IActFlowEngine engine, IWorkflowArchive archive, HttpListenerRequest req, HttpListenerResponse resp)
		{
			var parsedString = HttpUtility.HtmlDecode(req.Url!.Query);
			var idStr = HttpUtility.ParseQueryString(parsedString)["id"];
			if (idStr == null)
			{
				var items = new List<dynamic>();
				items.AddRange(engine.ActiveWorkflows.Select(x => new { ID = x.ID, Status = x.Status, StartedAt = x.StartedAt, EndedAt = x.EndedAt, IsArchive = false }));
				items.AddRange(archive.GetAllCompletedWorkflows().Select(x => new { ID = x.ID, Status = x.Status, StartedAt = x.StartedAt, EndedAt = x.EndedAt, IsArchive = true }));
				items = items.OrderByDescending(x => x.StartedAt).ThenBy(x => x.EndedAt).ToList();
				await WriteResponse(resp, items);
				return;
			}
			var id = new Guid(idStr);
			var result = engine.ActiveWorkflows.FirstOrDefault(x => x.ID == id);
			if (result == null)
			{
				var archived = archive.GetCompletedWorkflow(id);
				if (archived != null)
					result = archived.State;
			}
			await WriteResponse(resp, result);
		}

		private static async Task HandleHumanInputRequest(IActFlowEngine engine, HttpListenerRequest req, HttpListenerResponse resp)
		{
			var body = GetRequestPostData(req);
			if (body == null)
				return;
			var input = JsonSerializer.Deserialize<UpdateHumanInput>(body, Constants.SerializerOpts);
			if (input == null)
				return;
			engine.ApplyHumanInput(input.ID, input.Input);
			await WriteResponse(resp, true);
		}

		private static async Task WriteResponse(HttpListenerResponse resp, object? result)
		{
			byte[] data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(result, Constants.SerializerOpts));
			resp.ContentType = "application/json";
			resp.ContentEncoding = Encoding.UTF8;
			resp.ContentLength64 = data.LongLength;
			await resp.OutputStream.WriteAsync(data, 0, data.Length);
		}

		// https://stackoverflow.com/a/5198080
		private static string? GetRequestPostData(HttpListenerRequest request)
		{
			if (!request.HasEntityBody)
			{
				return null;
			}
			using (System.IO.Stream body = request.InputStream) // here we have data
			{
				using (var reader = new System.IO.StreamReader(body, request.ContentEncoding))
				{
					return reader.ReadToEnd();
				}
			}
		}
	}
}
