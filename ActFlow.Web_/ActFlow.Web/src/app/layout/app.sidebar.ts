import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TuiDropdown } from '@taiga-ui/core';
import { TuiAsideComponent, TuiAsideGroupComponent, TuiNavigation } from "@taiga-ui/layout";
import { LayoutService } from './services/layoutService';

@Component({
    selector: 'app-sidebar',
    standalone: true,
    imports: [CommonModule, RouterLink, TuiNavigation, TuiAsideGroupComponent, TuiAsideComponent, TuiDropdown],
    template: `
	    <aside
	        style="height:100%"
			tuitheme=""
	        [tuiNavigationAside]="layoutService.isMenuExpanded()"
	    >
			@for(item of menuItems(); track item){
				<tui-aside-group>
					<button
						[iconStart]="item.icon"
						tuiAsideItem
						tuiChevron
						type="button"
					>
						{{item.label}}
						<ng-template>
							@for(subitem of item.items; track subitem){
								<button
									tuiAsideItem
									[iconStart]="subitem.icon"
									type="button"
								>
									{{subitem.label}}
								</button>
							}
						</ng-template>
					</button>
				</tui-aside-group>
			}
	    </aside>
    `
})
export class AppSideBar {
	menuItems = signal<MenuItem[]>([
		{
			label: 'Status',
			icon: 'info'
		} as MenuItem,
		{
			label: 'Workflows',
			icon: 'chart-no-axes-combined',
			items: [
				{
					label:'Execute',
					icon:'circle-play'
				},
				{
					label:'Results',
					icon:'table'
				}
			]
		} as MenuItem,
		{
			label: 'Files',
			icon:'file',
			items: [
				{
					label:'Persistent'
				},
				{
					label:'Temporary'
				}
			]
		} as MenuItem
	])

    constructor(
          public layoutService: LayoutService
      ){}
}

interface MenuItem {
	label: string,
	icon: string | null,
	routerLink: string | null,
	items: SubMenuItem[] | null
}

interface SubMenuItem {
	label: string,
	icon: string | null,
	routerLink: string | null
}
