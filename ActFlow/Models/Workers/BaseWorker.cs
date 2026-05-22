using ActFlow.Models.Activities;
using ActFlow.Models.Contexts;
using ActFlow.Models.Workflows;
using System.Text.Json.Serialization;

namespace ActFlow.Models.Workers
{
	public abstract class BaseWorker<T> : IWorker where T : IActivity
	{
		[JsonPropertyName("id")]
		public string ID { get; set; }

		[JsonIgnore]
		public string PersistenDirectory { get; set; }

		protected BaseWorker(string iD)
		{
			ID = iD;
		}


		public abstract Task<WorkerResult> Execute(T act, WorkflowState state, CancellationToken token, string tmpDirectory);

		public async Task<WorkerResult> ExecuteActionAsync(dynamic act, WorkflowState state, CancellationToken token, string tmpDirectory)
		{
			if (act is T actualAct)
				return await Execute(actualAct, state, token, tmpDirectory);
			throw new Exception("Invalid action input to executor!");
		}

		public async Task<string> LoadFile(string path, FileDirectories type, string tmpDirectory, CancellationToken token)
		{
			switch (type)
			{
				case FileDirectories.Persistent:
					var pPath = Path.Combine(PersistenDirectory, path);
					if (!File.Exists(pPath))
						throw new Exception("File not found!");
					return await File.ReadAllTextAsync(pPath, token);
				case FileDirectories.Temporary:
					var tPath = Path.Combine(tmpDirectory, path);
					if (!File.Exists(tPath))
						throw new Exception("File not found!");
					return await File.ReadAllTextAsync(tPath, token);
				default:
					throw new Exception("Unknown file directory!");
			}
		}

		public async Task SaveFile(string path, string content, FileDirectories type, string tmpDirectory, CancellationToken token)
		{
			switch (type)
			{
				case FileDirectories.Persistent:
					var pPath = Path.Combine(PersistenDirectory, path);
					await File.WriteAllTextAsync(pPath, content, token);
					break;
				case FileDirectories.Temporary:
					var tPath = Path.Combine(tmpDirectory, path);
					await File.WriteAllTextAsync(tPath, content, token);
					break;
				default:
					throw new Exception("Unknown file directory!");
			}
		}

		public List<string> ListFiles(string path, FileDirectories type, string tmpDirectory)
		{
			switch (type)
			{
				case FileDirectories.Persistent:
					var pPath = Path.Combine(PersistenDirectory, path);
					var pInfo = new DirectoryInfo(pPath);
					return pInfo.GetFiles().Select(x => x.Name).ToList();
				case FileDirectories.Temporary:
					var tPath = Path.Combine(tmpDirectory, path);
					var tInfo = new DirectoryInfo(tPath);
					return tInfo.GetFiles().Select(x => x.Name).ToList();
				default:
					throw new Exception("Unknown file directory!");
			}
		}
	}
}
