import { CommonModule } from "@angular/common";
import { Component, Input, signal } from "@angular/core";
import { TuiButton } from "@taiga-ui/core";
import { FloatTable, FloatTableSort } from "../floattable";

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
    @Input() tuiThSortable: string | undefined = undefined;

	table : FloatTable;
	state : 'asc' | 'desc' | 'none' = 'none';
	icon = signal<string | null | undefined>("text-align-justify");

	constructor(table : FloatTable){
		this.table = table;
	}

	sort(){
		if(this.tuiThSortable){
			switch(this.state){
				case 'none':
					this.state = 'asc';
					this.table.setSort({ column: this.tuiThSortable, state: this.state } as FloatTableSort)
					this.table.applyFilter();
					this.icon.set('arrow-up-wide-narrow');
					break;
				case 'asc':
					this.state = 'desc';
					this.table.setSort({ column: this.tuiThSortable, state: this.state } as FloatTableSort)
					this.table.applyFilter();
					this.icon.set('arrow-down-wide-narrow');
					break;
				case 'desc':
					this.state = 'none';
					this.table.setSort({ column: this.tuiThSortable, state: this.state } as FloatTableSort)
					this.table.applyFilter();
					this.icon.set('text-align-justify');
					break;
			}
		}
	}
}
