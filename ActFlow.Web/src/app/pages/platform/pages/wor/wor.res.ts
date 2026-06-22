import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { EditorComponent } from "ngx-monaco-editor-v2";
import { MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { InplaceModule } from 'primeng/inplace';
import { InputGroup } from "primeng/inputgroup";
import { InputGroupAddon } from "primeng/inputgroupaddon";
import { TableModule } from 'primeng/table';
import { Tag } from "primeng/tag";
import { firstValueFrom } from 'rxjs';
import { FloatTable } from "../../../../common/components/floattable";
import { TableDateFilterColumn, TableTextFilterColumn } from '../../../../common/components/tables/filtercolumns';
import { TableDateRow, TableTagRow } from '../../../../common/components/tables/filterrows';
import { WorkflowState } from '../../../../models/WorkflowState';
import { WorkflowEditor } from "./components/wor.components.workfloweditor";
import { WorkflowStateService } from './services/wor.stateservice';

@Component({
    selector: 'app-wor-res',
    imports: [
    CommonModule,
    FormsModule,
    ButtonModule,
    FloatTable,
    TableTextFilterColumn,
    TableDateFilterColumn,
    TableTagRow,
    TableDateRow,
    TableModule,
    Tag,
    InputGroup,
    InputGroupAddon,
    InplaceModule,
    EditorComponent,
    WorkflowEditor
],
    template: `
        <app-floattable [values]="workflowStateService.items" [isLoading]="isLoading" (onLoadItems)="workflowStateService.Load()" [rowSelectable]="false" [showAdd]="false">
            <ng-template #tableHeader>
                <th style="width:2rem"></th>
                <th textfiltercolumn pSortableColumn="name" displayName="Name"></th>
                <th textfiltercolumn pSortableColumn="status" displayName="Status"></th>
                <th datefiltercolumn pSortableColumn="startedAt" displayName="Started At"></th>
                <th datefiltercolumn pSortableColumn="endedAt" displayName="Ended At"></th>
                <th style="width:2rem"></th>
            </ng-template>
            <ng-template #tableRows let-item let-expanded="expanded">
                <td style="width:2rem">
                    <p-button [pRowToggler]="item" [icon]="expanded === true ? 'pi pi-angle-down' : 'pi pi-angle-right'" text (onClick)="checkCache(item.id)" />
                </td>
                <td tagrow [value]="item.name"></td>
                <td tagrow [value]="statusNameMap[item.status]" [severity]="statusSeverityMap[item.status]"></td>
                <td daterow [value]="item.startedAt"></td>
                <td daterow [value]="item.endedAt"></td>
                <td style="width:2rem">
                    @if(item.isArchived){
                        <div class="flex flex-row gap-2">
                            <p-button icon="pi pi-times" text severity="danger" (onClick)="deleteWorkflowRun(item.id)" />
                            <p-button icon="pi pi-refresh" text severity="info" (onClick)="rerunWorkflow(item.id)" />
                        </div>
                    }
                    @else {
                        <p-button icon="pi pi-times" text severity="warn" (onClick)="cancelWorkflowRun(item.id)" />
                    }
                </td>
            </ng-template>
            <ng-template #tableExpandedrow let-item>
                <td colspan="6">
                    <div class="gap-2 result-details-container result-details-border">
                        @if(itemCache[item.id] && !isLoading){
                            @let fullItem = itemCache[item.id];
                            <span style="text-align: center;">Workflow Run Details</span>
                            <div class="sub-container">
                                <p-inputgroup>
                                    <p-inputgroup-addon class="flex flex-row gap-2">
                                        <i class="pi pi-hashtag"></i>
                                        ID
                                    </p-inputgroup-addon>
                                    <p-tag severity="secondary">{{fullItem.id}}</p-tag>
                                </p-inputgroup>

                                <p-inputgroup>
                                    <p-inputgroup-addon class="flex flex-row gap-2">
                                        <i class="pi pi-pencil"></i>
                                        Name
                                    </p-inputgroup-addon>
                                    <p-tag severity="secondary">{{fullItem.workflow.name}}</p-tag>
                                </p-inputgroup>

                                <p-inputgroup>
                                    <p-inputgroup-addon class="flex flex-row gap-2">
                                        <i class="pi pi-user"></i>
                                        Status
                                    </p-inputgroup-addon>
                                    <p-tag [severity]="statusSeverityMap[fullItem.status]">{{statusNameMap[fullItem.status]}}</p-tag>
                                </p-inputgroup>

                                <p-inputgroup>
                                    <p-inputgroup-addon class="flex flex-row gap-2">
                                        <i class="pi pi-bars"></i>
                                        Activity Index
                                    </p-inputgroup-addon>
                                    <p-tag severity="secondary">{{fullItem.activityIndex}} out of {{fullItem.workflow.activities.length}}</p-tag>
                                </p-inputgroup>

                                <p-inputgroup>
                                    <p-inputgroup-addon class="flex flex-row gap-2">
                                        <i class="pi pi-box"></i>
                                        Context items
                                    </p-inputgroup-addon>
                                    <p-tag severity="secondary">{{Object.keys(fullItem.contextStore).length}}</p-tag>
                                </p-inputgroup>

                                <p-inputgroup>
                                    <p-inputgroup-addon class="flex flex-row gap-2">
                                        <i class="pi pi-calendar-clock"></i>
                                        Started
                                    </p-inputgroup-addon>
                                    <p-tag severity="secondary">{{fullItem.startedAt | date: 'dd/MM/yyyy HH:mm:ss'}}</p-tag>
                                </p-inputgroup>

                                <p-inputgroup>
                                    <p-inputgroup-addon class="flex flex-row gap-2">
                                        <i class="pi pi-calendar-clock"></i>
                                        Ended
                                    </p-inputgroup-addon>
                                    <p-tag severity="secondary">{{fullItem.endedAt | date: 'dd/MM/yyyy HH:mm:ss'}}</p-tag>
                                </p-inputgroup>

                                <p-inputgroup>
                                    <p-inputgroup-addon class="flex flex-row gap-2">
                                        <i class="pi pi-book"></i>
                                        Logs
                                    </p-inputgroup-addon>
                                    <p-tag severity="secondary">{{fullItem.logText.length}}</p-tag>
                                </p-inputgroup>

                                <p-inputgroup>
                                    <p-inputgroup-addon class="flex flex-row gap-2">
                                        <i class="pi pi-book"></i>
                                        File Actions
                                    </p-inputgroup-addon>
                                    <p-tag severity="secondary">{{fullItem.files.length}}</p-tag>
                                </p-inputgroup>
                            </div>

                            <div class="result-details-border">
                                <p-inplace>
                                    <ng-template #display>
                                        <span>View Context Items</span>
                                    </ng-template>
                                    <ng-template #content>
                                        <div class="flex flex-col gap-2">
                                            <span>Context Store</span>
                                            <ngx-monaco-editor [options]="editorOptions" [ngModel]="JSON.stringify(fullItem.contextStore, null, 4)" [disabled]="true"> </ngx-monaco-editor>
                                        </div>
                                    </ng-template>
                                </p-inplace>
                            </div>

                            <div class="result-details-border">
                                <p-inplace>
                                    <ng-template #display>
                                        <span>View Logs</span>
                                    </ng-template>
                                    <ng-template #content>
                                        <div class="flex flex-col gap-2">
                                            <span>Log Items</span>
                                            @for(logItem of fullItem.logText; track logItem){
                                                <p-inputgroup>
                                                    <p-inputgroup-addon class="flex flex-row gap-2">
                                                        <i class="pi pi-book"></i>
                                                        Log
                                                    </p-inputgroup-addon>
                                                    <p-tag class="w-full" [severity]="logSeverityMap[logItem.logType]">{{logItem.text}}</p-tag>
                                                </p-inputgroup>
                                            }
                                        </div>
                                    </ng-template>
                                </p-inplace>
                            </div>

                            <div class="result-details-border">
                                <p-inplace>
                                    <ng-template #display>
                                        <span>View Workflow</span>
                                    </ng-template>
                                    <ng-template #content>
                                        <div class="flex flex-col gap-2">
                                            <app-workflows-components-workfloweditor [workflow]="fullItem.sourceWorkflow" [workflowState]="fullItem" [disabled]="true"/>
                                        </div>
                                    </ng-template>
                                </p-inplace>
                            </div>

                            @if(fullItem.files.length > 0) {
                                <div class="result-details-border">
                                    <p-inplace>
                                        <ng-template #display>
                                            <span>View File Actions</span>
                                        </ng-template>
                                        <ng-template #content>
                                            <div class="flex flex-col gap-2">
                                                <span>See what file actions was performed during the execution</span>
                                                @for(file of fullItem.files; track file){
                                                    <p-inputgroup>
                                                        <p-inputgroup-addon class="flex flex-row gap-2">
                                                            @if(file.directory == 'Temporary'){
                                                                <p-tag severity="info">Temporary</p-tag>
                                                            }
                                                            @else if(file.directory == 'Persistent'){
                                                                <p-tag severity="info">Persistent</p-tag>
                                                            }
                                                        </p-inputgroup-addon>
                                                        <p-inputgroup-addon class="flex flex-row gap-2">
                                                            @if(file.action == 'Load'){
                                                                <p-tag severity="secondary">Load</p-tag>
                                                            }
                                                            @else if(file.action == 'Save'){
                                                                <p-tag severity="secondary">Save</p-tag>
                                                            }
                                                        </p-inputgroup-addon>
                                                        <p-tag class="w-full" severity="secondary">{{file.path}}</p-tag>

                                                        @if(file.directory == 'Persistent'){
                                                            <p-inputgroup-addon class="flex flex-row gap-2">
                                                                <p-button class="h-full" label="View" (onClick)="viewFile(file.path)"/>
                                                            </p-inputgroup-addon>
                                                        }
                                                    </p-inputgroup>
                                                }
                                            </div>
                                        </ng-template>
                                    </p-inplace>
                                </div>
                            }
                        }
                        @else {
                            <span style="text-align: center;">Loading...</span>
                        }
                    </div>
                </td>
            </ng-template>
        </app-floattable>
    `,
    host:{
        class: 'card flex flex-col flex-grow'
    },
    styles: `
        .result-details-border {
            padding:5px;
            border-radius: 5px;
            border: 1px solid #415b6181;
        }

        .result-details-container {
            display:flex;
            flex-direction: column;

            .sub-container {
                display:flex;
                flex-direction: row;
                padding:5px;
                gap:5px;
                flex-wrap:wrap;
            }

            p-inputgroup {
                width:auto;
            }
        }
    `
})
export class Results {
    isLoading : boolean = false;

    itemCache : { [id:string]:WorkflowState } = {}

    editorOptions = {theme: 'vs-dark', language: 'json', automaticLayout: true};

    statusNameMap : {[id:string]:string} = {
        "None" : "None",
        "NotStarted" : "Not Started",
        "Running" : "Running",
        "Failed" : "Failed",
        "Succeeded" : "Succeeded",
        "Canceled" : "Canceled",
        "AwaitingHumanInput" : "Awaiting Human Input",
    };
    statusSeverityMap : {[id:string]:'success' | 'secondary' | 'info' | 'warn' | 'danger' | 'contrast' | undefined | null} = {
        "None" : "secondary",
        "NotStarted" : "secondary",
        "Running" : "info",
        "Failed" : "danger",
        "Succeeded" : "success",
        "Canceled" : "warn",
        "AwaitingHumanInput" : "contrast",
    };

    logSeverityMap : {[id:string]:'success' | 'secondary' | 'info' | 'warn' | 'danger' | 'contrast' | undefined | null} = {
        "Info" : "info",
        "Warn" : "warn",
        "Error" : "danger",
    };

    Object = Object;
    JSON = JSON;

    constructor(private http : HttpClient, public service: MessageService, public workflowStateService : WorkflowStateService, public router : Router){}

    async checkCache(id : string){
        if (this.itemCache[id])
            return;
        this.isLoading = true;
        var item = await this.workflowStateService.Get(id);
        this.itemCache[id] = item;
        this.isLoading = false;
    }

    async deleteWorkflowRun(id : string){
        this.isLoading = true;
        await firstValueFrom(this.http.delete("/api/results?id=" + id))
        this.service.add({ severity: 'success', summary: 'Workflow Deleted!', detail: 'The archived workflow has been deleted' });
        this.isLoading = false;
        await this.workflowStateService.Load();
    }

    async cancelWorkflowRun(id : string){
        this.isLoading = true;
        await firstValueFrom(this.http.delete("/api/execute/cancel?id=" + id))
        this.service.add({ severity: 'success', summary: 'Workflow Cancel Requested!', detail: 'A request to cancel the workflow have been send!' });
        this.isLoading = false;
        await this.workflowStateService.Load();
    }

    async rerunWorkflow(id : string){
        if (this.itemCache[id])
            sessionStorage.setItem("tmpWorkflowTransfer", this.JSON.stringify((this.itemCache[id]).sourceWorkflow, null, 4));
        sessionStorage.setItem("tmpWorkflowTransfer", this.JSON.stringify((await this.workflowStateService.Get(id)).sourceWorkflow, null, 4));
        this.router.navigateByUrl("workflows/run");
    }

    viewFile(path : string){
        this.router.navigate(["files/persistent"], { queryParams: { path:path } });
    }
}
