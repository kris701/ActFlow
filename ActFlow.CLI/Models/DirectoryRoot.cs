using System;
using System.Collections.Generic;
using System.Text;

namespace ActFlow.CLI.Models
{
	public class DirectoryRoot
	{
		public List<DirectoryModel> Directories { get; set; }
		public List<FilesModel> Files { get; set; }
	}
}
