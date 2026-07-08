export class FilterHelpers {
	public static textFilter(values: any[], fn : (i : string) => boolean, column : string) : any[]{
		var filtered = []
		for(let value of values)
		{
			var asStr : string = value[column];
			if (fn(asStr))
				filtered.push(value);
		}
		return filtered;
	}

	public static dateFilter(values: any[], fn : (i : Date) => boolean, column : string) : any[]{
		var filtered = []
		for(let value of values)
		{
			var asDate : Date = new Date(value[column]);
			if (fn(asDate))
				filtered.push(value);
		}
		return filtered;
	}
}
