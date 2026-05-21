using ActFlow.CLI.Models;
using ActFlow.Models.Workers;
using ActFlow.Models.Workflows;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Web;

namespace ActFlow.CLI.Programs
{
	public static class ServeProgram
	{
		public static async Task Run(ServeOptions opts)
		{
			Console.WriteLine("Checking paths...");
			var configFile = File.ReadAllText(opts.ConfigPath);

			Console.WriteLine("Loading plugins...");
			var plugins = Directory.GetDirectories(Constants._pluginPath);
			Console.WriteLine($"\t{plugins.Length} plugins to load");
			foreach (var plugin in plugins)
			{
				var lib = Path.Combine(plugin, "lib");
				if (!Directory.Exists(lib))
					continue;
				var libDirs = Directory.GetDirectories(lib);
				if (libDirs.Length == 0)
					continue;
				var first = libDirs.First();

				var files = Directory.GetFiles(plugin);
				var target = files.FirstOrDefault(x => x.EndsWith(".nuspec"));
				if (target == null)
					continue;
				target = target.Substring(target.LastIndexOf('\\') + 1);
				var actualName = target.Substring(0, target.LastIndexOf(".nuspec"));
				var dllToLoad = Path.Combine(first, actualName + ".dll");

				var assemblyName = AssemblyName.GetAssemblyName(dllToLoad);
				if (AppDomain.CurrentDomain.GetAssemblies().Any(x => x.GetName().Name == assemblyName.Name))
					throw new Exception($"Assembly with name '{assemblyName.Name}' already loaded!");
				Assembly.LoadFrom(dllToLoad);
				AppDomain.CurrentDomain.Load(assemblyName);
			}

			Console.WriteLine("Parsing Config...");
			var workers = JsonSerializer.Deserialize<List<IWorker>>(configFile, Constants._serializerOpts);
			if (workers == null)
				throw new Exception("Config is malformed!");

			Console.WriteLine("Initializing engine...");
			IActFlowEngine engine = new ActFlowEngine(workers) 
			{ 
				RemoveDelay = TimeSpan.FromSeconds(opts.Lifetime),
				ActivityLimiter = opts.Limiter,
				PersistentDirectory = opts.PersistentDirectory,
				TemporaryDirectory = opts.RunnerDirectory
			};

			HttpListener listener = new HttpListener();
			var url = $"http://localhost:{opts.Port}/";
			listener.Prefixes.Add(url);
			listener.Start();
			Console.WriteLine($"Now listening on '{url}'");
			while (true)
			{
				try
				{
					HttpListenerContext ctx = await listener.GetContextAsync();
					HttpListenerRequest req = ctx.Request;
					HttpListenerResponse resp = ctx.Response;

					if (req.Url != null)
					{
						Console.WriteLine($"\tRequest on '{req.Url.AbsolutePath}'");

						if ((req.HttpMethod == "POST") && (req.Url.AbsolutePath == "/run"))
							await HandleRunRequest(engine, req, resp);
						else if ((req.HttpMethod == "POST") && (req.Url.AbsolutePath == "/queue"))
							await HandleQueueRequest(engine, req, resp);
						else if ((req.HttpMethod == "GET") && (req.Url.AbsolutePath == "/result"))
							await HandleResultRequest(engine, req, resp);
					}

					resp.Close();
				}
				catch(Exception ex)
				{
					Console.WriteLine("Unknown error: " + ex.Message);
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

		private static async Task HandleResultRequest(IActFlowEngine engine, HttpListenerRequest req, HttpListenerResponse resp)
		{
			var parsedString = HttpUtility.HtmlDecode(req.Url!.Query);
			var idStr = HttpUtility.ParseQueryString(parsedString)["id"];
			if (idStr == null)
				return;
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
