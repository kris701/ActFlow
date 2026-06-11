import { Routes } from '@angular/router';
import { Results as WorkflowResults } from './wor.res';
import { WorkflowRun } from './wor.run';

export default [
    { path: 'results', component: WorkflowResults },
    { path: 'run', component: WorkflowRun }
] as Routes;
