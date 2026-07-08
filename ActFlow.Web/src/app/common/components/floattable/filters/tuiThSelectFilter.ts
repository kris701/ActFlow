import { CommonModule } from "@angular/common";
import { Component, Input, signal } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { TuiButton, TuiDataList, TuiDropdown, TuiInput } from "@taiga-ui/core";
import { TuiBadgeNotification, TuiBadgedContent, TuiButtonSelect, TuiDataListWrapper, TuiMultiSelect, TuiStringifyContentPipe } from "@taiga-ui/kit";
import { FloatTable } from "../floattable";
import { FloatTableFilter } from "../models/FloatTableFilter";

@Component({
    selector: 'tuiThSelectFilter',
    imports: [CommonModule, FormsModule, TuiDropdown, TuiDataListWrapper, TuiButton, TuiButtonSelect, TuiStringifyContentPipe, TuiBadgeNotification, TuiBadgedContent, TuiInput, TuiMultiSelect, TuiDataList],
    template: `
		@if(tuiThSelectFilter){
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

					<button
						appearance="secondary-grayscale"
						size="s"
						tuiButton
						tuiButtonSelect
						[(ngModel)]="selected"
					>
						{{selected.length === 1 ? selected[0] : 'Selected ' + selected.length}}

						<tui-data-list *tuiDropdown>
							<tui-opt-group
								label="Options"
								tuiMultiSelectGroup
							>
								@for (option of options; track option) {
									<button
										tuiOption
										type="button"
										[value]="option"
									>
										{{ option }}
									</button>
								}
							</tui-opt-group>
						</tui-data-list>
					</button>

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
export class TableSelectFilter {
    @Input() tuiThSelectFilter!: string;

	filterApplied = signal<boolean>(false);
	filterVisible = signal<boolean>(false);

	table : FloatTable;

	@Input() options : string[] = [];
	selected : string[] = [];
	filterType : any;
	filterTypes : any[];

	protected readonly stringify = (item: any): string => `${item.label}`;

	constructor(table : FloatTable){
		this.table = table;

		this.filterTypes = [
			{
				label: 'Contains',
				expression: 'sel;con'
			},
			{
				label: 'Not Contains',
				expression: 'sel;ncon'
			}
		];
		this.filterType = this.filterTypes[0];

		this.table.onPresetChange.subscribe(x => {
			if (!x)
			{
				this.filterType = this.filterTypes[0];
				this.selected = [];
				this.filterApplied.set(false);
				return;
			}
			var filter = x.filters.find(x => x.column == this.tuiThSelectFilter);
			if (!filter)
			{
				this.filterType = this.filterTypes[0];
				this.selected = [];
				this.filterApplied.set(false);
				return;
			}
			var type = this.filterTypes.find(x => x.expression == filter!.expression);
			if (!type)
			{
				this.filterType = this.filterTypes[0];
				this.selected = [];
				this.filterApplied.set(false);
				return;
			}

			this.filterType = type;
			this.selected = filter.value;
			this.filterApplied.set(true);
		})
	}

	applyFilter(expression : string){
		this.table.setFilter({
			column: this.tuiThSelectFilter,
			value: this.selected,
			expression: expression,
		} as FloatTableFilter);
		this.filterVisible.set(false);
		this.filterApplied.set(true);
	}

	clearFilter(){
		this.table.clearFilter(this.tuiThSelectFilter);
		this.filterVisible.set(false);
		this.filterApplied.set(false);
	}
}
