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
			var workers = JsonSerializer.Deserialize<List<IWorker>>(configFile, Constants._serializerOpts);
			if (workers == null)
				throw new Exception("Config is malformed!");

			ConsoleHelpers.WriteLineColor("Initializing engine...", ConsoleColor.Blue);
			IActFlowEngine engine = new ActFlowEngine(workers)
			{
				RemoveDelay = TimeSpan.FromSeconds(opts.Lifetime),
				ActivityLimiter = opts.Limiter,
				PersistentDirectory = opts.PersistentDirectory,
				TemporaryDirectory = opts.RunnerDirectory
			};

			HttpListener listener = new HttpListener();
			listener.Prefixes.Add(opts.Host);
			listener.Start();
			ConsoleHelpers.WriteLineColor($"Now listening on '{opts.Host}'", ConsoleColor.Green);
			while (true)
			{
				try
				{
					HttpListenerContext ctx = await listener.GetContextAsync();
					HttpListenerRequest req = ctx.Request;
					HttpListenerResponse resp = ctx.Response;

					if (req.Url != null)
					{
						ConsoleHelpers.WriteLineColor($"\tRequest on '{req.Url.AbsolutePath}'", ConsoleColor.DarkGray);

						if ((req.HttpMethod == "POST") && (req.Url.AbsolutePath == "/run"))
							await HandleRunRequest(engine, req, resp);
						else if ((req.HttpMethod == "POST") && (req.Url.AbsolutePath == "/queue"))
							await HandleQueueRequest(engine, req, resp);
						else if ((req.HttpMethod == "GET") && (req.Url.AbsolutePath == "/status"))
							await HandleStatusRequest(engine, req, resp);
					}

					resp.Close();
				}
				catch (Exception ex)
				{
					ConsoleHelpers.WriteLineColor("Unknown error: " + ex.Message, ConsoleColor.Red);
				}
			}
		}

		private static async Task HandleRunRequest(IActFlowEngine engine, HttpListenerRequest req, HttpListenerResponse resp)
		{
			var body = GetRequestPostData(req);
			if (body == null)
				return;
			var workflow = JsonSerializer.Deserialize<Workflow>(body, Constants._serializerOpts);
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
			var workflow = JsonSerializer.Deserialize<Workflow>(body, Constants._serializerOpts);
			if (workflow == null)
				return;

			var result = engine.Execute(workflow);
			await WriteResponse(resp, result);
		}

		private static async Task HandleStatusRequest(IActFlowEngine engine, HttpListenerRequest req, HttpListenerResponse resp)
		{
			var parsedString = HttpUtility.HtmlDecode(req.Url!.Query);
			var idStr = HttpUtility.ParseQueryString(parsedString)["id"];
			if (idStr == null)
			{
				await WriteResponse(resp, engine.ActiveWorkflows.Select(x => new { ID = x.ID, Status = x.Status, StartedAt = x.StartedAt, EndedAt = x.EndedAt }));
				return;
			}
			var id = new Guid(idStr);
			var result = engine.ActiveWorkflows.FirstOrDefault(x => x.ID == id);
			await WriteResponse(resp, result);
		}

		private static async Task WriteResponse(HttpListenerResponse resp, object? result)
		{
			byte[] data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(result, Constants._serializerOpts));
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
