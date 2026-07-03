import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, signal } from '@angular/core';
import { TuiButton } from '@taiga-ui/core';
import { TuiProgress } from '@taiga-ui/kit';
import { TuiCardMedium } from '@taiga-ui/layout';
import 'chartjs-adapter-date-fns';
import { BaseChartDirective } from 'ng2-charts';
import { firstValueFrom } from 'rxjs';

@Component({
    selector: 'app-sta',
    imports: [
    CommonModule, TuiButton, TuiProgress, TuiCardMedium, BaseChartDirective
],
    template: `
    <button tuiButton appearance="flat" size="s" iconStart="rotate-cw" (click)="loadStatus()" class="status-reloadbutton"></button>

    @if(isLoading()){
        <progress tuiProgressBar></progress>
    }
    @else {
        @let stat = status();
        <div class="flex flex-col gap-2 w-full h-full">
			<div class="status-grid-2 w-full">
				<div tuiCardMedium class="flex flex-col w-full h-full items-center">
					<span class="status-xlargenumber">{{stat.activeWorkflows}}</span>
					<span style="opacity:0.5">Active Workflows</span>
				</div>
				<div tuiCardMedium class="flex flex-col w-full h-full items-center">
					<span class="status-xlargenumber">{{stat.archivedWorkflows}}</span>
					<span style="opacity:0.5">Archived Workflows</span>
				</div>
			</div>

			<div class="status-grid-3 w-full">
				<div tuiCardMedium class="flex flex-col w-full h-full items-center">
					<span class="status-largenumber">{{stat.totalRuntime}}</span>
					<span style="opacity:0.5">Total Runtime</span>
				</div>
				<div tuiCardMedium class="flex flex-col w-full h-full items-center">
					<span class="status-largenumber">{{stat.oldestRun | date: 'dd/MM/yyyy HH:mm:ss'}}</span>
					<span style="opacity:0.5">Oldest Run</span>
				</div>
				<div tuiCardMedium class="flex flex-col w-full h-full items-center">
					<span class="status-largenumber">{{stat.latestRun | date: 'dd/MM/yyyy HH:mm:ss'}}</span>
					<span style="opacity:0.5">Newest Run</span>
				</div>
			</div>

			<div class="status-grid-2 w-full">
				<div tuiCardMedium class="flex flex-col w-full h-full items-center" style="max-height:300px">
					@if(stat.activeWorkflows == 0){
						<span>No active workflows...</span>
					}
					@else {
						<canvas class="p-4" baseChart [data]="activeStateData()" [options]="stateChartOptions" [type]="'pie'"> </canvas>
					}
					<span style="opacity:0.5">Active Workflow Statuses</span>
				</div>
				<div tuiCardMedium class="flex flex-col w-full h-full items-center" style="max-height:300px">
					@if(stat.archivedWorkflows == 0){
						<span>No archived workflows...</span>
					}
					@else {
						<canvas class="p-4" baseChart [data]="archivedStateData()" [options]="stateChartOptions" [type]="'pie'"> </canvas>
					}
					<span style="opacity:0.5">Archived Workflows Statuses</span>
				</div>
			</div>

			<div class="status-grid-2 w-full">
				<div tuiCardMedium class="flex flex-col w-full h-full items-center">
					@if(stat.mostExpensiveRun){
						<span class="status-largenumber">{{stat.mostExpensiveRun.name}}</span>
						<span style="font-style: italic;">{{stat.mostExpensiveRun.runtime}}</span>
					}
					@else {
						<span>No workflow runs have been made yet...</span>
					}
					<span style="opacity:0.5">Most expensive workflow run</span>
				</div>
				<div tuiCardMedium class="flex flex-col w-full h-full items-center">
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

			<div class="status-grid-1 w-full">
				<div tuiCardMedium class="flex flex-col w-full h-full items-center">
					<canvas baseChart [data]="runsPrDayData()" [options]="timelineChartOptions" [type]="timelineChartOptions.type"> </canvas>
					<span style="opacity:0.5">Runs the last 7 days</span>
				</div>
			</div>
        </div>
    }
    `,
    host:{
        class: 'base-view'
    },
    styles: `
		.status-grid-1 {
			display:grid;
			grid-auto-flow: dense;
  			grid-gap: 1rem;
    		grid-template-columns: repeat(1, 1fr);
		}
		.status-grid-2 {
			display:grid;
			grid-auto-flow: dense;
  			grid-gap: 1rem;
    		grid-template-columns: repeat(2, 1fr);
		}
		.status-grid-3 {
			display:grid;
			grid-auto-flow: dense;
  			grid-gap: 1rem;
    		grid-template-columns: repeat(3, 1fr);
		}

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
            z-index: 99999;
        }
    `
})
export class Status {
    status = signal<StatusModel>({} as StatusModel);
    isLoading = signal<boolean>(false);

    stateChartOptions : any = {
		type:'pie',
		plugins: {
            legend: {
				display: false
			}
        }
	};
    activeStateData = signal<any>({})
    archivedStateData = signal<any>({})

    timelineChartOptions : any = {
        type:'line',
        plugins: {
            legend: {
				display: false
			}
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
    runsPrDayData = signal<any>({})

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
