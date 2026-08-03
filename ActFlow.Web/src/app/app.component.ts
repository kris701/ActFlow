import { Component, signal } from '@angular/core';
import { EzUILayout, MenuItem } from '@kris701/ez-ui';
import { TuiTitle } from '@taiga-ui/core';
import { TuiProgress } from "@taiga-ui/kit";
import { TuiCardMedium } from '@taiga-ui/layout';
import { WorkflowStateService } from './pages/wor/services/wor.stateservice';

@Component({
    selector: 'app-root',
    standalone: true,
    imports: [EzUILayout, TuiProgress, TuiCardMedium, TuiTitle],
	providers: [WorkflowStateService],
    template: `
		<ezui-layout [sidebarItems]="sidebarItems" [sidebarFooterItems]="sidebarFooterItems">
			<ng-template #topbarlogo>
				<img src="logo.png" [style]="{ height: '45px' }" />
				<span>ActFlow</span>
			</ng-template>
		</ezui-layout>

        @if(!cachesLoaded()){
            <div tuiCardMedium [style]="{'position':'fixed', 'bottom':'0', 'right':'0', 'margin':'10px', 'border':'1px solid #415B61'}">
				<h2 tuiTitle>
					Loading Caches...
				</h2>
				<tui-progress-circle
					size="s"
					[max]="100"
					[value]="cacheLoadStage()"
				/>
            </div>
        }
    `
})
export class AppComponent {
	cachesLoaded = signal<boolean>(false);
    cacheLoadStage = signal<number>(0);

	sidebarItems = signal<MenuItem[]>([
		{
			label: 'Status',
			icon: 'info',
			routerLink: '/',
			visible: true
		} as MenuItem,
		{
			label: 'Workflows',
			icon: 'chart-no-axes-combined',
			visible: true,
			items: [
				{
					label:'Execute',
					icon:'circle-play',
					routerLink: 'workflows/run',
					visible: true
				},
				{
					label:'Results',
					icon:'table',
					routerLink: 'workflows/results',
					visible: true
				}
			]
		} as MenuItem,
		{
			label: 'Files',
			icon:'file',
			visible: true,
			items: [
				{
					label:'Persistent',
					routerLink: 'files/persistent',
					visible: true
				},
				{
					label:'Temporary',
					routerLink: 'files/temporary',
					visible: true
				}
			]
		} as MenuItem
	])

	sidebarFooterItems = signal<MenuItem[]>([
		{
			label: 'Repo',
			icon: 'github',
			command: async () => {
				open('https://github.com/kris701/ActFlow');
			},
			visible : true
		} as MenuItem
	])

	constructor(
		public workflowStateService : WorkflowStateService
	){
	}

	async ngOnInit(){
		this.cachesLoaded.set(false);
		var loadMax = 1;
		var loaded = 1;

		await this.workflowStateService.Load();
		this.cacheLoadStage.set(Math.round((loaded++ / loadMax) * 100));

		this.cacheLoadStage.set(100);
		this.cachesLoaded.set(true);
	}
}
