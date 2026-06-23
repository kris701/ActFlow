using ActFlow.Archiver;
using ActFlow.CLI.Helpers;
using ActFlow.CLI.Models.FileSystem;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ActFlow.CLI.Controllers
{
	/// <summary>
	/// Endpoints for the file system
	/// </summary>
	[ApiController]
	[Route("api/fs/temporary")]
	public class TemporaryFileSystemController : ControllerBase
	{
		private readonly IActFlowEngine _engine;
		private readonly IWorkflowArchive _archive;

		/// <summary>
		/// Main constructor
		/// </summary>
		/// <param name="engine"></param>
		/// <param name="archive"></param>
		public TemporaryFileSystemController(IActFlowEngine engine, IWorkflowArchive archive)
		{
			_engine = engine;
			_archive = archive;
		}

		/// <summary>
		/// Get full tmp file tree for a workflow run
		/// </summary>
		/// <returns></returns>
		[HttpGet("root")]
		public async Task<ActionResult<DirectoryRoot>> Get_Files([FromQuery] GetForRunModel input)
		{
			var active = _engine.ActiveWorkflows.FirstOrDefault(x => x.ID == input.ID);
			if (active != null)
				return Ok(DirectoryHelpers.GetFullDirectory(Path.Combine(_engine.RunnerDirectory, active.ID.ToString())));
			if (_engine.CompletedDirectory == null)
				throw new Exception("Archive not active!");
			var completed = _archive.GetCompletedWorkflow(input.ID);
			if (completed == null)
				throw new Exception("Workflow not found in archive or in active workflows!");
			return Ok(DirectoryHelpers.GetFullDirectory(Path.Combine(_engine.CompletedDirectory, completed.State.ID.ToString(), "tmp")));
		}

		/// <summary>
		/// Download a single file
		/// </summary>
		/// <returns></returns>
		[HttpGet("files")]
		public async Task<IActionResult> Get_File([FromQuery] GetFileForRunModel input)
		{
			var target = "";
			var active = _engine.ActiveWorkflows.FirstOrDefault(x => x.ID == input.ID);
			if (active != null)
				target = Path.Combine(_engine.RunnerDirectory, active.ID.ToString(), input.Path);
			if (target == "")
			{
				if (_engine.CompletedDirectory == null)
					throw new Exception("Archive not active!");
				var completed = _archive.GetCompletedWorkflow(input.ID);
				if (completed == null)
					throw new Exception("Workflow not found in archive or in active workflows!");
				target = Path.Combine(_engine.CompletedDirectory, completed.State.ID.ToString(), "tmp", input.Path);
			}

			if (!System.IO.File.Exists(target))
				throw new Exception("File does not exist!");
			var fileInfo = new FileInfo(target);

			var data = new FileStream(target, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
			data.Position = 0;
			Response.Headers.Append("Content-Length", $"{data.Length}");
			return File(data, "application/octet-stream", fileInfo.Name, true);
		}
	}
}
