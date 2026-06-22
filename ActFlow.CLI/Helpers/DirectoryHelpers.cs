using ActFlow.CLI.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ActFlow.CLI.Helpers
{
	public static class DirectoryHelpers
	{
		public static DirectoryRoot GetFullDirectory(string rootPath)
		{
			var src = BuildFromPath(rootPath, rootPath);
			var model = new DirectoryRoot()
			{
				Directories = src.Directories,
				Files = src.Files
			};
			return model;
		}

		private static DirectoryModel BuildFromPath(string path, string rootPath)
		{
			var dirInfo = new DirectoryInfo(path);

			var result = new DirectoryModel()
			{
				Path = RemovePrefix(path, rootPath),
				Name = dirInfo.Name,
				Files = new List<FilesModel>(),
				Directories = new List<DirectoryModel>(),
			};

			var dirPaths = Directory.GetDirectories(path);

			foreach (var dirPath in dirPaths)
			{
				var dirModel = BuildFromPath(dirPath, rootPath);
				result.Directories.Add(dirModel);
			}

			var filePaths = Directory.GetFiles(path);

			foreach (var filePath in filePaths)
			{
				var fileInfo = new FileInfo(filePath);
				result.Files.Add(new FilesModel()
				{
					Name = fileInfo.Name,
					Extension = fileInfo.Extension,
					Path = RemovePrefix(filePath, rootPath),
					SizeB = fileInfo.Length
				});
			}

			return result;
		}

		private static string RemovePrefix(string path, string rootPath)
		{
			if (path.StartsWith(rootPath))
			{
				if (path == rootPath)
					return "";
				return path.Substring(rootPath.Length + 1);
			}
			return path;
		}
	}
}
