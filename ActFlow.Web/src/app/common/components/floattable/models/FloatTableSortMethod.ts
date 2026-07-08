export interface FloatTableSortMethod {
	state : 'asc' | 'desc' | 'none';
	sort(a : any, b : any, column : string) : number;
}
