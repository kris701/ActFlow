using System.Text.Json.Serialization;
using ToolsSharp.Interfaces;

namespace ActFlow.Models.Workflows
{
	public class WorkflowState : IGenericClonable<WorkflowState>
	{
		public delegate Task WorkflowEventHandler(WorkflowState state);

		public event WorkflowEventHandler? OnWorkflowUpdated;
		public event WorkflowEventHandler? OnWorkflowCompleted;

		[JsonPropertyName("id")]
		public Guid ID { get; set; }
		public WorkflowStatuses Status { get; set; }
		public int ActivityIndex { get; set; }
		public Dictionary<string, string> ContextStore { get; set; }
		public DateTime? StartedAt { get; set; }
		public DateTime? EndedAt { get; set; }
		public string LogText { get; set; }
		public IWorkflow Workflow { get; set; }

		public bool IsProcessingUserInput { get; set; } = false;

		[JsonIgnore]
		public CancellationTokenSource TokenSource { get; private set; } = new CancellationTokenSource();

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

		public WorkflowState(IWorkflow workflow)
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

		public WorkflowState Clone() => new WorkflowState(this);

		public void AppendToLog() => AppendToLog("");
		public void AppendToLog(string text)
		{
			LogText += $"{text}{Environment.NewLine}";
		}

		public void AppendToLogError(string text)
		{
			LogText += $"ERR: {text}{Environment.NewLine}";
		}

		public async Task Update()
		{
			if (OnWorkflowUpdated != null)
				await OnWorkflowUpdated.Invoke(this);
		}

		public async Task Complete()
		{
			if (OnWorkflowCompleted != null)
				await OnWorkflowCompleted.Invoke(this);
		}

		public void AddContext(string key, string value)
		{
			ContextStore.Add(key.ToLower(), value);
		}

		public void AddContexts(Dictionary<string, string> values)
		{
			foreach (var key in values.Keys)
				AddContext(key, values[key]);
		}
	}
}
