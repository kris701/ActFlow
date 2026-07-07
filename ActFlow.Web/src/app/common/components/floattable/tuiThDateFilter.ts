import { CommonModule } from "@angular/common";
import { Component, Input, signal } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { TuiDay, TuiTime } from "@taiga-ui/cdk/date-time";
import { TuiButton, TuiDropdown, TuiInput } from "@taiga-ui/core";
import { TuiBadgeNotification, TuiBadgedContent, TuiButtonSelect, TuiDataListWrapper, TuiInputDateTime, TuiStringifyContentPipe } from "@taiga-ui/kit";
import { FloatTable } from "../floattable";

@Component({
    selector: 'tuiThDateFilter',
    imports: [CommonModule, FormsModule, TuiDropdown, TuiDataListWrapper, TuiButton, TuiButtonSelect, TuiStringifyContentPipe, TuiBadgeNotification, TuiBadgedContent, TuiInput, TuiInputDateTime],
    template: `
		@if(tuiThDateFilter){
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

					<tui-textfield>
						<label tuiLabel>Choose a date</label>
						<input
							tuiInputDateTime
							[(ngModel)]="filterType.value"
						/>
						<tui-calendar *tuiDropdown />
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
export class TableDateFilter {
    @Input() tuiThDateFilter: string | undefined = undefined;

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
				label: 'After',
				value : [TuiDay.currentLocal(), TuiTime.currentLocal()],
				action: (date : [TuiDay, TuiTime]) => this.dateFilter((i) => {
					var normal = date[0].toLocalNativeDate()
					normal.setMilliseconds(date[1].toAbsoluteMilliseconds());
					return i.getTime() > normal.getTime()
				})
			},
			{
				label: 'Before',
				value : [TuiDay.currentLocal(), TuiTime.currentLocal()],
				action: (date : [TuiDay, TuiTime]) => this.dateFilter((i) => {
					var normal = date[0].toLocalNativeDate()
					normal.setMilliseconds(date[1].toAbsoluteMilliseconds());
					return i.getTime() < normal.getTime()
				})
			}
		];
		this.filterType = this.filterTypes[0];
	}

	dateFilter(fn : (i : Date) => boolean){
		var values = [...this.table.values];
		var filtered = []
		for(let value of values)
		{
			var asDate : Date = new Date(value[this.tuiThDateFilter as string]);
			if (fn(asDate))
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
