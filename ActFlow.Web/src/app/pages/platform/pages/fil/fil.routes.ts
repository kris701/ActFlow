import { Routes } from '@angular/router';
import { FilesPersistent } from './fil.persistent';
import { FilesTemporary } from './fil.temporary';


export default [
    { path: 'persistent', component: FilesPersistent },
    { path: 'temporary', component: FilesTemporary },
] as Routes;
