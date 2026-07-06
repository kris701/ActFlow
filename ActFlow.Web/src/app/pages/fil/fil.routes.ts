import { Routes } from '@angular/router';
import { FILPersistent } from './fil.persistent';
import { FILTemporary } from './fil.temporary';


export default [
    { path: 'persistent', component: FILPersistent },
    { path: 'temporary', component: FILTemporary },
] as Routes;
