import { HttpClient } from "@angular/common/http";
import { Component, EventEmitter, Input, OnChanges, Output, signal, SimpleChanges } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { TuiButton, TuiDataList, TuiDropdown, TuiDropdownHover, TuiGroup, TuiIcon, TuiNotificationService } from "@taiga-ui/core";
import { TuiChevron, TuiChip, TuiTooltip } from "@taiga-ui/kit";
import { EditorComponent } from "ngx-monaco-editor-v2";
import { firstValueFrom } from "rxjs";
import { variableNames } from "../../../app.config";
import { Workflow } from "../../../models/Workflow";
import { WorkflowState } from "../../../models/WorkflowState";

@Component({
    selector: 'app-workflows-components-workfloweditor',
    imports: [
    FormsModule,
    EditorComponent,
    TuiChip,
    TuiButton,
    TuiGroup,
    TuiChevron,
    TuiDropdown,
    TuiDataList,
    TuiDropdownHover,
    TuiIcon,
	TuiTooltip
],
    template: `
        @if(!disabled){
			@if(isWorkflowValid()){
				<div
					tuiGroup
					[collapsed]="false"
					[rounded]="true"
					class="topbar w-full"
					>
					@for(item of items(); track item){
						@if(item.items){
							<button
								tuiButton
								tuiChevron
								tuiDropdownHover
								type="button"
								size="m"
								[iconStart]="item.icon"
								[tuiDropdown]="content"
								#parent
							>
								{{item.label}}
								<ng-template #content>
									<tui-data-list>
										@for(subitem of item.items; track subitem){
											<button tuiOption size="m" class="w-full h-full" [iconStart]="subitem.icon" (click)="subitem.command()">{{subitem.label}}</button>
										}
									</tui-data-list>
								</ng-template>
							</button>
						}
						@else {
							<button tuiButton size="m" class="w-full h-full" [iconStart]="item.icon" (click)="item.command()">{{item.label}}</button>
						}
					}
				</div>
			}

            <div class="flex flex-col h-full flex-grow">
                <ngx-monaco-editor class="actualeditor" style="flex-grow:1" [options]="editorOptions" [(ngModel)]="workflowText" (ngModelChange)="onInput($event)" (onInit)="setSchema()" [disabled]="disabled"> </ngx-monaco-editor>
            </div>

			<div
				tuiGroup
				[collapsed]="false"
				[rounded]="true"
				class="bottombar w-full"
				>
                @if(isSaveVisible()){
                    <button tuiButton size="m" class="w-full h-full" iconStart="save" (click)="saveWorkflow()"></button>
                }

				@if(isWorkflowValid()){
					<span class="h-full" size="m" appearance="positive" tuiChip>Parsed</span>
				}
				@else {
					<span class="h-full" size="m" appearance="negative" tuiChip>
						Invalid!
						<tui-icon
							tuiHintDirection="end"
							[tuiTooltip]="invalidReason()"
						/>
					</span>
				}

				<span class="h-full" tuiChip>Workflow: <b>{{workflow.name}}</b></span>
				<span class="h-full" tuiChip>Retry Behaviour: <b>{{workflow.retryBehaviour}}</b></span>
				<span class="h-full" tuiChip>Globals: <b>{{Object.keys(workflow.globals).length}}</b></span>
				<span class="h-full" tuiChip>Activities: <b>{{workflow.activities.length}}</b></span>
            </div>
        }
        @else {
            <div class="flex flex-col h-full flex-grow">
                <ngx-monaco-editor class="readonlyeditor" style="flex-grow:1" [options]="editorOptions" [ngModel]="workflowText()" [disabled]="true"> </ngx-monaco-editor>
            </div>
        }
    `,
    host: {
        class: 'flex flex-col h-full m-2',
    },
    styles: `
        ::ng-deep .actualeditor .editor-container {
            height: 100% !important;
        }

        ::ng-deep .topbar {
            > :first-child {
                border-bottom-left-radius: 0px !important;
            }

            > :last-child {
                border-bottom-right-radius: 0px !important;
            }
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

    editorOptions = { theme: 'vs-dark', language: 'json', automaticLayout: false, readOnly: false };
    workflowText = signal<string>('');
    isSaveVisible = signal<boolean>(false);
    isWorkflowValid = signal<boolean>(true);
    invalidReason = signal<string>("");

    items = signal<MenuItem[]>([]);

    workers = signal<ConfigWorkersResult[]>([])
    contexts = signal<BaseContext[]>([])
    activities = signal<BaseActivity[]>([])

    constructor(
        private service: TuiNotificationService,
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
        } as MenuItem);
        newItems.push({
            label: 'Add Context',
            items: contexts.map(x => { return {
                label: x.$type,
                command: () => this.copyContextToClipboard(x, undefined)
            } as MenuItem })
        } as MenuItem);
        newItems.push({
            label: 'Add Global',
            command: () => this.addGlobal()
        } as MenuItem)
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
        } as MenuItem)
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
							completionBehaviour: {
                                enum: ['None', 'ReQueue']
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
		this.service.open("Global added", {
			label: "Global Added",
			appearance: 'info',
			icon:'info',
			autoClose: 1000
		}).subscribe();
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
		this.service.open("Activity added", {
			label: "Activity Added",
			appearance: 'info',
			icon:'info',
			autoClose: 1000
		}).subscribe();
    }

    copyContextToClipboard(context: BaseContext | undefined, str : string | undefined) {
        if (context) {
            var json = JSON.stringify(context, null, 4);
            navigator.clipboard.writeText(json);
			this.service.open("Text copied to clipboard!", {
				label: "Text Copied",
				appearance: 'info',
				icon:'info',
				autoClose: 1000
			}).subscribe();
        }
        else if (str){
            navigator.clipboard.writeText(str);
			this.service.open("Text copied to clipboard!", {
				label: "Text Copied",
				appearance: 'info',
				icon:'info',
				autoClose: 1000
			}).subscribe();
        }
    }

    onInput(event : any){
        this.isSaveVisible.set(true);
    }

    public saveWorkflow() : boolean {
        try{
            this.workflow = JSON.parse(this.workflowText());
            this.workflowChange.emit(this.workflow);
			this.service.open("Workflow saved!", {
				label: "Saved",
				appearance: 'positive',
				icon:'info',
				autoClose: 1000
			}).subscribe();

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

interface MenuItem {
	label: string,
	icon: string | null,
	command() : void;
	items: MenuItem[] | null
}
