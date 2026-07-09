import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, Output, signal, SimpleChanges } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TuiButton, TuiGroup, TuiHint, TuiInput, TuiScrollbar } from "@taiga-ui/core";
import { TuiBadge, TuiStatus } from '@taiga-ui/kit';
import { v4 as uuidv4 } from 'uuid';
import { FloatTable } from './floattable';
import { FloatTableSortFilterPreset } from './models/FloatTableSortFilterPreset';
import { FloatTableSortFilterPresetSave } from './models/FloatTableSortFilterPresetSave';

@Component({
    selector: 'app-floattable-presets',
    imports: [FormsModule, CommonModule, TuiScrollbar, TuiButton, TuiGroup, TuiBadge, TuiStatus, TuiInput, TuiHint],
    template: `
		<button tuiButton iconStart="plus" size="s" appearance="secondary" (click)="createPreset()" tuiHint="Create new preset from current filters"></button>

		<tui-scrollbar class="w-full">
			@let current = currentPreset();
			@for(preset of presets(); track preset.id){
				<div tuiGroup [collapsed]="true" [rounded]="true">
					@let isActive = current?.id == preset.id;
					@if(isActive){
						@if(preset.edit){
							<tui-textfield style="width:10rem" tuiTextfieldSize="s" (keydown.enter)="preset.edit = false;saveCurrentPreset()">
								<input tuiInput [(ngModel)]="preset.name"/>
							</tui-textfield>
							<button style="flex: 0 0 auto;" tuiButton iconStart="save" size="s" appearance="info" (click)="preset.edit = false;saveCurrentPreset()" tuiHint="Save"></button>
						}
						@else {
							<div class="h-full" appearance="positive" tuiBadge tuiStatus>
								{{preset.name}}
							</div>
							<button style="flex: 0 0 auto;" tuiButton iconStart="square-pen" size="s" appearance="info" (click)="preset.edit = true" tuiHint="Edit"></button>
						}
					<button style="flex: 0 0 auto;" tuiButton iconStart="x" size="s" appearance="negative" (click)="removePreset(preset.id)" tuiHint="Delete"></button>
					}
					@else {
						<button style="flex: 0 0 auto;" tuiButton appearance="secondary" size="s"  (click)="selectPreset(preset.id)">{{preset.name}}</button>
					}
				</div>
			}
		</tui-scrollbar>
    `,
    host: {
		class:'flex flex-row gap-2 p-2',
		style:"padding-bottom:0px;padding-top:0px"
    },
    styles: `
		::ng-deep tui-scrollbar {
			> .t-content {
				display:flex;
				gap:10px;
				width:0px;
			}
		}
    `
})
export class FloatTablePresets implements OnChanges {
	@Input() storageKey: string | null = null;
	currentPreset = signal<FloatTableSortFilterPreset | null>(null);
	presets = signal<FloatTableSortFilterPreset[]>([])

	constructor(private table : FloatTable){

	}

	async ngOnChanges(changes: SimpleChanges) {
		if (changes['storageKey'] && changes['storageKey'].previousValue != changes['storageKey'].currentValue) {
			this.storageKey = changes['storageKey'].currentValue;
			if (this.storageKey){
				var item = localStorage.getItem(this.storageKey)
				if (item){
					var parsed : FloatTableSortFilterPresetSave = JSON.parse(item)
					if (parsed){
						this.presets.set(parsed.presets);
						if (parsed.current){
							var target = (parsed.presets).find(x => x.id == parsed.current);
							if (target)
								this.selectPreset(target.id)
						}
					}
				}
			}
		}
	}

	ngOnInit(){
		setTimeout(
			() => {
				var current = this.currentPreset();
				if (current)
					this.onPresetChange.emit(current);
			},
			500
		)
	}

	@Output() onPresetChange: EventEmitter<FloatTableSortFilterPreset | null> = new EventEmitter<FloatTableSortFilterPreset | null>();

	saveCurrentPreset(){
		var currentPreset = this.currentPreset();
		if (currentPreset){
			currentPreset.sorts = [...this.table.sorts()];
			currentPreset.filters = [...this.table.filters()];
			this.currentPreset.set(currentPreset);

			var presets = this.presets();
			var target = this.presets().findIndex(x => x.id == currentPreset?.id)
			if (target != -1)
				presets[target] = currentPreset;
			this.presets.set(presets);

			this.savePresets();
		}
	}

	selectPreset(id : string){
		var presets = this.presets();
		var target = presets.findIndex(x => x.id == id);
		if (target != -1)
		{
			var preset = presets[target];
			this.currentPreset.set(preset);
			this.table.sorts.set(preset.sorts);
			this.table.filters.set(preset.filters);
			this.onPresetChange.emit(preset);
			this.table.applyFilter();
		}

		this.savePresets();
	}

	createPreset(){
		this.saveCurrentPreset();

		var preset = {
			id: uuidv4(),
			name: 'New Preset',
			sorts : [...this.table.sorts()],
			filters: [...this.table.filters()]
		} as FloatTableSortFilterPreset
		this.currentPreset.set(preset);

		var presets = this.presets();
		presets.push(preset);
		this.presets.set(presets);

		this.savePresets();
	}

	removePreset(id : string){
		var presets = this.presets();
		var target = presets.findIndex(x => x.id == id);
		if (target != -1)
			presets.splice(target, 1);
		this.presets.set(presets);

		var current = this.currentPreset();
		if (current?.id == id){
			if (presets.length > 0)
			{
				this.currentPreset.set(presets[0]);
				this.onPresetChange.emit(presets[0]);
			}
			else
			{
				this.currentPreset.set(null);
				this.onPresetChange.emit(null);
			}

		}

		this.savePresets();
	}

	savePresets(){
		if(this.storageKey){
			var saveBody = {
				current: this.currentPreset()?.id,
				presets: [...this.presets()]
			} as FloatTableSortFilterPresetSave
			localStorage.setItem(
				this.storageKey,
				JSON.stringify(saveBody));
		}
	}

	clearFilters(){
		this.currentPreset.set(null);
		this.onPresetChange.emit(null);
		this.table.sorts.set([])
		this.table.filters.set([])
		this.savePresets();
		this.table.applyFilter();
	}
}

