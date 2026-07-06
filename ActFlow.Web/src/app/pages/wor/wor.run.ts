import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, signal } from '@angular/core';
import { TuiButton, TuiNotificationService } from '@taiga-ui/core';
import { firstValueFrom } from 'rxjs';
import { Workflow } from '../../models/Workflow';
import { WorkflowEditor } from "./components/wor.components.workfloweditor";
import { WorkflowStateService } from './services/wor.stateservice';

@Component({
    selector: 'app-wor-run',
    imports: [
    CommonModule,
    WorkflowEditor,
	TuiButton
],
    template: `
        <span style="font-size:20;text-align:center">Run a workflow</span>

        <app-workflows-components-workfloweditor #editor [(workflow)]="workflow"/>

		<button tuiButton iconStart="list-start" size="s" (click)="editor.saveWorkflow();queueWorkflow()">Queue Workflow</button>

		<div></div>
    `,
    host:{
        class: 'base-view'
    }
})
export class WORRun {
    workflow = signal<Workflow>({ name: 'name', retryBehaviour: "None", globals: {}, activities: [] } as Workflow);

    constructor(private http : HttpClient, public service: TuiNotificationService, public workflowStateService : WorkflowStateService){}

    async queueWorkflow(){
        await firstValueFrom(this.http.post("api/execute/queue", this.workflow()));
		this.service.open("The workflow have been started!", {
			label: "Workflow Started",
			appearance: 'positive',
			icon:'info',
			autoClose: 1000
		}).subscribe();
        await this.workflowStateService.Load();
    }
}
