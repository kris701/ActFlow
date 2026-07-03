export interface Workflow {
    name: string;
    retryBehaviour: 'None' | 'Workflow' | "Activity";
    globals : {[id:string]:string};
    activities : any[];
}
