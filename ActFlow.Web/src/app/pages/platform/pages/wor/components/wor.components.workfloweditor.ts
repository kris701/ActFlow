import { HttpClient } from "@angular/common/http";
import { Component, EventEmitter, Input, OnChanges, Output, signal, SimpleChanges } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { EditorComponent } from "ngx-monaco-editor-v2";
import { MenuItem, MessageService } from "primeng/api";
import { BlockUIModule } from 'primeng/blockui';
import { Button } from "primeng/button";
import { InputGroup } from "primeng/inputgroup";
import { InputGroupAddon } from "primeng/inputgroupaddon";
import { Menubar } from "primeng/menubar";
import { MessageModule } from 'primeng/message';
import { PanelModule } from 'primeng/panel';
import { PopoverModule } from 'primeng/popover';
import { TagModule } from "primeng/tag";
import { firstValueFrom } from "rxjs";
import { variableNames } from "../../../../../../app.config";
import { Workflow } from "../../../../../models/Workflow";
import { WorkflowState } from "../../../../../models/WorkflowState";

@Component({
    selector: 'app-workflows-components-workfloweditor',
    imports: [
    Menubar,
    EditorComponent,
    FormsModule,
    InputGroup,
    Button,
    InputGroupAddon,
    MessageModule,
    TagModule,
    PopoverModule,
    BlockUIModule,
    PanelModule
],
    template: `
        @if(!disabled){
            <p-blockui [target]="menubar" [blocked]="!isWorkflowValid" />
            <p-panel #menubar class="menubarpanel">
                <p-menubar class="menubar" [model]="items()" [autoZIndex]="false" [baseZIndex]="1000" />
            </p-panel>

            <div class="flex flex-col h-full flex-grow">
                <ngx-monaco-editor class="actualeditor" style="flex-grow:1" [options]="editorOptions" [(ngModel)]="workflowText" (ngModelChange)="onInput($event)" (onInit)="setSchema()" [disabled]="disabled"> </ngx-monaco-editor>
            </div>

            <p-inputgroup class="w-full bottombar">
                @if(isSaveVisible()){
                    <p-inputgroup-addon class="w-full">
                        <p-button class="w-full h-full" [style]="{'width':'100%'}" fluid icon="pi pi-save" (onClick)="saveWorkflow()" />
                    </p-inputgroup-addon>
                }

                <p-inputgroup-addon class="w-full">
                    @if(isWorkflowValid()){
                        <p-tag severity="success">Parsed</p-tag>
                    }
                    @else {
                        <p-button class="w-full h-full" [style]="{'width':'100%'}" fluid severity="danger" (onClick)="errorPopover.show($event)">Invalid!</p-button>
                        <p-popover #errorPopover>
                            <p-message severity="error">{{invalidReason()}}</p-message>
                        </p-popover>
                    }
                </p-inputgroup-addon>

                <p-inputgroup-addon class="w-full">
                    <span>Workflow: <b>{{workflow.name}}</b></span>
                </p-inputgroup-addon>

                <p-inputgroup-addon class="w-full">
                    <span>Retry Behaviour: <b>{{workflow.retryBehaviour}}</b></span>
                </p-inputgroup-addon>

                <p-inputgroup-addon class="w-full">
                    <span>Globals: <b>{{Object.keys(workflow.globals).length}}</b></span>
                </p-inputgroup-addon>

                <p-inputgroup-addon class="w-full">
                    <span>activities: <b>{{workflow.activities.length}}</b></span>
                </p-inputgroup-addon>
            </p-inputgroup>
        }
        @else {
            <div class="flex flex-col h-full flex-grow">
                <ngx-monaco-editor class="readonlyeditor" style="flex-grow:1" [options]="editorOptions" [ngModel]="workflowText()" [disabled]="true"> </ngx-monaco-editor>
            </div>
        }
    `,
    host: {
        class: 'flex flex-col h-full',
        style: 'flex-grow:1'
    },
    styles: `
        ngx-monaco-editor {
            height: 100%;
        }

        ::ng-deep .actualeditor .editor-container {
            height: 100% !important;
            min-height:400px !important;

            border-color: var(--p-panel-border-color);
            border-right-width: 1px;
            border-left-width: 1px;
        }

        ::ng-deep .readonlyeditor .editor-container {
            height: 100% !important;
            min-height:400px !important;

            border: 1px solid var(--p-panel-border-color);
            border-radius: var(--p-panel-border-radius);

            clip-path: inset(0px round var(--p-panel-border-radius));
        }

        ::ng-deep .menubarpanel {
            border-bottom-width: 0px !important;
            border-bottom-right-radius: 0px !important;
            border-bottom-left-radius: 0px !important;
            padding: 0px !important;

            .p-panel-header {
                padding: 0px !important;
            }

            .p-panel-content {
                padding: 0px !important;
            }
        }

        ::ng-deep .menubar {
            border: 0px !important;
        }

        ::ng-deep .bottombar {
            > :first-child {
                border-top-left-radius: 0px !important;
            }

            > :last-child {
                border-top-right-radius: 0px !important;
            }
        }
    `
})
export class WorkflowEditor implements OnChanges {
    @Input() disabled: boolean = false;
    @Input() workflow: Workflow = {} as Workflow;
    @Input() workflowState: WorkflowState | undefined = undefined;
    @Output() workflowChange = new EventEmitter<Workflow>();
    @Output() workflowStateChange = new EventEmitter<WorkflowState | undefined>();

    Object = Object;

    editorOptions = { theme: 'vs-dark', language: 'json', automaticLayout: true, readOnly: false };
    workflowText = signal<string>('');
    isSaveVisible = signal<boolean>(false);
    isWorkflowValid = signal<boolean>(true);
    invalidReason = signal<string>("");

    items = signal<MenuItem[]>([]);

    workers = signal<ConfigWorkersResult[]>([])
    contexts = signal<BaseContext[]>([])
    activities = signal<BaseActivity[]>([])

    constructor(
        private service: MessageService,
        private http: HttpClient
    ) {
    }

    async ngOnInit(){
        var tmpTransfer = sessionStorage.getItem("tmpWorkflowTransfer");
        if (tmpTransfer)
        {
            this.workflowText.set(tmpTransfer);
            sessionStorage.removeItem("tmpWorkflowTransfer");
        }
        else
            this.workflowText.set(JSON.stringify(this.workflow, null, 4))

        var workers = await firstValueFrom(this.http.get<ConfigWorkersResult[]>("api/config/workers"));
        var contexts = await firstValueFrom(this.http.get<BaseContext[]>("api/config/contexts"));
        var activities = await firstValueFrom(this.http.get<BaseActivity[]>("api/config/activities"));

        var newItems : MenuItem[] = []
        newItems.push({
            label: 'Add Activity',
            items: activities.map(x => { return {
                label: x.$type + " (" + x.workerID + ")",
                command: () => this.addActivity(x)
            } as MenuItem })
        });
        newItems.push({
            label: 'Add Context',
            items: contexts.map(x => { return {
                label: x.$type,
                command: () => this.copyContextToClipboard(x, undefined)
            } as MenuItem })
        });
        newItems.push({
            label: 'Add Global',
            command: () => this.addGlobal()
        })
        newItems.push({
            label: 'Add Constant',
            items: [
                {
                    label: 'UTC Now (Datetime)',
                    command: () => this.copyContextToClipboard(undefined, "${{constants.utcnow}}")
                },
                {
                    label: 'UTC Now (Datetime) (File save)',
                    command: () => this.copyContextToClipboard(undefined, "${{constants.utcnow.safe}}")
                },
                {
                    label: 'Now (Datetime)',
                    command: () => this.copyContextToClipboard(undefined, "${{constants.now}}")
                },
                {
                    label: 'Now (Datetime) (File save)',
                    command: () => this.copyContextToClipboard(undefined, "${{constants.now.safe}}")
                },
                {
                    label: 'State ID',
                    command: () => this.copyContextToClipboard(undefined, "${{constants.stateid}}")
                },
            ]
        })
        this.items.set(newItems);

        this.workers.set(workers);
        this.contexts.set(contexts);
        this.activities.set(activities);

        this.loadGuidAliases();
    }

    ngOnChanges(changes: SimpleChanges): void {
        if (changes['script'] && changes['script'].currentValue != changes['script'].previousValue) {
            this.loadGuidAliases();
            this.workflowText.set(JSON.stringify(this.workflow, null, 2));
        }
        if (changes['scriptState'] && changes['scriptState'].currentValue != changes['scriptState'].previousValue) {
            this.loadGuidAliases();
        }
        if (changes['disabled'] && changes['disabled'].currentValue != changes['disabled'].previousValue) {
            this.editorOptions.readOnly = changes['disabled'].currentValue;
        }
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
                                enum: ['None', 'Workflow', 'Activity']
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

    loadGuidAliases() {
        variableNames.splice(0, variableNames.length);
        for (var key in this.workflow.globals)
            variableNames.push({
                id: key,
                name: this.workflow.globals[key],
                type: 'var'
            });
        if (this.workflowState) {
            for (var key in this.workflowState.contextStore) {
                variableNames.push({
                    id: key,
                    name: this.workflowState.contextStore[key],
                    type: 'var'
                });
            }
        }
    }

    addGlobal() {
        if (!this.saveWorkflow())
            return;

        var name = 'key';
        var offset = 1;
        while (this.workflow.globals[name]) name = 'key' + offset++;
        this.workflow.globals[name] = 'value';
        this.workflowText.set(JSON.stringify(this.workflow, null, 4));
        this.service.add({ severity: 'info', summary: 'Info Message', detail: 'Global Added!' });
    }

    addActivity(activity : BaseActivity) {
        if (!this.saveWorkflow())
            return;

        var name = 'activity';
        var offset = 1;
        while (this.workflow.activities.find((x) => x.name == name)) name = 'activity' + offset++;
        var toInsert : any = activity
        toInsert.name = name;
        if (toInsert.workerID == 'default')
            delete toInsert.workerID;
        this.workflow.activities.push(toInsert);
        this.workflowText.set(JSON.stringify(this.workflow, null, 4));
        this.service.add({ severity: 'info', summary: 'Info Message', detail: 'Activity Added!' });
    }

    copyContextToClipboard(context: BaseContext | undefined, str : string | undefined) {
        if (context) {
            var json = JSON.stringify(context, null, 4);
            navigator.clipboard.writeText(json);
            this.service.add({ severity: 'info', summary: 'Info Message', detail: 'Text copied to clipboard!' });
        }
        else if (str){
            navigator.clipboard.writeText(str);
            this.service.add({ severity: 'info', summary: 'Info Message', detail: 'Text copied to clipboard!' });
        }
    }

    onInput(event : any){
        this.isSaveVisible.set(true);
    }

    public saveWorkflow() : boolean {
        try{
            this.workflow = JSON.parse(this.workflowText());
            this.workflowChange.emit(this.workflow);
            this.service.add({ severity: 'success', summary: 'Success Message', detail: 'Workflow Saved!' });

            this.loadGuidAliases();
            this.isSaveVisible.set(false);
            this.isWorkflowValid.set(true);
            this.invalidReason.set("");
            return true;
        }
        catch({ name, message } : any){
            this.isWorkflowValid.set(false);
            this.invalidReason.set(name + ": " + message)
        }
        return false;
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
