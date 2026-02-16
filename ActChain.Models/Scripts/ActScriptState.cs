using System.Text.Json.Serialization;
using ToolsSharp.Interfaces;

namespace ActChain.Models.Scripts
{
	public class ActScriptState : IGenericClonable<ActScriptState>
	{
		public delegate Task ActScriptEventHandler(ActScriptState item);

		public event ActScriptEventHandler? OnChainUpdated;
		public event ActScriptEventHandler? OnChainCompleted;

		[JsonPropertyName("id")]
		public Guid ID { get; set; }
		public ScriptStatus Status { get; set; }
		public int Stage { get; set; }
		public Dictionary<string, string> ContextStore { get; set; }
		public DateTime? StartedAt { get; set; }
		public DateTime? EndedAt { get; set; }
		public string LogText { get; set; }
		public ActScript Script { get; set; }

		public bool IsProcessingUserInput { get; set; } = false;

		[JsonIgnore]
		public CancellationTokenSource TokenSource { get; private set; } = new CancellationTokenSource();

		public ActScriptState()
		{
			ID = Guid.NewGuid();
			Script = new ActScript();
			Stage = 0;
			Status = ScriptStatus.None;
			StartedAt = null;
			EndedAt = null;
			LogText = "";
			ContextStore = new Dictionary<string, string>();
		}

		public ActScriptState(ActScript script)
		{
			ID = Guid.NewGuid();
			Script = script;
			Stage = 0;
			Status = ScriptStatus.NotStarted;
			StartedAt = null;
			EndedAt = null;
			LogText = "";
			ContextStore = new Dictionary<string, string>();
			AddContexts(script.Globals);
			TokenSource = new CancellationTokenSource();
		}

		public ActScriptState(ActScriptState other)
		{
			ID = other.ID;
			Script = other.Script;
			Stage = other.Stage;
			Status = other.Status;
			StartedAt = other.StartedAt;
			EndedAt = other.EndedAt;
			LogText = other.LogText;
			ContextStore = other.ContextStore;
			if (other.OnChainUpdated != null)
				OnChainUpdated += (x) => other.OnChainUpdated.Invoke(x);
			if (other.OnChainCompleted != null)
				OnChainCompleted += (x) => other.OnChainCompleted.Invoke(x);
			TokenSource = new CancellationTokenSource();
		}

		public ActScriptState Clone() => new ActScriptState(this);

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
			if (OnChainUpdated != null)
				await OnChainUpdated.Invoke(this);
		}

		public async Task Complete()
		{
			if (OnChainCompleted != null)
				await OnChainCompleted.Invoke(this);
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
