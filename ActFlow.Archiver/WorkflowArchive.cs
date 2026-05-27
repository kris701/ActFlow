using ActFlow.Archiver.Models;
using ActFlow.Models.Workflows;
using System.Text.Json;
using System.Text.Json.Serialization;
using ToolsSharp;

namespace ActFlow.Archiver
{
	public class WorkflowArchive : IWorkflowArchive
	{
		/// <summary>
		/// Amount of completed workflows stored
		/// </summary>
		public int CompletedWorkflows { get; private set; } = 0;
		/// <summary>
		/// The path to where to save completed workflow runs
		/// </summary>
		public string CompletedDirectory { get; set; } = ".completed";

		private bool _updatingCache;
		private Dictionary<Guid, ListWorkflowState> _cache = new Dictionary<Guid, ListWorkflowState>();
		private FileSystemWatcher _watcher = new FileSystemWatcher();

		public WorkflowArchive()
		{
			if (!Constants.SerializerOpts.Converters.Any(x => x.GetType() == typeof(JsonStringEnumConverter)) &&
				!Constants.SerializerOpts.Converters.IsReadOnly)
				Constants.SerializerOpts.Converters.Add(new JsonStringEnumConverter());
		}

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
				var stateFile = Path.Combine(folder, "state.json");

				if (!File.Exists(stateFile))
					continue;
				var state = JsonSerializer.Deserialize<WorkflowState>(stateFile, Constants.SerializerOpts);
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
			_updatingCache = false;
		}

		public List<ListWorkflowState> GetAllCompletedWorkflows() => _cache.Values.ToList();

		public CompletedWorkflowState GetCompletedWorkflow(Guid id)
		{
			if (!_cache.ContainsKey(id))
				throw new Exception("No completed workflow exists with that ID!");
			var listState = _cache[id];

			var stateFile = Path.Combine(CompletedDirectory, id.ToString(), "state.json");
			var state = JsonSerializer.Deserialize<WorkflowState>(stateFile, Constants.SerializerOpts);
			if (state == null)
				throw new Exception("Could not deserialize the state file!");

			var tmpFilesPath = Path.Combine(CompletedDirectory, id.ToString(), "tmp");
			string[] allfiles = Directory.GetFiles(tmpFilesPath, "*.*", SearchOption.AllDirectories);

			return new CompletedWorkflowState()
			{
				State = state,
				Files = allfiles.ToList(),
			};
		}

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
