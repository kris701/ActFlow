using ActFlow.Models.Activities;
using ActFlow.Models.Attributes;
using ActFlow.Models.Contexts;
using ActFlow.Models.Workflows;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ActFlow.Models.Workers
{
	public abstract class BaseWorker<T> : IWorker where T : IActivity
	{
		[StringLength(256, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 256 characters long!")]
		[StictLowerCaseString]
		public string ID { get; set; } = "default";

		[JsonIgnore]
		public string PersistenDirectory { get; set; }

		public abstract Task<WorkerResult> Execute(T act, WorkflowState state, CancellationToken token, string tmpDirectory);

		public async Task<WorkerResult> ExecuteActionAsync(dynamic act, WorkflowState state, CancellationToken token, string tmpDirectory)
		{
			if (act is T actualAct)
				return await Execute(actualAct, state, token, tmpDirectory);
			throw new Exception("Invalid action input to executor!");
		}

		public async Task<string> LoadFileText(string path, FileDirectories type, string tmpDirectory, WorkflowState state, CancellationToken token)
		{
			switch (type)
			{
				case FileDirectories.Persistent:
					var pPath = Path.Combine(PersistenDirectory, path);
					if (!File.Exists(pPath))
						throw new Exception("File not found!");
					var pData = await File.ReadAllTextAsync(pPath, token);
					state.Files.Add(new WorkflowFile()
					{
						Path = path,
						Directory = FileDirectories.Persistent,
						Action = WorkflowFileActionTypes.Load
					});
					return pData;
				case FileDirectories.Temporary:
					var tPath = Path.Combine(tmpDirectory, path);
					if (!File.Exists(tPath))
						throw new Exception("File not found!");
					var tData = await File.ReadAllTextAsync(tPath, token);
					state.Files.Add(new WorkflowFile()
					{
						Path = path,
						Directory = FileDirectories.Temporary,
						Action = WorkflowFileActionTypes.Load
					});
					return tData;
				default:
					throw new Exception("Unknown file directory!");
			}
		}

		public Stream LoadFileStream(string path, FileDirectories type, string tmpDirectory, WorkflowState state, CancellationToken token)
		{
			switch (type)
			{
				case FileDirectories.Persistent:
					var pPath = Path.Combine(PersistenDirectory, path);
					if (!File.Exists(pPath))
						throw new Exception("File not found!");
					var pData = File.OpenRead(pPath);
					state.Files.Add(new WorkflowFile()
					{
						Path = path,
						Directory = FileDirectories.Persistent,
						Action = WorkflowFileActionTypes.Load
					});
					return pData;
				case FileDirectories.Temporary:
					var tPath = Path.Combine(tmpDirectory, path);
					if (!File.Exists(tPath))
						throw new Exception("File not found!");
					var tData = File.OpenRead(tPath);
					state.Files.Add(new WorkflowFile()
					{
						Path = path,
						Directory = FileDirectories.Temporary,
						Action = WorkflowFileActionTypes.Load
					});
					return tData;
				default:
					throw new Exception("Unknown file directory!");
			}
		}

		public async Task SaveFileText(string path, string content, FileDirectories type, string tmpDirectory, WorkflowState state, CancellationToken token)
		{
			switch (type)
			{
				case FileDirectories.Persistent:
					var pPath = Path.Combine(PersistenDirectory, path);
					await File.WriteAllTextAsync(pPath, content, token);
					state.Files.Add(new WorkflowFile()
					{
						Path = path,
						Directory = FileDirectories.Persistent,
						Action = WorkflowFileActionTypes.Save
					});
					break;
				case FileDirectories.Temporary:
					var tPath = Path.Combine(tmpDirectory, path);
					await File.WriteAllTextAsync(tPath, content, token);
					state.Files.Add(new WorkflowFile()
					{
						Path = path,
						Directory = FileDirectories.Temporary,
						Action = WorkflowFileActionTypes.Save
					});
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

		public string GetFileName(string path, FileDirectories type, string tmpDirectory)
		{
			switch (type)
			{
				case FileDirectories.Persistent:
					var pPath = Path.Combine(PersistenDirectory, path);
					var pInfo = new FileInfo(pPath);
					return pInfo.Name;
				case FileDirectories.Temporary:
					var tPath = Path.Combine(tmpDirectory, path);
					var tInfo = new FileInfo(tPath);
					return tInfo.Name;
				default:
					throw new Exception("Unknown file directory!");
			}
		}
	}
}
