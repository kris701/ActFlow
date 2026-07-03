import { Injectable } from "@angular/core";
import { BaseListService } from "../../../common/interfaces/baseListService";
import { ListWorkflowState } from "../../../models/ListWorkflowState";
import { WorkflowState } from "../../../models/WorkflowState";

@Injectable()
export class WorkflowStateService extends BaseListService<WorkflowState, ListWorkflowState> {
    override getAllEndpoint: string = "/api/results/all";
    override getEndpoint: string = "/api/results";
}
