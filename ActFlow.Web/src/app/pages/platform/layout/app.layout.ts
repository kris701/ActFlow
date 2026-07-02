import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CardModule } from 'primeng/card';
import { ConfirmDialog } from "primeng/confirmdialog";
import { ProgressBarModule } from 'primeng/progressbar';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';
import { WorkflowStateService } from '../pages/wor/services/wor.stateservice';
import { AppFooter } from './app.footer';
import { AppSidebar } from './app.sidebar';
import { AppTopbar } from './app.topbar';

@Component({
    selector: 'app-layout',
    standalone: true,
    imports: [CommonModule, AppTopbar, AppSidebar, RouterModule, AppFooter, TagModule, TooltipModule, ConfirmDialog, CardModule, ProgressBarModule],
    template: `
    <div class="layout-wrapper">
        <div class="flex flex-col w-full h-full">
            <app-topbar></app-topbar>
            <div class="flex flex-row w-full h-full" style="overflow:hidden">
                <app-sidebar></app-sidebar>
                <div class="layout-main-container">
                    <div class="layout-main">
                        <router-outlet></router-outlet>
                    </div>
                    <app-footer></app-footer>
                </div>
            </div>
        </div>
        <p-confirmdialog />
        @if(!cachesLoaded()){
            <p-card header="Loading Caches..." [style]="{'position':'fixed', 'bottom':'0', 'right':'0', 'margin':'10px', 'border':'1px solid #415B61'}">
                <p-progressbar [value]="cacheLoadStage()" />
            </p-card>
        }
    </div>
    `,
    styles: `
    .layout-wrapper {
        display: flex;
        flex-direction: column;
        height:100vh;
        min-height: 100vh;
        max-height: 100vh;
    }

    .layout-main-container {
        display: flex;
        flex-direction: column;
        min-height: inherit;
        max-height: inherit;
        overflow:auto;
        width:100%;
        justify-content: space-between;
        padding: 2rem 2rem 0 2rem;
    }

    .layout-main {
        flex: 1 1 auto;
        padding-bottom: 2rem;
        display: flex;
        flex-direction: column;
    }
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
