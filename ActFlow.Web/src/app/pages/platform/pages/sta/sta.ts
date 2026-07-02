import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, signal } from '@angular/core';
import 'chartjs-adapter-date-fns';
import { ButtonModule } from 'primeng/button';
import { ChartModule } from 'primeng/chart';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { firstValueFrom } from 'rxjs';

@Component({
    selector: 'app-sta',
    imports: [
    CommonModule, ChartModule, ButtonModule, ProgressSpinnerModule
],
    template: `
    <p-button icon="pi pi-refresh" (onClick)="loadStatus()" text class="status-reloadbutton"/>

    @if(isLoading()){
        <p-progress-spinner ariaLabel="loading" />
    }
    @else {
        @let stat = status();
        <div class="flex flex-col gap-2">
            <div class="flex flex-row gap-2 w-full">
                <div class="card flex flex-col gap-2 h-full w-full" style="align-items: center;justify-content: center;">
                    <span class="status-xlargenumber">{{stat.activeWorkflows}}</span>
                    <span style="opacity:0.5">Active Workflows</span>
                </div>
                <div class="card flex flex-col gap-2 h-full w-full" style="align-items: center;justify-content: center;">
                    <span class="status-xlargenumber">{{stat.archivedWorkflows}}</span>
                    <span style="opacity:0.5">Archived Workflows</span>
                </div>
            </div>

            <div class="flex flex-row gap-2 w-full">
                <div class="card flex flex-col gap-2 h-full w-full" style="align-items: center;justify-content: center;">
                    <span class="status-largenumber">{{stat.totalRuntime}}</span>
                    <span style="opacity:0.5">Total Runtime</span>
                </div>
                <div class="card flex flex-col gap-2 h-full w-full" style="align-items: center;justify-content: center;">
                    <span class="status-largenumber">{{stat.oldestRun | date: 'dd/MM/yyyy HH:mm:ss'}}</span>
                    <span style="opacity:0.5">Oldest Run</span>
                </div>
                <div class="card flex flex-col gap-2 h-full w-full" style="align-items: center;justify-content: center;">
                    <span class="status-largenumber">{{stat.latestRun | date: 'dd/MM/yyyy HH:mm:ss'}}</span>
                    <span style="opacity:0.5">Newest Run</span>
                </div>
            </div>

            <div class="flex flex-row gap-2 w-full">
                <div class="card flex flex-col gap-2 h-full w-full" style="align-items: center;justify-content: center;">
                    @if(stat.activeWorkflows == 0){
                        <span>No active workflows...</span>
                    }
                    @else {
                        <p-chart [type]="stateChartOptions.type" [options]="stateChartOptions" [data]="activeStateData()"  class="w-full" />
                    }
                    <span style="opacity:0.5">Active Workflow Statuses</span>
                </div>
                <div class="card flex flex-col gap-2 h-full w-full" style="align-items: center;justify-content: center;">
                    @if(stat.archivedWorkflows == 0){
                        <span>No archived workflows...</span>
                    }
                    @else {
                        <p-chart [type]="stateChartOptions.type" [options]="stateChartOptions" [data]="archivedStateData()"  class="w-full" />
                    }
                    <span style="opacity:0.5">Archived Workflows Statuses</span>
                </div>
            </div>

            <div class="flex flex-row gap-2 w-full">
                <div class="card flex flex-col gap-2 h-full w-full" style="align-items: center;justify-content: center;">
                    @if(stat.mostExpensiveRun){
                        <span class="status-largenumber">{{stat.mostExpensiveRun.name}}</span>
                        <span style="font-style: italic;">{{stat.mostExpensiveRun.runtime}}</span>
                    }
                    @else {
                        <span>No workflow runs have been made yet...</span>
                    }
                    <span style="opacity:0.5">Most expensive workflow run</span>
                </div>
                <div class="card flex flex-col gap-2 h-full w-full" style="align-items: center;justify-content: center;">
                    @if(stat.leastExpensiveRun){
                        <span class="status-largenumber">{{stat.leastExpensiveRun.name}}</span>
                        <span style="font-style: italic;">{{stat.leastExpensiveRun.runtime}}</span>
                    }
                    @else {
                        <span>No workflow runs have been made yet...</span>
                    }
                    <span style="opacity:0.5">Least expensive workflow run</span>
                </div>
            </div>

            <div class="flex flex-row gap-2 w-full">
                <div class="card flex flex-col gap-2 h-full w-full" style="align-items: center;justify-content: center;">
                    <p-chart [type]="timelineChartOptions.type" [options]="timelineChartOptions" [data]="runsPrDayData()"  class="w-full" />
                    <span style="opacity:0.5">Runs the last 7 days</span>
                </div>
            </div>
        </div>
    }
    `,
    host:{
        class: 'flex flex-col flex-grow'
    },
    styles: `
        .status-xlargenumber {
            font-size:70px;
            text-align: center;
        }

        .status-largenumber {
            font-size:30px;
            text-align: center;
        }

        .status-reloadbutton {
            position:absolute;
            margin-left: -20px;
            margin-top: -20px;
        }
    `
})
export class Status {
    status = signal<StatusModel>({} as StatusModel);
    isLoading = signal<boolean>(false);

    stateChartOptions : any = { type:'pie' };
    activeStateData = signal({})
    archivedStateData = signal({})

    timelineChartOptions : any = {
        type:'line',
        plugins: {
            legend: false
        },
        scales: {
            x: {
                type:'time',
                time:{
                    unit: 'day'
                }
            },
            y: {
                ticks: {
                    stepSize: 1
                }
            }
        }
    };
    runsPrDayData = signal({})

    constructor(private http : HttpClient){}

    async ngOnInit(){
        await this.loadStatus();
    }

    async loadStatus(){
        this.isLoading.set(true);
        var status = await firstValueFrom(this.http.get<StatusModel>("/api/status"))
        if (status.totalRuntime)
            status.totalRuntime = status.totalRuntime.split('.')[0]
        if (status.totalRuntime && status.totalRuntime == "00:00:00")
            status.totalRuntime = "Less than a second"

        this.activeStateData.set(this.buildChart(status.activeStateMap));
        this.archivedStateData.set(this.buildChart(status.archivedStateMap));

        if (status.mostExpensiveRun)
            status.mostExpensiveRun.runtime = status.mostExpensiveRun.runtime.split('.')[0]
        if (status.mostExpensiveRun && status.mostExpensiveRun.runtime == "00:00:00")
            status.mostExpensiveRun.runtime = "Less than a second"

        if (status.leastExpensiveRun)
            status.leastExpensiveRun.runtime = status.leastExpensiveRun.runtime.split('.')[0]
        if (status.leastExpensiveRun && status.leastExpensiveRun.runtime == "00:00:00")
            status.leastExpensiveRun.runtime = "Less than a second"

        this.runsPrDayData.set({
            datasets: [
                {
                    fill:true,
                    data: Object.keys(status.runsPrDay).map(l => { return {
                        x: new Date(l),
                        y: status.runsPrDay[l]
                    }; }),
                    tension: 0.5
                }
            ]
        });
        this.status.set(status);
        this.isLoading.set(false);
    }

    buildChart(data : { [name:string]:number }){
        return {
            labels: Object.keys(data),
            datasets: [
                {
                    data: Object.values(data)
                }
            ]
        }
    }
}

interface StatusModel {
    activeWorkflows : number;
    archivedWorkflows : number;

    totalRuntime : string;
    oldestRun : Date | null;
    latestRun : Date | null;

    activeStateMap : { [name:string]:number };
    archivedStateMap : { [name:string]:number };

    mostExpensiveRun : StatusModelRun | null;
    leastExpensiveRun : StatusModelRun | null;

    runsPrDay : { [name:string]:number };
}

interface StatusModelRun {
    id: string;
    name: string;
    runtime: string;
}
