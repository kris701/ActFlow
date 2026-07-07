import { CommonModule } from "@angular/common";
import { Component, Input, signal } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { TuiButton, TuiDropdown, TuiInput } from "@taiga-ui/core";
import { TuiBadgeNotification, TuiBadgedContent, TuiButtonSelect, TuiDataListWrapper, TuiStringifyContentPipe } from "@taiga-ui/kit";
import { FloatTable } from "../floattable";

@Component({
    selector: 'tuiThTextFilter',
    imports: [CommonModule, FormsModule, TuiDropdown, TuiDataListWrapper, TuiButton, TuiButtonSelect, TuiStringifyContentPipe, TuiBadgeNotification, TuiBadgedContent, TuiInput],
    template: `
		@if(tuiThTextFilter){
			<tui-badged-content>
				@if(filterApplied()){
					<tui-badge-notification
						size="s"
						tuiSlot="top"
					>
						1
					</tui-badge-notification>
				}
				<button
					tuiButton size="s"
					iconStart="funnel"
					appearance="flat-grayscale"
					style="opacity:0.72"
					[tuiDropdown]="filterPop"
					[(tuiDropdownOpen)]="filterVisible"
					(click)="filterVisible.set(true)"
				></button>
			</tui-badged-content>

			<ng-template #filterPop>
				<div class="flex flex-col gap-2 p-4" style="min-width:20rem">
					<button
						appearance="outline-grayscale"
						size="s"
						tuiButton
						tuiButtonSelect
						[(ngModel)]="filterType"
					>
						{{ filterType.label }}
						<tui-data-list-wrapper
							*tuiDropdown
							[itemContent]="stringify | tuiStringifyContent"
							[items]="filterTypes"
						/>
					</button>

					<tui-textfield tuiTextfieldSize="s">
						<input tuiInput [(ngModel)]="filterType.value"/>
					</tui-textfield>
					<button
						tuiButton size="s"
						iconStart="funnel"
						tuiButton
						(click)="filterType.action(filterType.value)"
					>
						Apply
					</button>

					@if(filterApplied()){
						<button
							tuiButton size="s"
							iconStart="circle-x"
							tuiButton
							(click)="clearFilter()"
						>
							Clear
						</button>
					}
				</div>
			</ng-template>
		}
    `
})
export class TableTextFilter {
    @Input() tuiThTextFilter: string | undefined = undefined;

	filterApplied = signal<boolean>(false);
	filterVisible = signal<boolean>(false);

	table : FloatTable;

	filterType : any;
	filterTypes : any[];

	protected readonly stringify = (item: any): string => `${item.label}`;

	constructor(table : FloatTable){
		this.table = table;
		this.table.onFilterApplied.subscribe(x => {
			this.filterApplied.set(false);
		});

		this.filterTypes = [
			{
				label: 'Contains',
				value : '',
				action: (str : string) => this.stringFilter((i) => i.includes(str))
			},
			{
				label: 'Not Contains',
				value : '',
				action: (str : string) => this.stringFilter((i) => !i.includes(str))
			},
			{
				label: 'Starts With',
				value : '',
				action: (str : string) => this.stringFilter((i) => i.startsWith(str))
			},
			{
				label: 'Ends With',
				value : '',
				action: (str : string) => this.stringFilter((i) => i.endsWith(str))
			}
		];
		this.filterType = this.filterTypes[0];
	}

	stringFilter(fn : (i : string) => boolean){
		var values = [...this.table.values];
		var filtered = []
		for(let value of values)
		{
			var asStr : string = value[this.tuiThTextFilter as string];
			if (fn(asStr))
				filtered.push(value);
		}

		this.applyFilter(filtered);
	}

	applyFilter(newValues : any[]){
		this.table.applyFilter(newValues);
		this.filterVisible.set(false);
		this.filterApplied.set(true);
	}

	clearFilter(){
		this.table.applyFilter(this.table.values);
		this.filterVisible.set(false);
		this.filterApplied.set(false);
	}
}
