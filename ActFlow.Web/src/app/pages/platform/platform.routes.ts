import { provideHttpClient } from '@angular/common/http';
import { Routes } from '@angular/router';
import { AppLayout } from './layout/app.layout';
import { Status } from './pages/sta/sta';

export default [
    {
        path: '',
        component: AppLayout,
        providers: [provideHttpClient()],
        children: [
            { path: '', component: Status },
            { path: 'workflows', loadChildren: () => import('./pages/wor/wor.routes') }
        ]
    }
] as Routes;
