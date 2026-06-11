import { WorkflowStatuses } from "./WorkflowStatuses";

export interface ListWorkflowState {
    id : string;
    name: string;
    status : WorkflowStatuses;
    startedAt : Date | null;
    endedAt : Date | null;
    isArchived : boolean;
}
