import { CommonModule } from '@angular/common';
import { Component, ContentChild, EventEmitter, Input, OnChanges, Output, signal, SimpleChanges, TemplateRef } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TuiTable, TuiTablePagination } from '@taiga-ui/addon-table';
import { TuiButton, TuiDropdown, TuiGroup, TuiIcon, TuiInput, TuiLoader, TuiScrollbar } from "@taiga-ui/core";
import { TuiBadge, TuiChevron, TuiDataListWrapper, TuiStatus } from '@taiga-ui/kit';
import { TuiBlockStatus } from '@taiga-ui/layout';

@Component({
    selector: 'app-floattable',
    imports: [FormsModule, CommonModule, TuiTable, TuiScrollbar, TuiButton, TuiChevron, TuiDropdown, TuiDataListWrapper, TuiTablePagination, TuiLoader, TuiBlockStatus, TuiIcon, TuiGroup, TuiBadge, TuiStatus, TuiInput],
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
					@if(showAdd || showRefresh || showClearFilters){
						<div class="flex flex-row gap-2 p-2" style="padding-bottom:0px">
							@if(showRefresh){
								<button tuiButton iconStart="rotate-cw" size="s" appearance="info" (click)="onLoadItems.emit()"></button>
							}
							@if(showAdd){
								<button tuiButton iconStart="plus" size="s" appearance="info" (click)="onAddItem.emit()"></button>
							}
							@if(showClearFilters){
								<button tuiButton iconStart="funnel-x" size="s" appearance="info" (click)="clearFilters()"></button>
							}
						</div>
					}
					@if(allowPresets){
						<div class="flex flex-row gap-2 p-2" style="padding-bottom:0px;padding-top:0px">
							<button tuiButton iconStart="plus" size="s" appearance="secondary" (click)="createPreset()"></button>

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
												<button style="flex: 0 0 auto;" tuiButton iconStart="save" size="s" appearance="info" (click)="preset.edit = false;saveCurrentPreset()"></button>
											}
											@else {
												<div class="h-full" appearance="positive" tuiBadge tuiStatus>
													{{preset.name}}
												</div>
												<button style="flex: 0 0 auto;" tuiButton iconStart="square-pen" size="s" appearance="info" (click)="preset.edit = true"></button>
											}
										<button style="flex: 0 0 auto;" tuiButton iconStart="x" size="s" appearance="negative" (click)="removePreset(preset.id)"></button>
										}
										@else {
											<button style="flex: 0 0 auto;" tuiButton appearance="secondary" size="s"  (click)="selectPreset(preset.id)">{{preset.name}}</button>
										}
									</div>
								}
							</tui-scrollbar>
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
					overflow-x:auto;
				}

				> .t-loader {
					position:absolute;
					width:100%;
					height:100%;
				}
			}

			::ng-deep tui-scrollbar {
				> .t-content {
					display:flex;
					gap:10px;
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
    @Input() showClearFilters: boolean = false;

	@Input() expandable: boolean = false;

    @Input() values: any[] = [];
	internalValues: any[] = [];
    displayValues = signal<any[]>([]);

	sorts = signal<FloatTableSort[]>([]);
	filters = signal<FloatTableFilter[]>([]);

	@Input() storageKey: string | null = null;
	@Input() allowPresets: boolean = false;
	currentPreset = signal<FloatTableSortFilterPreset | null>(null);
	presets = signal<FloatTableSortFilterPreset[]>([])

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
		if (changes['values'] && changes['values'].previousValue != changes['values'].currentValue) {
			this.values = changes['values'].currentValue;
			this.applyFilter();
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

    @Output() onAddItem: EventEmitter<any> = new EventEmitter();
    @Output() onLoadItems: EventEmitter<any> = new EventEmitter();
    @Output() onRowExpanded: EventEmitter<any> = new EventEmitter();
	@Output() onPresetChange: EventEmitter<FloatTableSortFilterPreset | null> = new EventEmitter<FloatTableSortFilterPreset | null>();

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
			filtered = this.filter(filtered, filter);
		this.internalValues = filtered;
		this.applySorts();
		this.state = [];
		this.page.set(0);
		this.pages.set(Math.floor(this.internalValues.length / this.pageSize()) + 1)
		this.processPage();

		this.saveCurrentPreset();
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

		this.applyFilter();
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

		this.applyFilter();
	}

	clearFilter(column : string){
		var filters = this.filters();
		var target = filters.findIndex(x => x.column == column);
		if (target != -1)
			filters.splice(target, 1)
		this.filters.set(filters);

		this.applyFilter();
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
		var split = filter.expression.split(';');
		switch(split[0]){
			case 'str':
				switch(split[1]){
					case 'con':
						return this.textFilter(values, (i : string) => i.includes(filter.value), filter.column);
					case 'ncon':
						return this.textFilter(values, (i : string) => !i.includes(filter.value), filter.column);
					case 'sta':
						return this.textFilter(values, (i : string) => i.startsWith(filter.value), filter.column);
					case 'end':
						return this.textFilter(values, (i : string) => i.endsWith(filter.value), filter.column);
				}
				break;
			case 'sel':
				switch(split[1]){
					case 'con':
						return this.textFilter(values, (i : string) => filter.value.includes(i), filter.column);
					case 'ncon':
						return this.textFilter(values, (i : string) => !filter.value.includes(i), filter.column);
				}
				break;
			case 'dat':
				switch(split[1]){
					case 'bef':
						return this.dateFilter(
							values,
							(i : Date) => {
								var normal = filter.value[0].toLocalNativeDate()
								normal.setMilliseconds(filter.value[1].toAbsoluteMilliseconds());
								return i.getTime() < normal.getTime()
							},
							filter.column);
					case 'aft':
						return this.dateFilter(
							values,
							(i : Date) => {
								var normal = filter.value[0].toLocalNativeDate()
								normal.setMilliseconds(filter.value[1].toAbsoluteMilliseconds());
								return i.getTime() > normal.getTime()
							},
							filter.column);
				}
				break;
		}
		return values;
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

	saveCurrentPreset(){
		var currentPreset = this.currentPreset();
		if (currentPreset){
			currentPreset.sorts = [...this.sorts()];
			currentPreset.filters = [...this.filters()];
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
			this.sorts.set(preset.sorts);
			this.filters.set(preset.filters);
			this.onPresetChange.emit(preset);
			this.applyFilter();
		}

		this.savePresets();
	}

	createPreset(){
		this.saveCurrentPreset();

		var preset = {
			id: crypto.randomUUID(),
			name: 'New Preset',
			sorts : [...this.sorts()],
			filters: [...this.filters()]
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
		this.sorts.set([])
		this.filters.set([])
		this.savePresets();
		this.applyFilter();
	}
}

export interface FloatTableSort {
	column : string;
	state : 'asc' | 'desc' | 'none';
}

export interface FloatTableFilter {
	column : string;
	value : any;
	expression : string;
}

export interface FloatTableSortFilterPreset {
	id : string;
	name : string;
	sorts : FloatTableSort[];
	filters : FloatTableFilter[];
	edit: boolean;
}

export interface FloatTableSortFilterPresetSave {
	current : string | null;
	presets : FloatTableSortFilterPreset[];
}
