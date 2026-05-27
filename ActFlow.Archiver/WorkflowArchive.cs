using ActFlow.Archiver.Models;
using ActFlow.Models.Workflows;
using System.Text.Json;
using System.Text.Json.Serialization;
using ToolsSharp;

namespace ActFlow.Archiver
{
	/// <summary>
	/// Implementation to handle archival of completed workflows
	/// </summary>
	public class WorkflowArchive : IWorkflowArchive
	{
		/// <summary>
		/// The path to where to save completed workflow runs
		/// </summary>
		public string CompletedDirectory { get; set; } = ".completed";

		private bool _updatingCache;
		private Dictionary<Guid, ListWorkflowState> _cache = new Dictionary<Guid, ListWorkflowState>();
		private FileSystemWatcher _watcher = new FileSystemWatcher();

		/// <summary>
		/// Main constructor
		/// </summary>
		public WorkflowArchive()
		{
			if (!Constants.SerializerOpts.Converters.Any(x => x.GetType() == typeof(JsonStringEnumConverter)) &&
				!Constants.SerializerOpts.Converters.IsReadOnly)
				Constants.SerializerOpts.Converters.Add(new JsonStringEnumConverter());
		}

		/// <summary>
		/// Initialize the archiver
		/// </summary>
		/// <returns></returns>
		public async Task Initialize()
		{
			if (!Directory.Exists(CompletedDirectory))
				Directory.CreateDirectory(CompletedDirectory);

			_watcher = new FileSystemWatcher(CompletedDirectory);
			_watcher.NotifyFilter = NotifyFilters.Attributes
								 | NotifyFilters.CreationTime
								 | NotifyFilters.DirectoryName
								 | NotifyFilters.FileName
								 | NotifyFilters.LastAccess
								 | NotifyFilters.LastWrite
								 | NotifyFilters.Security
								 | NotifyFilters.Size;

			_watcher.IncludeSubdirectories = true;
			_watcher.EnableRaisingEvents = true;

			_watcher.Changed += OnChanged;
			_watcher.Created += OnChanged;
			_watcher.Deleted += OnChanged;
			_watcher.Renamed += OnChanged;

			LoadArchive();
		}

		private void OnChanged(object sender, FileSystemEventArgs e)
		{
			if (e.ChangeType == WatcherChangeTypes.Changed)
				LoadArchive();
		}

		private void LoadArchive()
		{
			if (_updatingCache)
				return;
			_updatingCache = true;
			_cache = new Dictionary<Guid, ListWorkflowState>();
			var folders = Directory.GetDirectories(CompletedDirectory);
			foreach (var folder in folders)
			{
				try
				{
					var stateFile = Path.Combine(folder, "state.json");

					if (!File.Exists(stateFile))
						continue;
					var state = JsonSerializer.Deserialize<SimpleWorkflowState>(File.ReadAllText(stateFile), Constants.SerializerOpts);
					if (state == null)
						continue;

					_cache.Add(state.ID, new ListWorkflowState()
					{
						ID = state.ID,
						Status = state.Status,
						Name = state.Workflow.Name,
						StartedAt = state.StartedAt,
						EndedAt = state.EndedAt
					});
				}
				catch (Exception) { }
			}
			_updatingCache = false;
		}

		/// <summary>
		/// Get a simplified list of all completed workflows
		/// </summary>
		/// <returns></returns>
		public List<ListWorkflowState> GetAllCompletedWorkflows() => _cache.Values.ToList();

		/// <summary>
		/// Gets details information on a single workflow
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public CompletedWorkflowState? GetCompletedWorkflow(Guid id)
		{
			if (!_cache.ContainsKey(id))
				return null;
			var listState = _cache[id];

			var stateFile = Path.Combine(CompletedDirectory, id.ToString(), "state.json");
			var state = JsonSerializer.Deserialize<WorkflowState>(File.ReadAllText(stateFile), Constants.SerializerOpts);
			if (state == null)
				return null;

			var tmpFilesPath = Path.Combine(CompletedDirectory, id.ToString(), "tmp");
			var allFiles = new List<string>();
			if (Directory.Exists(tmpFilesPath))
				allFiles = Directory.GetFiles(tmpFilesPath, "*.*", SearchOption.AllDirectories).ToList();

			return new CompletedWorkflowState()
			{
				State = state,
				Files = allFiles
			};
		}

		/// <summary>
		/// Removed a completed workflow
		/// </summary>
		/// <param name="id"></param>
		public void RemoveCompletedWorkflow(Guid id)
		{
			if (!_cache.ContainsKey(id))
				throw new Exception("No completed workflow exists with that ID!");
			var path = Path.Combine(CompletedDirectory, id.ToString());
			if (Directory.Exists(path))
				DirectoryHelper.DeleteDirectory(path);
		}
	}
}
