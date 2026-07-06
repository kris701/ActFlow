import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { TuiTable } from '@taiga-ui/addon-table';
import { TuiButton, TuiExpand, TuiGroup, TuiNotificationService } from '@taiga-ui/core';
import { TuiAccordion, TuiChip, TuiProgressBar } from '@taiga-ui/kit';
import { EditorComponent } from "ngx-monaco-editor-v2";
import { firstValueFrom } from 'rxjs';
import { FloatTable } from "../../common/components/floattable";
import { WorkflowState } from '../../models/WorkflowState';
import { WorkflowEditor } from "./components/wor.components.workfloweditor";
import { WorkflowStateService } from './services/wor.stateservice';

@Component({
    selector: 'app-wor-res',
    imports: [
    CommonModule,
    FormsModule,
    FloatTable,
    TuiTable,
    TuiChip,
    TuiButton,
    TuiGroup,
    TuiChip,
    TuiProgressBar,
    EditorComponent,
    TuiAccordion,
    TuiExpand,
    WorkflowEditor
],
    template: `
	<app-floattable [values]="workflowStateService.items()" [expandable]="true" [isLoading]="isLoading" (onRowExpanded)="checkCache($event.id)" (onLoadItems)="loadItems()" [showRefresh]="true">
		<ng-template #tableHeader>
			<th tuiTh>Name</th>
			<th tuiTh>Status</th>
			<th tuiTh>Started At</th>
			<th tuiTh>Ended At</th>
			<th tuiTh style="width:2rem"></th>
		</ng-template>
		<ng-template #tableRows let-item>
			<td tuiTd>{{ item.name }}</td>
			<td tuiTd>
				<span [appearance]="statusSeverityMap[item.status]" size="xs" tuiChip>
					{{statusNameMap[item.status]}}
				</span>
			</td>
			<td tuiTd>{{ item.startedAt | date: 'dd/MM/yyyy HH:mm:ss' }}</td>
			<td tuiTd>{{ item.endedAt | date: 'dd/MM/yyyy HH:mm:ss' }}</td>
			<td tuiTd>
				@if(item.isArchived){
					<div class="flex flex-row gap-2">
						<button tuiButton iconStart="x" size="s" appearance="negative" (click)="deleteWorkflowRun(item.id)"></button>
						<button tuiButton iconStart="rotate-cw" size="s" appearance="info" (click)="rerunWorkflow(item.id)"></button>
					</div>
				}
				@else {
					<button tuiButton iconStart="x" size="s" appearance="warning" (click)="cancelWorkflowRun(item.id)"></button>
				}
			</td>
		</ng-template>
		<ng-template #tableExpandedrow let-item>
			<td colSpan="6">
				<div class="flex flex-col p-2 gap-2">
					@let items = itemCache();
					@let fullItem = items[item.id];
					@if(fullItem){
						<span style="text-align: center;">Workflow Run Details</span>
						<div class="sub-container">
							<div tuiGroup [collapsed]="true" [rounded]="true">
								<span size="s" tuiChip>ID</span>
								<span size="s" appearance="info" tuiChip>{{fullItem.id}}</span>
							</div>
							<div tuiGroup [collapsed]="true" [rounded]="true">
								<span size="s" tuiChip>Name</span>
								<span size="s" appearance="info" tuiChip>{{fullItem.workflow.name}}</span>
							</div>
							<div tuiGroup [collapsed]="true" [rounded]="true">
								<span size="s" tuiChip>Status</span>
								<span size="s" [appearance]="statusSeverityMap[fullItem.status]" tuiChip>{{statusNameMap[fullItem.status]}}</span>
							</div>
							<div tuiGroup [collapsed]="true" [rounded]="true">
								<span size="s" tuiChip>Activity Index</span>
								<span size="s" appearance="info" tuiChip>{{fullItem.activityIndex}} out of {{fullItem.workflow.activities.length}}</span>
							</div>
							<div tuiGroup [collapsed]="true" [rounded]="true">
								<span size="s" tuiChip>Context items</span>
								<span size="s" appearance="info" tuiChip>{{Object.keys(fullItem.contextStore).length}}</span>
							</div>
							<div tuiGroup [collapsed]="true" [rounded]="true">
								<span size="s" tuiChip>Started</span>
								<span size="s" appearance="info" tuiChip>{{fullItem.startedAt | date: 'dd/MM/yyyy HH:mm:ss'}}</span>
							</div>
							<div tuiGroup [collapsed]="true" [rounded]="true">
								<span size="s" tuiChip>Ended</span>
								<span size="s" appearance="info" tuiChip>{{fullItem.endedAt | date: 'dd/MM/yyyy HH:mm:ss'}}</span>
							</div>
							<div tuiGroup [collapsed]="true" [rounded]="true">
								<span size="s" tuiChip>Logs</span>
								<span size="s" appearance="info" tuiChip>{{fullItem.logText.length}}</span>
							</div>
							<div tuiGroup [collapsed]="true" [rounded]="true">
								<span size="s" tuiChip>File Actions</span>
								<span size="s" appearance="info" tuiChip>{{fullItem.files.length}}</span>
							</div>
						</div>

						<tui-accordion>
							<button tuiAccordion>Context Items</button>
							<tui-expand>
								<ng-container *tuiItem>
									<div class="flex flex-col gap-2">
										<span>Context Store</span>
										<ngx-monaco-editor [options]="editorOptions" [ngModel]="JSON.stringify(fullItem.contextStore, null, 4)" [disabled]="true"> </ngx-monaco-editor>
									</div>
								</ng-container>
							</tui-expand>
						</tui-accordion>

						<tui-accordion>
							<button tuiAccordion>Logs</button>
							<tui-expand>
								<ng-container *tuiItem>
									<div class="flex flex-col gap-2">
										<span>Log Items</span>
										@for(logItem of fullItem.logText; track logItem){
											<span size="s" class="w-full h-full" [appearance]="logSeverityMap[logItem.logType]" tuiChip>
												<span style="overflow:hidden;text-wrap:auto">{{logItem.text}}</span>
											</span>
										}
									</div>
								</ng-container>
							</tui-expand>
						</tui-accordion>

						<tui-accordion>
							<button tuiAccordion>View Workflow</button>
							<tui-expand>
								<ng-container *tuiItem>
									<div class="flex flex-col gap-2">
										<app-workflows-components-workfloweditor [workflow]="fullItem.sourceWorkflow" [workflowState]="fullItem" [disabled]="true"/>
									</div>
								</ng-container>
							</tui-expand>
						</tui-accordion>

						@if(fullItem.files.length > 0){
							<tui-accordion>
								<button tuiAccordion>View File Actions</button>
								<tui-expand>
										<div class="flex flex-col gap-2">
											<span>See what file actions was performed during the execution</span>
											@for(file of fullItem.files; track file){
												<div tuiGroup [collapsed]="true" [rounded]="true" orientation="horizontal">
													<span size="s" tuiChip [style.flex]="'0 0 auto'">
														@if(file.directory == 'Temporary'){
															Temporary
														}
														@else {
															Persistent
														}
													</span>
													<span size="s" tuiChip [style.flex]="'0 0 auto'">
														@if(file.action == 'Load'){
															Load
														}
														@else {
															Save
														}
													</span>
													<span size="s" appearance="info" tuiChip>{{file.path}}</span>
													@if(file.directory == 'Persistent'){
														<button tuiButton size="s" [style.flex]="'0 0 auto'" (click)="viewPersistentFile(file.path)">View</button>
													}
													@else if(file.directory == 'Temporary'){
														<button tuiButton size="s" [style.flex]="'0 0 auto'" (click)="viewTemporaryFile(fullItem.id, file.path)">View</button>
													}
												</div>
											}
										</div>
								</tui-expand>
							</tui-accordion>
						}
					}
					@else {
						<progress tuiProgressBar></progress>
					}
				</div>
			</td>
		</ng-template>
	</app-floattable>
    `,
    host:{
        class: 'base-view'
    },
	styles: `
		.sub-container {
			display:flex;
			flex-direction: row;
			padding:5px;
			gap:5px;
			flex-wrap:wrap;
			justify-content: center;

			> div :first-child {
				flex: 0 0 auto;
            }
		}
	`
})
export class WORResults {
	isLoading = signal<boolean>(false);

    itemCache = signal<{ [id:string]:WorkflowState }>({})

	editorOptions = {theme: 'vs-dark', language: 'json', automaticLayout: false};

    statusNameMap : {[id:string]:string} = {
        "None" : "None",
        "NotStarted" : "Not Started",
        "Running" : "Running",
        "Failed" : "Failed",
        "Succeeded" : "Succeeded",
        "Canceled" : "Canceled",
        "AwaitingHumanInput" : "Awaiting Human Input",
    };
    statusSeverityMap : {[id:string]:string} = {
        "None" : "secondary",
        "NotStarted" : "secondary",
        "Running" : "info",
        "Failed" : "negative",
        "Succeeded" : "positive",
        "Canceled" : "warning",
        "AwaitingHumanInput" : "accent",
    };

    logSeverityMap : {[id:string]:string} = {
        "Info" : "info",
        "Warn" : "warning",
        "Error" : "negative",
    };

    Object = Object;
    JSON = JSON;

	constructor(private http : HttpClient, public service: TuiNotificationService, public workflowStateService : WorkflowStateService, public router : Router){}

	async loadItems(){
		this.isLoading.set(true);
		await this.workflowStateService.Load();
		this.isLoading.set(false);
	}

    async checkCache(id : string){
        var items = this.itemCache();
        if (items[id])
            return;
        this.isLoading.set(true);
        var item = await this.workflowStateService.Get(id);
        items[id] = item;
        this.itemCache.set(items);
        this.isLoading.set(false);
    }

    async deleteWorkflowRun(id : string){
        this.isLoading.set(true);
        await firstValueFrom(this.http.delete("/api/results?id=" + id))
		this.service.open("The archived workflow has been deleted!", {
			label: "'Workflow Deleted",
			appearance: 'positive',
			icon:'info',
			autoClose: 1000
		}).subscribe();
        this.isLoading.set(false);
        await this.workflowStateService.Load();
    }

    async cancelWorkflowRun(id : string){
        this.isLoading.set(true);
        await firstValueFrom(this.http.delete("/api/execute/cancel?id=" + id))
		this.service.open("A request to cancel the workflow have been send!", {
			label: "'Workflow Cancled",
			appearance: 'positive',
			icon:'info',
			autoClose: 1000
		}).subscribe();
        this.isLoading.set(false);
        await this.workflowStateService.Load();
    }

    async rerunWorkflow(id : string){
        var items = this.itemCache();
        if (items[id])
            sessionStorage.setItem("tmpWorkflowTransfer", this.JSON.stringify((items[id]).sourceWorkflow, null, 4));
        sessionStorage.setItem("tmpWorkflowTransfer", this.JSON.stringify((await this.workflowStateService.Get(id)).sourceWorkflow, null, 4));
        this.router.navigateByUrl("workflows/run");
    }

    viewPersistentFile(path : string){
        this.router.navigate(["files/persistent"], { queryParams: { path:path } });
    }

    viewTemporaryFile(id : string, path : string){
        this.router.navigate(["files/temporary"], { queryParams: { id: id, path:path } });
    }
}
