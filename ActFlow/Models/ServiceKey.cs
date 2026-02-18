namespace ActFlow.Models
{
	internal class ServiceKey
	{
		public string ExecutorID { get; set; }
		public string TypeName { get; set; }

		public ServiceKey(string executorID, string typeName)
		{
			ExecutorID = executorID;
			TypeName = typeName;
		}

		public override bool Equals(object? obj)
		{
			return obj is ServiceKey key &&
				   ExecutorID == key.ExecutorID &&
				   TypeName == key.TypeName;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(ExecutorID, TypeName);
		}

		public override string? ToString()
		{
			return $"({TypeName}) {ExecutorID}";
		}
	}
}
