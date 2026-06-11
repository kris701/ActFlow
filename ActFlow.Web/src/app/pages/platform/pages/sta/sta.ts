import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import 'chartjs-adapter-date-fns';
import { ButtonModule } from 'primeng/button';
import { ChartModule } from 'primeng/chart';
import { firstValueFrom } from 'rxjs';

@Component({
    selector: 'app-sta',
    imports: [
    CommonModule, ChartModule, ButtonModule
],
    template: `
    <p-button icon="pi pi-refresh" (onClick)="loadStatus()" text class="status-reloadbutton"/>

    <div class="flex flex-col gap-2">
        <div class="flex flex-row gap-2 w-full">
            <div class="card flex flex-col gap-2 h-full w-full" style="align-items: center;justify-content: center;">
                <span class="status-xlargenumber">{{status.activeWorkflows}}</span>
                <span style="opacity:0.5">Active Workflows</span>
            </div>
            <div class="card flex flex-col gap-2 h-full w-full" style="align-items: center;justify-content: center;">
                <span class="status-xlargenumber">{{status.archivedWorkflows}}</span>
                <span style="opacity:0.5">Archived Workflows</span>
            </div>
        </div>

        <div class="flex flex-row gap-2 w-full">
            <div class="card flex flex-col gap-2 h-full w-full" style="align-items: center;justify-content: center;">
                <span class="status-largenumber">{{status.totalRuntime}}</span>
                <span style="opacity:0.5">Total Runtime</span>
            </div>
            <div class="card flex flex-col gap-2 h-full w-full" style="align-items: center;justify-content: center;">
                <span class="status-largenumber">{{status.oldestRun | date: 'dd/MM/yyyy HH:mm:ss'}}</span>
                <span style="opacity:0.5">Oldest Run</span>
            </div>
            <div class="card flex flex-col gap-2 h-full w-full" style="align-items: center;justify-content: center;">
                <span class="status-largenumber">{{status.latestRun | date: 'dd/MM/yyyy HH:mm:ss'}}</span>
                <span style="opacity:0.5">Newest Run</span>
            </div>
        </div>

        <div class="flex flex-row gap-2 w-full">
            <div class="card flex flex-col gap-2 h-full w-full" style="align-items: center;justify-content: center;">
                @if(status.activeWorkflows == 0){
                    <span>No active workflows...</span>
                }
                @else {
                    <p-chart [type]="stateChartOptions.type" [options]="stateChartOptions" [data]="activeStateData"  class="w-full" />
                }
                <span style="opacity:0.5">Active Workflow Statuses</span>
            </div>
            <div class="card flex flex-col gap-2 h-full w-full" style="align-items: center;justify-content: center;">
                @if(status.archivedWorkflows == 0){
                    <span>No archived workflows...</span>
                }
                @else {
                    <p-chart [type]="stateChartOptions.type" [options]="stateChartOptions" [data]="archivedStateData"  class="w-full" />
                }
                <span style="opacity:0.5">Archived Workflows Statuses</span>
            </div>
        </div>

        <div class="flex flex-row gap-2 w-full">
            <div class="card flex flex-col gap-2 h-full w-full" style="align-items: center;justify-content: center;">
                @if(status.mostExpensiveRun){
                    <span class="status-largenumber">{{status.mostExpensiveRun.name}}</span>
                    <span style="font-style: italic;">{{status.mostExpensiveRun.runtime}}</span>
                }
                @else {
                    <span>No workflow runs have been made yet...</span>
                }
                <span style="opacity:0.5">Most expensive workflow run</span>
            </div>
            <div class="card flex flex-col gap-2 h-full w-full" style="align-items: center;justify-content: center;">
                @if(status.leastExpensiveRun){
                    <span class="status-largenumber">{{status.leastExpensiveRun.name}}</span>
                    <span style="font-style: italic;">{{status.leastExpensiveRun.runtime}}</span>
                }
                @else {
                    <span>No workflow runs have been made yet...</span>
                }
                <span style="opacity:0.5">Least expensive workflow run</span>
            </div>
        </div>

        <div class="flex flex-row gap-2 w-full">
            <div class="card flex flex-col gap-2 h-full w-full" style="align-items: center;justify-content: center;">
                <p-chart [type]="timelineChartOptions.type" [options]="timelineChartOptions" [data]="runsPrDayData"  class="w-full" />
                <span style="opacity:0.5">Runs the last 7 days</span>
            </div>
        </div>
    </div>
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
    status : StatusModel = {} as StatusModel;

    stateChartOptions : any = { type:'pie' };
    activeStateData = {}
    archivedStateData = {}

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
    runsPrDayData = {}

    constructor(private http : HttpClient){}

    async ngOnInit(){
        await this.loadStatus();
    }

    async loadStatus(){
        this.status = await firstValueFrom(this.http.get<StatusModel>("/api/status"))
        if (this.status.totalRuntime)
            this.status.totalRuntime = this.status.totalRuntime.split('.')[0]
        if (this.status.totalRuntime && this.status.totalRuntime == "00:00:00")
            this.status.totalRuntime = "Less than a second"

        this.activeStateData = this.buildChart(this.status.activeStateMap);
        this.archivedStateData = this.buildChart(this.status.archivedStateMap);

        if (this.status.mostExpensiveRun)
            this.status.mostExpensiveRun.runtime = this.status.mostExpensiveRun.runtime.split('.')[0]
        if (this.status.mostExpensiveRun && this.status.mostExpensiveRun.runtime == "00:00:00")
            this.status.mostExpensiveRun.runtime = "Less than a second"

        if (this.status.leastExpensiveRun)
            this.status.leastExpensiveRun.runtime = this.status.leastExpensiveRun.runtime.split('.')[0]
        if (this.status.leastExpensiveRun && this.status.leastExpensiveRun.runtime == "00:00:00")
            this.status.leastExpensiveRun.runtime = "Less than a second"

        this.runsPrDayData = {

            datasets: [
                {
                    backgroundColor: Object.keys(this.status.runsPrDay).map(x => this.getRandomColor()),
                    fill:true,
                    data: Object.keys(this.status.runsPrDay).map(l => { return {
                        x: new Date(l),
                        y: this.status.runsPrDay[l]
                    }; })
                }
            ]
        };

        console.log(this.runsPrDayData)
    }

    buildChart(data : { [name:string]:number }){
        return {
            labels: Object.keys(data),
            datasets: [
                {
                    backgroundColor: Object.keys(data).map(x => this.getRandomColor()),
                    data: Object.values(data)
                }
            ]
        }
    }

    getRandomColor() {
        var letters = '0123456789ABCDEF'.split('');
        var color = '#';
        for (var i = 0; i < 6; i++ ) {
            color += letters[Math.floor(Math.random() * 16)];
        }
        return color;
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
