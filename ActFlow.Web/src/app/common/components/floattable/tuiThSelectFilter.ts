import { CommonModule } from "@angular/common";
import { Component, Input, signal } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { TuiButton, TuiDataList, TuiDropdown, TuiInput } from "@taiga-ui/core";
import { TuiBadgeNotification, TuiBadgedContent, TuiButtonSelect, TuiDataListWrapper, TuiMultiSelect, TuiStringifyContentPipe } from "@taiga-ui/kit";
import { FloatTable } from "../floattable";

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
				<div class="flex flex-col gap-2 p-4" style="min-width:10rem">
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
						[(ngModel)]="filterType.selected"
					>
						{{filterType.selected.length === 1 ? filterType.selected[0] : 'Selected ' + filterType.selected.length}}

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
						(click)="filterType.action(filterType.selected)"
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
    @Input() tuiThSelectFilter: string | undefined = undefined;

	filterApplied = signal<boolean>(false);
	filterVisible = signal<boolean>(false);

	table : FloatTable;

	@Input() options : string[] = [];

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
				selected : [],
				action: (selected : string[]) => this.listFilter((i) => selected.includes(i))
			},
			{
				label: 'Not Contains',
				selected : [],
				action: (selected : string[]) => this.listFilter((i) => !selected.includes(i))
			}
		];
		this.filterType = this.filterTypes[0];
	}

	listFilter(fn : (i : string) => boolean){
		var values = [...this.table.values];
		var filtered = []
		for(let value of values)
		{
			var asStr : string = value[this.tuiThSelectFilter as string];
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
