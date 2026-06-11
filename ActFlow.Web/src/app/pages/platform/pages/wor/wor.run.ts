import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { EditorComponent } from "ngx-monaco-editor-v2";
import { MessageService } from 'primeng/api';
import { Button } from "primeng/button";
import { firstValueFrom } from 'rxjs';
import { Workflow } from './../../../../models/Workflow';
import { WorkflowStateService } from './services/wor.stateservice';

@Component({
    selector: 'app-wor-run',
    imports: [
    CommonModule,
    FormsModule,
    EditorComponent,
    Button
],
    template: `
        <span style="font-size:20;text-align:center">Run a workflow</span>
        <div class="flex h-full flex-grow">
            <ngx-monaco-editor style="flex-grow:1" [options]="editorOptions" [(ngModel)]="workflowRun"> </ngx-monaco-editor>
        </div>
        <p-button icon="pi pi-bolt" label="Queue Workflow" fluid (onClick)="queueWorkflow()"/>
    `,
    host:{
        class: 'card flex flex-col flex-grow'
    },
    styles: `
        ngx-monaco-editor {
            height: 100%
        }
    `
})
export class WorkflowRun {
    editorOptions = {theme: 'vs-dark', language: 'json', automaticLayout: true};

    workflowRun : string = "";

    constructor(private http : HttpClient, public service: MessageService, public workflowStateService : WorkflowStateService){}

    ngOnInit(){
        var tmpTransfer = sessionStorage.getItem("tmpWorkflowTransfer");
        if (tmpTransfer)
        {
            this.workflowRun = tmpTransfer;
            sessionStorage.removeItem("tmpWorkflowTransfer");
        }
        else
            this.workflowRun = JSON.stringify({ name: 'name', retryBehaviour: 0, globals: {}, activities: [] } as Workflow, null, 4)
    }

    async queueWorkflow(){
        await firstValueFrom(this.http.post("api/queue", JSON.parse(this.workflowRun)));
        this.service.add({ severity: 'success', summary: 'Workflow Started!', detail: 'The workflow have been started!' });
        await this.workflowStateService.Load();
    }
}
