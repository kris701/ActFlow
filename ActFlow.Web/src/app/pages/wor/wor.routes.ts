import { Routes } from '@angular/router';
import { WORResults } from './wor.results';
import { WORRun } from './wor.run';

export default [
    { path: 'results', component: WORResults },
    { path: 'run', component: WORRun }
] as Routes;
