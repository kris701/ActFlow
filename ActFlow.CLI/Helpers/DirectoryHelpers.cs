namespace ActFlow.CLI.Helpers
{
	public static class DirectoryHelpers
	{
		public static void DeleteDirectory(string dir)
		{
			var dirInfo = new DirectoryInfo(dir);
			try
			{
				SetAttributesNormal(dirInfo);
				Directory.Delete(dir, true);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error: {e.Message}");
			}
		}

		private static void SetAttributesNormal(DirectoryInfo dir)
		{
			foreach (var subDir in dir.GetDirectories())
				SetAttributesNormal(subDir);
			foreach (var file in dir.GetFiles())
			{
				file.Attributes = FileAttributes.Normal;
			}
			dir.Attributes = FileAttributes.Normal;
		}
	}
}
