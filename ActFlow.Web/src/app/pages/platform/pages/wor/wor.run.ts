import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { Button } from "primeng/button";
import { firstValueFrom } from 'rxjs';
import { Workflow } from './../../../../models/Workflow';
import { WorkflowEditor } from "./components/wor.components.workfloweditor";
import { WorkflowStateService } from './services/wor.stateservice';

@Component({
    selector: 'app-wor-run',
    imports: [
    CommonModule,
    FormsModule,
    Button,
    WorkflowEditor
],
    template: `
        <span style="font-size:20;text-align:center">Run a workflow</span>

        <app-workflows-components-workfloweditor #editor [(workflow)]="workflow"/>

        <p-button icon="pi pi-bolt" label="Queue Workflow" fluid (onClick)="editor.saveWorkflow();queueWorkflow()"/>
    `,
    host:{
        class: 'card flex flex-col flex-grow gap-2'
    }
})
export class WorkflowRun {
    workflow : Workflow = { name: 'name', retryBehaviour: "None", globals: {}, activities: [] } as Workflow;

    constructor(private http : HttpClient, public service: MessageService, public workflowStateService : WorkflowStateService){}

    async queueWorkflow(){
        await firstValueFrom(this.http.post("api/execute/queue", this.workflow));
        this.service.add({ severity: 'success', summary: 'Workflow Started!', detail: 'The workflow have been started!' });
        await this.workflowStateService.Load();
    }
}
