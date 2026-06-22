using ActFlow.CLI.Helpers;
using ActFlow.CLI.Models.FileSystem;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ToolsSharp;

namespace ActFlow.CLI.Controllers
{
	/// <summary>
	/// Endpoints for the file system
	/// </summary>
	[ApiController]
	[Route("api/fs/persistent")]
	public class PersistentFileSystemController : ControllerBase
	{
		private readonly IActFlowEngine _engine;

		/// <summary>
		/// Main constructor
		/// </summary>
		/// <param name="engine"></param>
		public PersistentFileSystemController(IActFlowEngine engine)
		{
			_engine = engine;
		}

		/// <summary>
		/// Get full persistent file tree
		/// </summary>
		/// <returns></returns>
		[HttpGet("root")]
		public async Task<ActionResult<DirectoryRoot>> Get_Files() => Ok(DirectoryHelpers.GetFullDirectory(_engine.PersistentDirectory));

		/// <summary>
		/// Download a single file
		/// </summary>
		/// <returns></returns>
		[HttpGet("files")]
		public async Task<IActionResult> Get_File([FromHeader] GetFileModel input)
		{
			var target = Path.Combine(_engine.PersistentDirectory, input.Path);
			if (!System.IO.File.Exists(target))
				throw new Exception("File does not exist!");
			var fileInfo = new FileInfo(target);

			var data = new FileStream(target, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
			data.Position = 0;
			Response.Headers.Append("Content-Length", $"{data.Length}");
			return File(data, "application/octet-stream", fileInfo.Name, true);
		}

		/// <summary>
		/// Create a single directory
		/// </summary>
		/// <returns></returns>
		[HttpPost("files")]
		public async Task<IActionResult> Post_File([FromForm] UploadFileModel input)
		{
			var root = Path.Combine(_engine.PersistentDirectory, input.Path);
			if (!System.IO.Directory.Exists(root))
				throw new Exception("Directory does not exist!");
			var target = Path.Combine(root, input.File.FileName);
			if (System.IO.Directory.Exists(target))
				throw new Exception("File already exists!");

			var data = input.File.OpenReadStream();
			data.Position = 0;
			using (var str = System.IO.File.OpenWrite(target))
				await data.CopyToAsync(str);

			return Ok();
		}

		/// <summary>
		/// Delete a single file
		/// </summary>
		/// <returns></returns>
		[HttpDelete("files")]
		public async Task<IActionResult> Delete_File([FromHeader] DeleteFileModel input)
		{
			var target = Path.Combine(_engine.PersistentDirectory, input.Path);
			if (!System.IO.File.Exists(target))
				throw new Exception("File does not exist!");
			var fileInfo = new FileInfo(target);
			System.IO.File.Delete(target);
			return Ok();
		}

		/// <summary>
		/// Create a single directory
		/// </summary>
		/// <returns></returns>
		[HttpPost("directory")]
		public async Task<IActionResult> Post_Directory([FromBody] CreatePathModel input)
		{
			var root = Path.Combine(_engine.PersistentDirectory, input.Path);
			if (!System.IO.Directory.Exists(root))
				throw new Exception("Directory does not exist!");
			var target = Path.Combine(root, input.Name);
			if (System.IO.Directory.Exists(target))
				throw new Exception("Directory already exists!");
			Directory.CreateDirectory(target);
			return Ok();
		}

		/// <summary>
		/// Delete a single directory
		/// </summary>
		/// <returns></returns>
		[HttpDelete("directory")]
		public async Task<IActionResult> Delete_Directory([FromHeader] DeleteDirectoryModel input)
		{
			var target = Path.Combine(_engine.PersistentDirectory, input.Path);
			if (!System.IO.Directory.Exists(target))
				throw new Exception("Directory does not exist!");
			var fileInfo = new FileInfo(target);
			DirectoryHelper.DeleteDirectory(target);
			return Ok();
		}
	}
}
