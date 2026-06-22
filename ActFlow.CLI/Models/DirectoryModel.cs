using System;
using System.Collections.Generic;
using System.Text;

namespace ActFlow.CLI.Models
{
	public class DirectoryModel
	{
		public string Path { get; set; }
		public string Name { get; set; }

		public List<DirectoryModel> Directories { get; set; }
		public List<FilesModel> Files { get; set; }
	}
}
