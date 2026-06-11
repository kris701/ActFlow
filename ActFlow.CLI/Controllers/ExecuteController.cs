using ActFlow.CLI.Models;
using ActFlow.Models.Workflows;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ActFlow.CLI.Controllers
{
	[ApiController]
	[Route("api")]
	public class ExecuteController : ControllerBase
	{
		private readonly IActFlowEngine _engine;

		public ExecuteController(IActFlowEngine engine)
		{
			_engine = engine;
		}

		[HttpPost("run")]
		public async Task<ActionResult<WorkflowState>> Post_Run([Required][FromBody] Workflow inputModel)
		{
			var result = await _engine.ExecuteAsync(inputModel);
			return Ok(result);
		}

		[HttpPost("queue")]
		public async Task<ActionResult<Guid>> Post_Queue([Required][FromBody] Workflow inputModel)
		{
			var result = _engine.Execute(inputModel);
			return Ok(result);
		}

		[HttpPost("input")]
		public async Task<ActionResult<Guid>> Post_Input([Required][FromBody] UpdateHumanInput inputModel)
		{
			_engine.ApplyHumanInput(inputModel.ID, inputModel.Input);
			return Ok();
		}

		[HttpDelete("cancel")]
		public async Task<ActionResult> Delete_Cancel([Required][FromQuery] Guid id)
		{
			_engine.Cancel(id);
			return Ok();
		}
	}
}
