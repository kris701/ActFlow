using ActFlow.Archiver;
using ActFlow.Archiver.Models;
using ActFlow.Models.Workflows;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ActFlow.CLI.Controllers
{
	/// <summary>
	/// Controller endpoints to get workflow results
	/// </summary>
	[ApiController]
	[Route("api/results")]
	public class ResultsController : ControllerBase
	{
		private readonly IActFlowEngine _engine;
		private readonly IWorkflowArchive _archive;

		/// <summary>
		/// Main constructor
		/// </summary>
		/// <param name="engine"></param>
		/// <param name="archive"></param>
		public ResultsController(IActFlowEngine engine, IWorkflowArchive archive)
		{
			_engine = engine;
			_archive = archive;
		}

		/// <summary>
		/// Get a detailed result from a workflow run.
		/// This can both be an active run or an archived one.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		[HttpGet]
		public async Task<ActionResult<WorkflowState>> Get_Result([Required][FromQuery] Guid id)
		{
			var result = _engine.ActiveWorkflows.FirstOrDefault(x => x.ID == id);
			if (result == null)
			{
				var archived = _archive.GetCompletedWorkflow(id);
				if (archived != null)
					result = archived.State;
			}
			if (result == null)
				throw new Exception($"Could not find any workflow with that id ({id})!");

			return Ok(result);
		}

		/// <summary>
		/// Delete a completed workflow from the archive
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpDelete]
		public async Task<ActionResult> Delete_Result([Required][FromQuery] Guid id)
		{
			_archive.RemoveCompletedWorkflow(id);
			return Ok();
		}

		/// <summary>
		/// Get a overview of all completed workflows
		/// </summary>
		/// <returns></returns>
		[HttpGet("all")]
		public async Task<ActionResult<List<ListWorkflowState>>> Get_Results()
		{
			var items = new List<ListWorkflowState>();
			items.AddRange(_engine.ActiveWorkflows.Select(x => new ListWorkflowState() { ID = x.ID, Status = x.Status, StartedAt = x.StartedAt, EndedAt = x.EndedAt, Name = x.Workflow.Name, IsArchived = false }));
			items.AddRange(_archive.GetAllCompletedWorkflows().Select(x => new ListWorkflowState() { ID = x.ID, Status = x.Status, StartedAt = x.StartedAt, EndedAt = x.EndedAt, Name = x.Name, IsArchived = x.IsArchived }));
			items = items.OrderByDescending(x => x.StartedAt).ThenBy(x => x.EndedAt).ToList();

			return Ok(items);
		}
	}
}
