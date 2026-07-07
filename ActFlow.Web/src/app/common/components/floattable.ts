import { CommonModule } from '@angular/common';
import { Component, ContentChild, EventEmitter, Input, OnChanges, Output, signal, SimpleChanges, TemplateRef } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TuiTable, TuiTablePagination } from '@taiga-ui/addon-table';
import { TuiButton, TuiDropdown, TuiIcon, TuiLoader, TuiScrollbar } from "@taiga-ui/core";
import { TuiChevron, TuiDataListWrapper } from '@taiga-ui/kit';
import { TuiBlockStatus } from '@taiga-ui/layout';

@Component({
    selector: 'app-floattable',
    imports: [FormsModule, CommonModule, TuiTable, TuiScrollbar, TuiButton, TuiChevron, TuiDropdown, TuiDataListWrapper, TuiTablePagination, TuiLoader, TuiBlockStatus, TuiIcon],
    template: `
		<div class="app-floattable">
			<tui-loader [inheritColor]="true" [overlay]="true" size="xxl" [loading]="isLoading()">
				@if(values.length == 0){
					<tui-block-status>
						<tui-icon tuiSlot="top" icon="grid-2x2-x" />

						<h3>No Data</h3>

						No data to display.
					</tui-block-status>
				}
				@else {
					@if(showAdd || showRefresh){
						<div class="flex flex-row gap-2 p-2" style="padding-bottom:0px">
							@if(showRefresh){
								<button tuiButton iconStart="rotate-cw" size="s" appearance="info" (click)="onLoadItems.emit()"></button>
							}
							@if(showAdd){
								<button tuiButton iconStart="plus" size="s" appearance="info" (click)="onAddItem.emit()"></button>
							}
						</div>
					}
					<tui-scrollbar class="w-full h-full">
						<table tuiTable class="w-full h-full">
							<thead>
								<tr>
									@if(expandable){
										<th tuiTh></th>
									}
									<ng-container [ngTemplateOutlet]="tableHeader"></ng-container>
								</tr>
							</thead>
							@for (item of displayValues(); track page() * pageSize() + i; let i = $index){
								@let fullIndex = page() * pageSize() + i;
								<tbody tuiTbody>
									<tr>
										@if(expandable){
											<td tuiTd class="app-floattable-expander">
												<button
													appearance="flat-grayscale"
													size="xs"
													tuiIconButton
													type="button"
													[tuiChevron]="state[fullIndex] ?? false"
													(click)="state[fullIndex] = !state[fullIndex];onRowExpanded.emit(item)"
												>
													Toggle
												</button>
											</td>
										}
										<ng-container [ngTemplateOutlet]="tableRows" [ngTemplateOutletContext]="{ $implicit: item  }"></ng-container>
									</tr>
								</tbody>

								<tbody tuiTableExpand [expanded]="state[fullIndex] ?? false">
									@if(state[fullIndex] ?? false){
										<tr>
											<ng-container [ngTemplateOutlet]="tableExpandedrow" [ngTemplateOutletContext]="{ $implicit: item  }"></ng-container>
										</tr>
									}
								</tbody>
							}
						</table>
					</tui-scrollbar>
					<div class="app-floattable-footer">
						<tui-table-pagination
								[(size)]="pageSize"
								[(page)]="page"
								[total]="internalValues.length"
								[items]="pageSizes"
								(pageChange)="processPage()"
								(sizeChange)="processPage()"
							/>
					</div>
				}
			</tui-loader>
		</div>
    `,
    host: {
		class:'w-full h-full'
    },
    styles: `
		.app-floattable {
			border: 2px solid var(--tui-border-normal);
			border-radius: var(--tui-radius-l);
			display:flex;
			height:100%;
			width:100%;
			overflow:hidden;

			::ng-deep tui-loader {
				display:flex;
				height:100%;
				width:100%;
				flex-direction: column;
				> .t-content {
					display:flex;
					height:100%;
					width:100%;
					flex-direction: column;
					gap:0.5rem;
				}

				> .t-loader {
					position:absolute;
					width:100%;
					height:100%;
				}
			}

			::ng-deep tui-scrollbar {
				> .t-content {
					width:0px;
				}
			}

			.app-floattable-footer {
				display:flex;
				margin-bottom:0.5rem;
				padding-left:2rem;
				padding-right:2rem;

				tui-table-pagination {
					flex: 1
				}
			}

			::ng-deep table {
				border-radius: var(--tui-radius-l);
			}

			::ng-deep th {
				background-color: var(--tui-background-base-alt) !important;
			}

			::ng-deep .app-floattable-expander {
				padding:0px;

				button {
					width:100%;
					height:100%;
					border-radius: 0px;
				}
			}

			th[tuiTh] {
				align-items:center;
			}
		}
    `
})
export class FloatTable implements OnChanges {
    @ContentChild('tableHeader', { static: false }) tableHeader: TemplateRef<any> | undefined;
    @ContentChild('tableRows', { static: false }) tableRows: TemplateRef<any> | undefined;
    @ContentChild('tableExpandedrow', { static: false }) tableExpandedrow: TemplateRef<any> | undefined;
    @ContentChild('additionalheader', { static: false }) additionalheader: TemplateRef<any> | undefined;

    @Input() disabled: boolean = false;
    @Input() isLoading = signal<boolean>(false);

    @Input() showAdd: boolean = false;
    @Input() showRefresh: boolean = false;

	@Input() expandable: boolean = false;

    @Input() values: any[] = [];
	internalValues: any[] = [];
    displayValues = signal<any[]>([]);

	sorts = signal<FloatTableSort[]>([]);
	filters = signal<FloatTableFilter[]>([]);

	async ngOnChanges(changes: SimpleChanges) {
		if (changes['values'] && changes['values'].previousValue != changes['values'].currentValue) {
			this.values = changes['values'].currentValue;
			this.applyFilter();
		}
	}

    @Output() onAddItem: EventEmitter<any> = new EventEmitter();
    @Output() onLoadItems: EventEmitter<any> = new EventEmitter();
    @Output() onRowExpanded: EventEmitter<any> = new EventEmitter();
	@Output() onLoadFilter: EventEmitter<FloatTableFilter> = new EventEmitter<FloatTableFilter>();

    @Input() pageSize = signal<number>(25);
	page = signal<number>(0);
	pages = signal<number>(0);
	readonly pageSizes = [10, 25, 50, 100, 1000];
	processPage(){
		var fromIndex = this.pageSize() * this.page();
		var toIndex = fromIndex + this.pageSize();
		this.displayValues.set(this.internalValues.slice(fromIndex, toIndex));
	}

	applySorts(){
		var sorted = [...this.internalValues]
		for(var sort of this.sorts())
			sorted = this.sort(sorted, sort);
		this.internalValues = sorted;
	}

	applyFilter(){
		var filtered = [...this.values]
		for(var filter of this.filters())
		{
			if (filter.applied)
				filtered = this.filter(filtered, filter);
		}
		this.internalValues = filtered;
		this.applySorts();
		this.state = [];
		this.page.set(0);
		this.pages.set(Math.floor(this.internalValues.length / this.pageSize()) + 1)
		this.processPage();
	}

	state: Record<number, boolean> = {};

	setSort(sort : FloatTableSort){
		var sorts = this.sorts();
		var target = sorts.findIndex(x => x.column == sort.column);
		if (target != -1)
			sorts[target] = sort;
		else
			sorts.push(sort);
		this.sorts.set(sorts);
	}

	setFilter(filter : FloatTableFilter){
		var filters = this.filters();
		var target = filters.findIndex(x => x.column == filter.column);
		if (target != -1){
			filters[target] = filter;
		}
		else
			filters.push(filter);
		this.filters.set(filters);
	}

	sort(values : any[], sort : FloatTableSort) : any[]{
		switch(sort.state){
			case 'asc':
				return values.sort((a : any,b : any) => {
					if (a[sort.column] < b[sort.column])
						return 1;
					if (a[sort.column] > b[sort.column])
						return -1;
					return 0;
				});
			case 'desc':
				return values.sort((a : any,b : any) => {
					if (a[sort.column] < b[sort.column])
						return -1;
					if (a[sort.column] > b[sort.column])
						return 1;
					return 0;
				});
		}
		return values;
	}

	filter(values : any[], filter : FloatTableFilter) : any[]{
		switch(filter.type){
			case 'text':
			case 'select':
				return this.textFilter(values, filter.filter, filter.column);
			case 'date':
				return this.dateFilter(values, filter.filter, filter.column);
		}
	}

	textFilter(values: any[], fn : (i : string) => boolean, column : string) : any[]{
		var filtered = []
		for(let value of values)
		{
			var asStr : string = value[column];
			if (fn(asStr))
				filtered.push(value);
		}
		return filtered;
	}

	dateFilter(values: any[], fn : (i : Date) => boolean, column : string) : any[]{
		var filtered = []
		for(let value of values)
		{
			var asDate : Date = new Date(value[column]);
			if (fn(asDate))
				filtered.push(value);
		}
		return filtered;
	}
}

export interface FloatTableSort {
	column : string;
	state : 'asc' | 'desc' | 'none';
}

export interface FloatTableFilter {
	column : string;
	type : 'text' | 'select' | 'date';
	value : any;
	applied : boolean;
	filterIndex : number;
	filter(v : any) : boolean;
}
