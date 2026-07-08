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

					<tui-textfield (keydown.enter)="applyFilter(filterType.expression)">
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
						(click)="applyFilter(filterType.expression)"
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
    @Input() tuiThDateFilter!: string;

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
				expression: 'dat;aft'
			},
			{
				label: 'Before',
				expression: 'dat;bef'
			}
		];
		this.filterType = this.filterTypes[0];

		this.table.onPresetChange.subscribe(x => {
			if (!x)
			{
				this.filterType = this.filterTypes[0];
				this.value = [TuiDay.currentLocal(), TuiTime.currentLocal()];
				this.filterApplied.set(false);
				return;
			}
			var filter = x.filters.find(x => x.column == this.tuiThDateFilter);
			if (!filter)
			{
				this.filterType = this.filterTypes[0];
				this.value = [TuiDay.currentLocal(), TuiTime.currentLocal()];
				this.filterApplied.set(false);
				return;
			}
			var type = this.filterTypes.find(x => x.expression == filter!.expression);
			if (!type)
			{
				this.filterType = this.filterTypes[0];
				this.value = [TuiDay.currentLocal(), TuiTime.currentLocal()];
				this.filterApplied.set(false);
				return;
			}

			this.filterType = type;
			this.value = filter.value;
			this.filterApplied.set(true);
		})
	}

	applyFilter(expression : string){
		this.table.setFilter({
			column: this.tuiThDateFilter,
			value: this.value,
			expression: expression,
		} as FloatTableFilter);
		this.filterVisible.set(false);
		this.filterApplied.set(true);
	}

	clearFilter(){
		this.table.clearFilter(this.tuiThDateFilter);
		this.filterVisible.set(false);
		this.filterApplied.set(false);
	}
}
