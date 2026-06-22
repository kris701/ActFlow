import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { Routes } from '@angular/router';
import { httpInterceptor } from './http.interceptor.ts';
import { AppLayout } from './layout/app.layout';
import { Status } from './pages/sta/sta';
import { WorkflowStateService } from './pages/wor/services/wor.stateservice';

export default [
    {
        path: '',
        component: AppLayout,
        providers: [provideHttpClient(withInterceptors([httpInterceptor])), WorkflowStateService],
        children: [
            { path: '', component: Status },
            { path: 'workflows', loadChildren: () => import('./pages/wor/wor.routes') },
            { path: 'files', loadChildren: () => import('./pages/fil/fil.routes') },
        ]
    }
] as Routes;
