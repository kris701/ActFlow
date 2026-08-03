import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { Routes } from '@angular/router';
import { appHttpInterceptor } from './app.http.interceptor.ts';
import { Notfound } from './pages/notfound/notfound';
import { Status } from './pages/sta/sta';

export const appRoutes: Routes = [
    {
        path: '',
        providers: [
			provideHttpClient(withInterceptors([appHttpInterceptor]))
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
