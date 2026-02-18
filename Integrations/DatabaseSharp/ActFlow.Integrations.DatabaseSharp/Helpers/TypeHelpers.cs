namespace ActFlow.Integrations.DatabaseSharp.Helpers
{
	internal static class TypeHelpers
	{
		public static Type ByName(string name)
		{
			var asm = AppDomain.CurrentDomain.GetAssemblies().Reverse();
			foreach (var assembly in asm)
			{
				var type = assembly.GetType(name);
				if (type != null)
					return type;
			}
			throw new Exception($"Could not find the requested type '{name}'");
		}
	}
}
