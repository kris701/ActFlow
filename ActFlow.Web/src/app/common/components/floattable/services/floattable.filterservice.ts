import { FilterHelpers } from "../helpers/floattable.filterhelpers";
import { FloatTableFilter } from "../models/FloatTableFilter";
import { FloatTableFilterMethod } from "../models/FloatTableFilterMethod";
import { FloatTableSort } from "../models/FloatTableSort";
import { FloatTableSortMethod } from "../models/FloatTableSortMethod";

export class FloatTableFilterService {
	public sortMethods : FloatTableSortMethod[] = [
		{
			state: 'asc',
			sort: (a: any, b: any, column : string) => {
				if (a[column] < b[column])
					return 1;
				if (a[column] > b[column])
					return -1;
				return 0;
			}
		} as FloatTableSortMethod,
		{
			state: 'desc',
			sort: (a: any, b: any, column : string) => {
				if (a[column] < b[column])
					return -1;
				if (a[column] > b[column])
					return 1;
				return 0;
			}
		} as FloatTableSortMethod
	]

	public filterMethods : FloatTableFilterMethod[] = [
		// Text filters
		{
			key: 'str',
			action: 'con',
			filter: (values : any[], column : string, value : any) => FilterHelpers.textFilter(values, (i : string) => i.includes(value), column)
		},
		{
			key: 'str',
			action: 'ncon',
			filter: (values : any[], column : string, value : any) => FilterHelpers.textFilter(values, (i : string) => !i.includes(value), column)
		},
		{
			key: 'str',
			action: 'sta',
			filter: (values : any[], column : string, value : any) => FilterHelpers.textFilter(values, (i : string) => i.startsWith(value), column)
		},
		{
			key: 'str',
			action: 'end',
			filter: (values : any[], column : string, value : any) => FilterHelpers.textFilter(values, (i : string) => i.endsWith(value), column)
		},

		// Select filters
		{
			key: 'sel',
			action: 'con',
			filter: (values : any[], column : string, value : any) => FilterHelpers.textFilter(values, (i : string) => value.includes(i), column)
		},
		{
			key: 'sel',
			action: 'ncon',
			filter: (values : any[], column : string, value : any) => FilterHelpers.textFilter(values, (i : string) => !value.includes(i), column)
		},

		// Date filters
		{
			key: 'dat',
			action: 'bef',
			filter: (values : any[], column : string, value : any) =>
				FilterHelpers.dateFilter(
					values,
					(i : Date) => {
						var normal = value[0].toLocalNativeDate()
						normal.setMilliseconds(value[1].toAbsoluteMilliseconds());
						return i.getTime() < normal.getTime()
					},
					column)
		},
		{
			key: 'dat',
			action: 'aft',
			filter: (values : any[], column : string, value : any) =>
				FilterHelpers.dateFilter(
					values,
					(i : Date) => {
						var normal = value[0].toLocalNativeDate()
						normal.setMilliseconds(value[1].toAbsoluteMilliseconds());
						return i.getTime() > normal.getTime()
					},
					column)
		},
	]

	public sort(values : any[], sort : FloatTableSort) : any[]{
		var target = this.sortMethods.find(x => x.state == sort.state);
		if (target)
			return values.sort((a : any, b: any) => target!.sort(a,b,sort.column));
		return values;
	}

	public filter(values : any[], filter : FloatTableFilter) : any[]{
		var split = filter.expression.split(';');
		var target = this.filterMethods.find(x => split[0] == x.key && split[1] == x.action);
		if (target)
			return target!.filter(values, filter.column, filter.value);
		return values;
	}
}

