import { CommonModule } from "@angular/common";
import { Component, Input, signal } from "@angular/core";
import { TuiButton } from "@taiga-ui/core";
import { FloatTable } from "../floattable";

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

		this.table.onSortApplied.subscribe(c => {
			if (c != this.tuiThSortable){
				this.icon.set('text-align-justify');
				this.state = 'none'
			}
		});
	}

	sort(){
		if(this.tuiThSortable){
			var values = [...this.table.internalValues];
			var sorted = []
			switch(this.state){
				case 'none':
					this.state = 'asc';
					sorted = values.sort((a : any,b : any) => {
						if (a[this.tuiThSortable as string] < b[this.tuiThSortable as string])
							return 1;
						if (a[this.tuiThSortable as string] > b[this.tuiThSortable as string])
							return -1;
						return 0;
					});
					this.table.applySort(sorted, this.tuiThSortable)
					this.icon.set('arrow-up-wide-narrow');
					break;
				case 'asc':
					this.state = 'desc';
					sorted = values.sort((a : any,b : any) => {
						if (a[this.tuiThSortable as string] < b[this.tuiThSortable as string])
							return -1;
						if (a[this.tuiThSortable as string] > b[this.tuiThSortable as string])
							return 1;
						return 0;
					});
					this.table.applySort(sorted, this.tuiThSortable)
					this.icon.set('arrow-down-wide-narrow');
					break;
				case 'desc':
					this.state = 'none';
					sorted = values;
					this.table.applySort(sorted, this.tuiThSortable)
					this.icon.set('text-align-justify');
					break;
			}
		}
	}
}
