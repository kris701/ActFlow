import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';
import { RouterModule } from '@angular/router';
import { TuiScrollbar, TuiTitle } from "@taiga-ui/core";
import { TuiProgress } from '@taiga-ui/kit';
import { TuiCardMedium, TuiNavigation } from '@taiga-ui/layout';
import { WorkflowStateService } from '../pages/wor/services/wor.stateservice';
import { AppSideBar } from "./app.sidebar";
import { AppTopBar } from "./app.topbar";

@Component({
    selector: 'app-layout',
    standalone: true,
    imports: [CommonModule, RouterModule, AppTopBar, AppSideBar, TuiNavigation, TuiScrollbar, TuiProgress, TuiCardMedium, TuiTitle],
    template: `
    <div class="flex flex-col h-full">
      <app-topbar></app-topbar>
      <div class="flex flex-row h-full" style="overflow:hidden">
        <app-sidebar></app-sidebar>
		<main tuiNavigationMain style="border:0px">
			<tui-scrollbar class="box">
				<router-outlet></router-outlet>
			</tui-scrollbar>
		</main>
      </div>
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
    </div>
    `
})
export class AppLayout {
    cachesLoaded = signal<boolean>(false);
    cacheLoadStage = signal<number>(0);

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
