using ActFlow.Archiver;
using ActFlow.Archiver.Models;
using ActFlow.CLI.Helpers;
using ActFlow.CLI.Models;
using ActFlow.Models.Workflows;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace ActFlow.CLI.Controllers
{
	/// <summary>
	/// Endpoints for persistent files
	/// </summary>
	[ApiController]
	[Route("api/files")]
	public class FilesController : ControllerBase
	{
		private readonly IActFlowEngine _engine;

		/// <summary>
		/// Main constructor
		/// </summary>
		/// <param name="engine"></param>
		public FilesController(IActFlowEngine engine)
		{
			_engine = engine;
		}

		/// <summary>
		/// Get full persistent file tree
		/// </summary>
		/// <returns></returns>
		[HttpGet("all")]
		public async Task<ActionResult<DirectoryRoot>> Get_Files() => Ok(DirectoryHelpers.GetFullDirectory(_engine.PersistentDirectory));

		/// <summary>
		/// Download a single file
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> Get_File([Required][FromHeader] string path)
		{
			var target = Path.Combine(_engine.PersistentDirectory, path);
			if (!System.IO.File.Exists(target))
				throw new Exception("File does not exist!");
			var fileInfo = new FileInfo(target);

			var data = new FileStream(target, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
			data.Position = 0;
			Response.Headers.Append("Content-Length", $"{data.Length}");
			return File(data, "application/octet-stream", fileInfo.Name, true);
		}

		/// <summary>
		/// Delete a single file
		/// </summary>
		/// <returns></returns>
		[HttpDelete]
		public async Task<IActionResult> Delete_File([Required][FromHeader] string path)
		{
			var target = Path.Combine(_engine.PersistentDirectory, path);
			if (!System.IO.File.Exists(target))
				throw new Exception("File does not exist!");
			var fileInfo = new FileInfo(target);
			System.IO.File.Delete(target);
			return Ok();
		}
	}
}
