import { CommonModule } from "@angular/common";
import { Component, Input, signal } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { TuiDay, TuiTime } from "@taiga-ui/cdk/date-time";
import { TuiButton, TuiDropdown, TuiInput } from "@taiga-ui/core";
import { TuiBadgeNotification, TuiBadgedContent, TuiButtonSelect, TuiDataListWrapper, TuiInputDateTime, TuiStringifyContentPipe } from "@taiga-ui/kit";
import { FloatTable, FloatTableFilter } from "../floattable";

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
							[(ngModel)]="value"
						/>
						<tui-calendar *tuiDropdown />
					</tui-textfield>
					<button
						tuiButton size="s"
						iconStart="funnel"
						tuiButton
						(click)="filterType.action(value)"
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

	value : [TuiDay, TuiTime] = [TuiDay.currentLocal(), TuiTime.currentLocal()];
	filterType : any;
	filterTypes : any[];

	protected readonly stringify = (item: any): string => `${item.label}`;

	constructor(table : FloatTable){
		this.table = table;

		this.filterTypes = [
			{
				label: 'After',
				action: (date : [TuiDay, TuiTime]) => this.applyFilter((i) => {
					var normal = date[0].toLocalNativeDate()
					normal.setMilliseconds(date[1].toAbsoluteMilliseconds());
					return i.getTime() > normal.getTime()
				})
			},
			{
				label: 'Before',
				action: (date : [TuiDay, TuiTime]) => this.applyFilter((i) => {
					var normal = date[0].toLocalNativeDate()
					normal.setMilliseconds(date[1].toAbsoluteMilliseconds());
					return i.getTime() < normal.getTime()
				})
			}
		];
		this.filterType = this.filterTypes[0];
	}

	applyFilter(fn : (i : Date) => boolean){
		this.table.setFilter({
			column: this.tuiThDateFilter,
			type: 'date',
			value: this.value,
			applied: true,
			filter: fn
		} as FloatTableFilter);
		this.table.applyFilter();
		this.filterVisible.set(false);
		this.filterApplied.set(true);
	}

	clearFilter(){
		this.table.setFilter({
			column: this.tuiThDateFilter,
			type: 'date',
			value: '',
			applied: false
		} as FloatTableFilter);
		this.table.applyFilter();
		this.filterVisible.set(false);
		this.filterApplied.set(false);
	}
}
