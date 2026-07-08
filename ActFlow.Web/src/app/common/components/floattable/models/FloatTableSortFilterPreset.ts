import { FloatTableFilter } from "./FloatTableFilter";
import { FloatTableSort } from "./FloatTableSort";

export interface FloatTableSortFilterPreset {
	id : string;
	name : string;
	sorts : FloatTableSort[];
	filters : FloatTableFilter[];
	edit: boolean;
}
