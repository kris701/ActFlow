import { CommonModule } from "@angular/common";
import { Component, Input, signal } from "@angular/core";
import { TuiButton } from "@taiga-ui/core";
import { FloatTable } from "../floattable";
import { FloatTableSort } from "../models/FloatTableSort";

@Component({
    selector: 'tuiThSortable',
    imports: [CommonModule, TuiButton],
    template: `
		@if(tuiThSortable){
			<button tuiButton style="opacity:0.72" [iconStart]="icon()" size="s" appearance="flat-grayscale" (click)="sort()"></button>
		}
    `
})
export class TableSortableColumn {
    @Input() tuiThSortable!: string;

	table : FloatTable;
	state : 'asc' | 'desc' | 'none' = 'none';
	icon = signal<string | null | undefined>("text-align-justify");

	constructor(table : FloatTable){
		this.table = table;

		this.table.onPresetChange.subscribe(x => {
			if (!x)
			{
				this.state = 'none'
				this.icon.set('text-align-justify');
				return;
			}
			var sort = x.sorts.find(x => x.column == this.tuiThSortable);
			if (!sort)
			{
				this.state = 'none'
				this.icon.set('text-align-justify');
				return;
			}
			this.state = sort.state;

			switch(sort.state){
				case 'none':
					this.icon.set('text-align-justify');
					break;
				case 'asc':
					this.icon.set('arrow-up-wide-narrow');
					break;
				case 'desc':
					this.icon.set('arrow-down-wide-narrow');
					break;
			}
		})
	}

	sort(){
		switch(this.state){
			case 'none':
				this.state = 'asc';
				this.table.setSort({ column: this.tuiThSortable, state: this.state } as FloatTableSort)
				this.icon.set('arrow-up-wide-narrow');
				break;
			case 'asc':
				this.state = 'desc';
				this.table.setSort({ column: this.tuiThSortable, state: this.state } as FloatTableSort)
				this.icon.set('arrow-down-wide-narrow');
				break;
			case 'desc':
				this.state = 'none';
				this.table.setSort({ column: this.tuiThSortable, state: this.state } as FloatTableSort)
				this.icon.set('text-align-justify');
				break;
		}
	}
}
