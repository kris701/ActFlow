using ActFlow.Archiver;
using ActFlow.CLI.Helpers;
using ActFlow.CLI.Models;
using ActFlow.Models.Workers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi;
using System.Reflection;
using System.Text.Json;
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
				RunnerDirectory = opts.RunnerDirectory,
				CompletedDirectory = opts.CompletedDirectory
			};
			await engine.Initialize();
			IWorkflowArchive archive = new WorkflowArchive()
			{
				CompletedDirectory = opts.CompletedDirectory
			};
			await archive.Initialize();

			ConsoleHelpers.WriteLineColor("Starting API...", ConsoleColor.Blue);

			var builder = WebApplication.CreateBuilder();
			builder.WebHost.ConfigureKestrel(o =>
			{
				o.ListenAnyIP(opts.Port);
			});
			builder.Logging.AddFilter("Microsoft.AspNetCore", Microsoft.Extensions.Logging.LogLevel.Warning);
			builder.Logging.SetMinimumLevel(LogLevel.Information);

			builder.Services.AddSwaggerGen(c =>
			{
				var thisVersion = Assembly.GetEntryAssembly()?.GetName().Version!;
				var thisVersionStr = $"v{thisVersion.Major}.{thisVersion.Minor}.{thisVersion.Build}";
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "ActFlow API", Version = thisVersionStr });
				var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
				c.AddServer(new OpenApiServer() { Description = "Primary", Url = $"http://localhost:{opts.Port}" });
			});
			builder.Services.AddControllers().AddJsonOptions(o =>
			{
				o.JsonSerializerOptions.WriteIndented = Constants.SerializerOpts.WriteIndented;
				o.JsonSerializerOptions.TypeInfoResolver = Constants.SerializerOpts.TypeInfoResolver;
				o.JsonSerializerOptions.AllowTrailingCommas = Constants.SerializerOpts.AllowTrailingCommas;
				o.JsonSerializerOptions.ReadCommentHandling = Constants.SerializerOpts.ReadCommentHandling;
				o.JsonSerializerOptions.NumberHandling = Constants.SerializerOpts.NumberHandling;
				o.JsonSerializerOptions.Converters.Clear();
				foreach (var converter in Constants.SerializerOpts.Converters)
					o.JsonSerializerOptions.Converters.Add(converter);
			});
			builder.Services.AddSingleton<IActFlowEngine>(engine);
			builder.Services.AddSingleton<IWorkflowArchive>(archive);

			var app = builder.Build();

			app.UseSwagger();
			app.UseSwaggerUI();
			app.UseRouting();
			app.MapControllers();

			await app.RunAsync();
		}
	}
}
