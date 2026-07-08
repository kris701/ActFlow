import { FloatTableSortFilterPreset } from "./FloatTableSortFilterPreset";

export interface FloatTableSortFilterPresetSave {
	current : string | null;
	presets : FloatTableSortFilterPreset[];
}
