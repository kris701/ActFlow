using ActFlow.Archiver;
using ActFlow.Archiver.Models;
using ActFlow.CLI.Models.Status;
using ActFlow.Models.Workflows;
using Microsoft.AspNetCore.Mvc;

namespace ActFlow.CLI.Controllers
{
	/// <summary>
	/// Endpoints 
	/// </summary>
	[ApiController]
	[Route("api/status")]
	public class StatusController : ControllerBase
	{
		private readonly IActFlowEngine _engine;
		private readonly IWorkflowArchive _archive;

		/// <summary>
		/// Main constructor
		/// </summary>
		/// <param name="engine"></param>
		/// <param name="archive"></param>
		public StatusController(IActFlowEngine engine, IWorkflowArchive archive)
		{
			_engine = engine;
			_archive = archive;
		}

		/// <summary>
		/// Get the status of the ActFlow system
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public async Task<ActionResult<StatusModel>> Get_Status()
		{
			var items = new List<ListWorkflowState>();
			items.AddRange(_engine.ActiveWorkflows.Select(x => new ListWorkflowState() { ID = x.ID, Status = x.Status, StartedAt = x.StartedAt, EndedAt = x.EndedAt, Name = x.Workflow.Name, IsArchived = false }));
			items.AddRange(_archive.GetAllCompletedWorkflows().Select(x => new ListWorkflowState() { ID = x.ID, Status = x.Status, StartedAt = x.StartedAt, EndedAt = x.EndedAt, Name = x.Name, IsArchived = x.IsArchived }));

			var activeStateMap = new Dictionary<WorkflowStatuses, int>();
			foreach (var type in Enum.GetValues<WorkflowStatuses>())
				activeStateMap.Add(type, 0);
			foreach (var active in _engine.ActiveWorkflows)
				activeStateMap[active.Status]++;

			var archived = _archive.GetAllCompletedWorkflows();
			var archivedStateMap = new Dictionary<WorkflowStatuses, int>();
			foreach (var type in Enum.GetValues<WorkflowStatuses>())
				archivedStateMap.Add(type, 0);
			foreach (var archive in archived)
				archivedStateMap[archive.Status]++;

			StatusModelRun? mostExpensiveWorkflow = null;
			if (items.Count > 0)
			{
				var item = items.Where(x => x.StartedAt != null && x.EndedAt != null).OrderByDescending(x => ((DateTime)x.EndedAt! - (DateTime)x.StartedAt!).TotalSeconds).FirstOrDefault();
				if (item != null)
					mostExpensiveWorkflow = new StatusModelRun()
					{
						ID = item.ID,
						Name = item.Name,
						Runtime = ((DateTime)item.EndedAt! - (DateTime)item.StartedAt!)
					};
			}

			StatusModelRun? leastExpensiveWorkflow = null;
			if (items.Count > 0)
			{
				var item = items.Where(x => x.StartedAt != null && x.EndedAt != null).OrderBy(x => ((DateTime)x.EndedAt! - (DateTime)x.StartedAt!).TotalSeconds).FirstOrDefault();
				if (item != null)
					leastExpensiveWorkflow = new StatusModelRun()
					{
						ID = item.ID,
						Name = item.Name,
						Runtime = ((DateTime)item.EndedAt! - (DateTime)item.StartedAt!)
					};
			}

			var totalRuntime = TimeSpan.FromSeconds(0);
			foreach (var item in items)
				if (item.StartedAt != null && item.EndedAt != null && item.EndedAt > item.StartedAt)
					totalRuntime += (DateTime)item.EndedAt! - (DateTime)item.StartedAt!;

			var oldestRun = items.Where(x => x.EndedAt != null).OrderBy(x => x.EndedAt).FirstOrDefault()?.EndedAt;
			var newestRun = items.Where(x => x.EndedAt != null).OrderByDescending(x => x.EndedAt).FirstOrDefault()?.EndedAt;

			var runsPrDay = new Dictionary<DateTime, int>();
			for (int i = 0; i < 7; i++)
				runsPrDay.Add(DateTime.UtcNow.AddDays(-i), items.Where(x => x.EndedAt != null).Where(x => ((DateTime)x.EndedAt!).DayOfYear == DateTime.UtcNow.AddDays(-i).DayOfYear).Count());

			return Ok(new StatusModel()
			{
				ActiveWorkflows = _engine.ActiveWorkflows.Count,
				ArchivedWorkflows = archived.Count,

				MostExpensiveRun = mostExpensiveWorkflow,
				LeastExpensiveRun = leastExpensiveWorkflow,

				TotalRuntime = totalRuntime,

				OldestRun = oldestRun,
				LatestRun = newestRun,

				ActiveStateMap = activeStateMap,
				ArchivedStateMap = archivedStateMap,

				RunsPrDay = runsPrDay
			});
		}
	}
}
