using ActFlow.CLI.Models;
using ActFlow.Models.Workflows;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ActFlow.CLI.Controllers
{
	/// <summary>
	/// Endpoints for interacting with the ActFlow engine
	/// </summary>
	[ApiController]
	[Route("api/execute")]
	public class ExecuteController : ControllerBase
	{
		private readonly IActFlowEngine _engine;

		/// <summary>
		/// Main constructor
		/// </summary>
		/// <param name="engine"></param>
		public ExecuteController(IActFlowEngine engine)
		{
			_engine = engine;
		}

		/// <summary>
		/// Start a workflow and wait for it to complete asynchronosly
		/// </summary>
		/// <param name="inputModel"></param>
		/// <returns></returns>
		[HttpPost("run")]
		public async Task<ActionResult<WorkflowState>> Post_Run([Required][FromBody] Workflow inputModel)
		{
			var result = await _engine.ExecuteAsync(inputModel);
			return Ok(result);
		}

		/// <summary>
		/// Start a workflow, but dont wait for it to finish. Instead just give back the run ID.
		/// </summary>
		/// <param name="inputModel"></param>
		/// <returns></returns>
		[HttpPost("queue")]
		public async Task<ActionResult<Guid>> Post_Queue([Required][FromBody] Workflow inputModel)
		{
			var result = _engine.Execute(inputModel);
			return Ok(result);
		}

		/// <summary>
		/// Apply some human input to an activity that supports it
		/// </summary>
		/// <param name="inputModel"></param>
		/// <returns></returns>
		[HttpPost("input")]
		public async Task<ActionResult<Guid>> Post_Input([Required][FromBody] UpdateHumanInput inputModel)
		{
			_engine.ApplyHumanInput(inputModel.ID, inputModel.Input);
			return Ok();
		}

		/// <summary>
		/// Cancel a running workflow
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpDelete("cancel")]
		public async Task<ActionResult> Delete_Cancel([Required][FromQuery] Guid id)
		{
			_engine.Cancel(id);
			return Ok();
		}
	}
}
