import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { Routes } from '@angular/router';
import { appHttpInterceptor } from './app.http.interceptor.ts';
import { AppLayout } from './layout/app.layout';
import { Notfound } from './pages/notfound/notfound';
import { Status } from './pages/sta/sta';
import { WorkflowStateService } from './pages/wor/services/wor.stateservice';

export const appRoutes: Routes = [
    {
        path: '',
        component: AppLayout,
        providers: [
			provideHttpClient(withInterceptors([appHttpInterceptor])),
			WorkflowStateService
        ],
		children: [
			{ path: '', component: Status },
			{ path: 'workflows', loadChildren: () => import('./pages/wor/wor.routes') },
			{ path: 'files', loadChildren: () => import('./pages/fil/fil.routes') },
		]
    },
	{ path: 'notfound', component: Notfound },
    { path: '**', redirectTo: '/notfound' }
];
