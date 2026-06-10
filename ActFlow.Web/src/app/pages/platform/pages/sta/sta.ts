import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { firstValueFrom } from 'rxjs';

@Component({
    selector: 'app-sta',
    imports: [
    CommonModule
],
    template: `
    <div class="flex flex-col gap-2">
        <div class="flex flex-row gap-2 w-full">
            <div class="card flex flex-col gap-2 h-full w-full" style="align-items: center;justify-content: center;">
                <span class="status-largenumber">{{status.activeWorkflows}}</span>
                <b>Active Workflows</b>
            </div>
            <div class="card flex flex-col gap-2 h-full w-full" style="align-items: center;justify-content: center;">
                <span class="status-largenumber">{{status.archivedWorkflows}}</span>
                <b>Archived Workflows</b>
            </div>
        </div>

        <div class="flex flex-row gap-2 w-full">
            <div class="card flex flex-col gap-2 h-full w-full" style="align-items: center;justify-content: center;">
                <span class="status-largenumber">{{status.totalRuntime}}</span>
                <b>Total Runtime</b>
            </div>
            <div class="card flex flex-col gap-2 h-full w-full" style="align-items: center;justify-content: center;">
                <span class="status-largenumber">{{status.oldestRun}}</span>
                <b>Oldest Run</b>
            </div>
            <div class="card flex flex-col gap-2 h-full w-full" style="align-items: center;justify-content: center;">
                <span class="status-largenumber">{{status.latestRun}}</span>
                <b>Newest Run</b>
            </div>
        </div>
    </div>
    `,
    host:{
        class: 'flex flex-col flex-grow'
    },
    styles: `
        .status-largenumber {
            font-size:40px;
        }
    `
})
export class Status {
    status : StatusModel = {} as StatusModel;

    constructor(private http : HttpClient){}

    async ngOnInit(){
        await this.loadStatus();
    }

    async loadStatus(){
        this.status = await firstValueFrom(this.http.get<StatusModel>("/api/status"))
    }
}

interface StatusModel {
    activeWorkflows : number;
    archivedWorkflows : number;

    totalRuntime : number;
    oldestRun : Date;
    latestRun : Date;
}
