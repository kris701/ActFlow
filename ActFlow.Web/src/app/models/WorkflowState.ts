import { Workflow } from "./Workflow";
import { WorkflowStateLog } from "./WorkflowStateLog";
import { WorkflowStatuses } from "./WorkflowStatuses";

export interface WorkflowState {
    id : string;
    name: string;
    status : WorkflowStatuses;
    activityIndex: number;
    contextStore : {[id:string]:string};
    startedAt : Date | null;
    endedAt : Date | null;
    logText : WorkflowStateLog[];
    workflow : Workflow;
    sourceWorkflow : Workflow;
}
