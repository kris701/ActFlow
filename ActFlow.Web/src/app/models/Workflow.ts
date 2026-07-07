export interface Workflow {
    name: string;
    retryBehaviour: 'None' | 'Workflow' | "Activity";
    completionBehaviour: 'None' | 'ReQueue';
    globals : {[id:string]:string};
    activities : any[];
}
