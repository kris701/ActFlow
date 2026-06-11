import { provideHttpClient } from '@angular/common/http';
import { Routes } from '@angular/router';
import { AppLayout } from './layout/app.layout';
import { Status } from './pages/sta/sta';
import { WorkflowStateService } from './pages/wor/services/wor.stateservice';

export default [
    {
        path: '',
        component: AppLayout,
        providers: [provideHttpClient(), WorkflowStateService],
        children: [
            { path: '', component: Status },
            { path: 'workflows', loadChildren: () => import('./pages/wor/wor.routes') }
        ]
    }
] as Routes;
