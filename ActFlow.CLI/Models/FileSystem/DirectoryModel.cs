namespace ActFlow.CLI.Models.FileSystem
{
	public class DirectoryModel
	{
		public string Path { get; set; }
		public string Name { get; set; }

		public List<DirectoryModel> Directories { get; set; }
		public List<FilesModel> Files { get; set; }
	}
}
