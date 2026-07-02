import { Routes } from '@angular/router';
import { AppLayout } from './layout/app.layout';
import { Status } from './pages/sta/sta';

export const appRoutes: Routes = [
    {
        path: '',
        component: AppLayout,
        providers: [
        ],
		children: [
			{ path: '', component: Status },
		]
    }
];
