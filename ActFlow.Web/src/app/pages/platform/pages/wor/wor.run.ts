import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { EditorComponent } from "ngx-monaco-editor-v2";
import { MenuItem, MessageService } from 'primeng/api';
import { Button } from "primeng/button";
import { Menubar } from "primeng/menubar";
import { firstValueFrom } from 'rxjs';
import { Workflow } from './../../../../models/Workflow';
import { WorkflowStateService } from './services/wor.stateservice';

@Component({
    selector: 'app-wor-run',
    imports: [
    CommonModule,
    FormsModule,
    EditorComponent,
    Button,
    Menubar
],
    template: `
        <span style="font-size:20;text-align:center">Run a workflow</span>
        <p-menubar [model]="items" [autoZIndex]="false" [baseZIndex]="1000" />
        <div class="flex h-full flex-grow">
            <ngx-monaco-editor style="flex-grow:1" [options]="editorOptions" [(ngModel)]="workflowText" (onInit)="editorLoaded()"> </ngx-monaco-editor>
        </div>
        <p-button icon="pi pi-bolt" label="Queue Workflow" fluid (onClick)="queueWorkflow()"/>
    `,
    host:{
        class: 'card flex flex-col flex-grow gap-2'
    },
    styles: `
        ngx-monaco-editor {
            height: 100%
        }
    `
})
export class WorkflowRun {
    editorOptions = {theme: 'vs-dark', language: 'json', automaticLayout: true};

    workflow : Workflow = { name: 'name', retryBehaviour: 0, globals: {}, activities: [] } as Workflow;
    workflowText : string = "";

    items: MenuItem[] = [];
    isMonacoLoaded: boolean = false;

    workers : ConfigWorkersResult[] = []
    contexts : BaseContext[] = []
    activities : BaseActivity[] = []

    constructor(private http : HttpClient, public service: MessageService, public workflowStateService : WorkflowStateService){}

    async ngOnInit(){
        var tmpTransfer = sessionStorage.getItem("tmpWorkflowTransfer");
        if (tmpTransfer)
        {
            this.workflowText = tmpTransfer;
            sessionStorage.removeItem("tmpWorkflowTransfer");
        }
        else
            this.workflowText = JSON.stringify(this.workflow, null, 4)

        this.workers = await firstValueFrom(this.http.get<ConfigWorkersResult[]>("api/config/workers"));
        this.contexts = await firstValueFrom(this.http.get<BaseContext[]>("api/config/contexts"));
        this.activities = await firstValueFrom(this.http.get<BaseActivity[]>("api/config/activities"));

        var newItems : MenuItem[] = []
        newItems.push({
            label: 'Add Activity',
            items: this.activities.map(x => { return {
                label: x.name + " (" + x.$type + ")",
                command: () => this.addActivity(x)
            } as MenuItem })
        });
        newItems.push({
            label: 'Add Context',
            items: this.contexts.map(x => { return {
                label: x.$type,
                command: () => this.copyContextToClipboard(x)
            } as MenuItem })
        });
        newItems.push({
            label: 'Add Global',
            command: () => this.addGlobal()
        })
        this.items = newItems;
    }

    editorLoaded() {
        this.isMonacoLoaded = true;
        this.setSchema();
    }

    setSchema() {
        (window as any).monaco.languages.json.jsonDefaults.setDiagnosticsOptions({
            validate: true,
            schemas: [
                {
                    uri: 'http://actflow/globals-schema.json',
                    fileMatch: ['*'],
                    schema: {
                        type: 'object',
                        properties: {
                            name: {
                                type: 'string'
                            },
                            retryBehaviour: {
                                type: 'number'
                            },
                            globals: {
                                type: 'object',
                                additionalProperties: {
                                    type: 'string'
                                }
                            },
                            activities: {
                                type: 'array',
                                items: {
                                    $ref: 'http://actflow/activities-schema.json'
                                }
                            }
                        },
                        required: ['globals', 'activities'],
                        additionalProperties: false
                    }
                },
                {
                    uri: 'http://actflow/activities-schema.json',
                    schema: {
                        type: 'object',
                        properties: {
                            $type: {
                                type: 'string'
                            },
                            workerID: {
                                type: 'string'
                            },
                            name: {
                                type: 'string'
                            }
                        },
                        required: ['$type'],
                        additionalProperties: true
                    }
                }
            ]
        });
    }

    addGlobal() {
        this.workflow = JSON.parse(this.workflowText);

        var name = 'key';
        var offset = 1;
        while (this.workflow.globals[name]) name = 'key' + offset++;
        this.workflow.globals[name] = 'value';
        this.workflowText = JSON.stringify(this.workflow, null, 4);
        this.service.add({ severity: 'info', summary: 'Info Message', detail: 'Global Added!' });
    }

    addActivity(activity : BaseActivity) {
        this.workflow = JSON.parse(this.workflowText);

        var name = 'activity';
        var offset = 1;
        while (this.workflow.activities.find((x) => x.name == name)) name = 'activity' + offset++;
        var toInsert : any = activity
        toInsert.name = name;
        if (toInsert.workerID == 'default')
            delete toInsert.workerID;
        this.workflow.activities.push(toInsert);
        this.workflowText = JSON.stringify(this.workflow, null, 4);
        this.service.add({ severity: 'info', summary: 'Info Message', detail: 'Activity Added!' });
    }

    async queueWorkflow(){
        await firstValueFrom(this.http.post("api/execute/queue", JSON.parse(this.workflowText)));
        this.service.add({ severity: 'success', summary: 'Workflow Started!', detail: 'The workflow have been started!' });
        await this.workflowStateService.Load();
    }

    copyContextToClipboard(context: BaseContext | undefined) {
        if (context) {
            var json = JSON.stringify(context, null, 4);
            navigator.clipboard.writeText(json);
            this.service.add({ severity: 'info', summary: 'Info Message', detail: 'Context copied to clipboard!' });
        }
    }
}

interface ConfigWorkersResult {
    type : string;
    id : string;
}

interface BaseContext {
    $type : string
}

interface BaseActivity {
    $type : string,
    name : string,
    workerID : string;
}
