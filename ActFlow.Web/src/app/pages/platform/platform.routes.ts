import { provideHttpClient } from '@angular/common/http';
import { Routes } from '@angular/router';
import { AppLayout } from './layout/app.layout';
import { Dashboard } from './pages/dsh/dsh';

export default [
    {
        path: '',
        component: AppLayout,
        providers: [provideHttpClient()],
        children: [
            { path: '', component: Dashboard },
        ]
    }
] as Routes;
