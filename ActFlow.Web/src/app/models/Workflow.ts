export interface Workflow {
    name: string;
    retryBehaviour: number;
    globals : {[id:string]:string};
    activities : any[];
}
