import { Injectable } from "@angular/core";
import { BaseListInterface } from "@kris701/ez-ui";
import { ListWorkflowState } from "../../../models/ListWorkflowState";
import { WorkflowState } from "../../../models/WorkflowState";

@Injectable()
export class WorkflowStateService extends BaseListInterface<WorkflowState, ListWorkflowState> {
    override getAllEndpoint: string = "/api/results/all";
    override getEndpoint: string = "/api/results";
}
