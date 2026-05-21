using System.Text.Json.Serialization;

namespace ActFlow.Models.Workflows
{
	/// <summary>
	/// Represents the state of a given workflow
	/// </summary>
	public class WorkflowState
	{
		/// <summary>
		/// Generel event handler for changes in the state
		/// </summary>
		/// <param name="state"></param>
		/// <returns></returns>
		public delegate Task WorkflowEventHandler(WorkflowState state);

		/// <summary>
		/// Event for when a workflow is updated
		/// </summary>
		public event WorkflowEventHandler? OnWorkflowUpdated;
		/// <summary>
		/// Event for when a workflow is completed
		/// </summary>
		public event WorkflowEventHandler? OnWorkflowCompleted;

		/// <summary>
		/// The ID of this state
		/// </summary>
		[JsonPropertyName("id")]
		public Guid ID { get; set; }
		/// <summary>
		/// Current workflow state status
		/// </summary>
		public WorkflowStatuses Status { get; set; }
		/// <summary>
		/// The index of the activity currently being executed
		/// </summary>
		public int ActivityIndex { get; set; }
		/// <summary>
		/// Store of contexts
		/// </summary>
		public Dictionary<string, string> ContextStore { get; set; }
		/// <summary>
		/// Timestamp for when the engine started executing this workflow
		/// </summary>
		public DateTime? StartedAt { get; set; }
		/// <summary>
		/// Timestamp for when the engine completed the execution of this workflow
		/// </summary>
		public DateTime? EndedAt { get; set; }
		/// <summary>
		/// Log text made during execution of the state
		/// </summary>
		public string LogText { get; set; }
		/// <summary>
		/// The workflow file used
		/// </summary>
		public Workflow Workflow { get; set; }

		[JsonIgnore]
		internal bool IsProcessingUserInput { get; set; } = false;
		[JsonIgnore]
		internal CancellationTokenSource TokenSource { get; private set; } = new CancellationTokenSource();

		/// <summary>
		/// Empty constructor
		/// </summary>
		public WorkflowState()
		{
			ID = Guid.NewGuid();
			Workflow = new Workflow();
			ActivityIndex = 0;
			Status = WorkflowStatuses.None;
			StartedAt = null;
			EndedAt = null;
			LogText = "";
			ContextStore = new Dictionary<string, string>();
		}

		/// <summary>
		/// Constructor from a workflow
		/// </summary>
		/// <param name="workflow"></param>
		public WorkflowState(Workflow workflow)
		{
			ID = Guid.NewGuid();
			Workflow = workflow;
			ActivityIndex = 0;
			Status = WorkflowStatuses.NotStarted;
			StartedAt = null;
			EndedAt = null;
			LogText = "";
			ContextStore = new Dictionary<string, string>();
			AddContexts(workflow.Globals);
			TokenSource = new CancellationTokenSource();
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="other"></param>
		public WorkflowState(WorkflowState other)
		{
			ID = other.ID;
			Workflow = other.Workflow;
			ActivityIndex = other.ActivityIndex;
			Status = other.Status;
			StartedAt = other.StartedAt;
			EndedAt = other.EndedAt;
			LogText = other.LogText;
			ContextStore = other.ContextStore;
			if (other.OnWorkflowUpdated != null)
				OnWorkflowUpdated += (x) => other.OnWorkflowUpdated.Invoke(x);
			if (other.OnWorkflowCompleted != null)
				OnWorkflowCompleted += (x) => other.OnWorkflowCompleted.Invoke(x);
			TokenSource = new CancellationTokenSource();
		}

		/// <summary>
		/// Append a spacer to the log
		/// </summary>
		public void AppendToLog() => AppendToLog("");
		/// <summary>
		/// Append some text to the log
		/// </summary>
		/// <param name="text"></param>
		public void AppendToLog(string text)
		{
			LogText += $"{text}{Environment.NewLine}";
		}

		/// <summary>
		/// Append an error to the log
		/// </summary>
		/// <param name="text"></param>
		public void AppendToLogError(string text)
		{
			LogText += $"ERR: {text}{Environment.NewLine}";
		}

		/// <summary>
		/// Force indicate an update occured
		/// </summary>
		/// <returns></returns>
		public async Task Update()
		{
			if (OnWorkflowUpdated != null)
				await OnWorkflowUpdated.Invoke(this);
		}

		internal async Task Complete()
		{
			if (OnWorkflowCompleted != null)
				await OnWorkflowCompleted.Invoke(this);
		}

		/// <summary>
		/// Add a single context to the context store
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void AddContext(string key, string value)
		{
			ContextStore.Add(key.ToLower(), value);
		}
		/// <summary>
		/// Add a dictionary of contexts to the context store
		/// </summary>
		/// <param name="values"></param>
		public void AddContexts(Dictionary<string, string> values)
		{
			foreach (var key in values.Keys)
				AddContext(key, values[key]);
		}
	}
}
