import { WorkflowLogTypes } from "./WorkflowLogTypes";

export interface WorkflowStateLog {
    logType : WorkflowLogTypes;
    text : string;
}
