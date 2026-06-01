import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideRouter, withEnabledBlockingInitialNavigation, withInMemoryScrolling, withViewTransitions } from '@angular/router';
import { definePreset } from '@primeuix/themes';
import Aura from '@primeuix/themes/aura';
import { provideMonacoEditor } from 'ngx-monaco-editor-v2';
import { ConfirmationService, MessageService } from 'primeng/api';
import { providePrimeNG } from 'primeng/config';
import { appRoutes } from './app.routes';
import { LayoutService } from './app/services/layoutService';
import * as Theme from "./theme.json";

const ThemePreset = definePreset(Aura, Theme);

export const appConfig: ApplicationConfig = {
    providers: [
        provideRouter(appRoutes, withInMemoryScrolling({ anchorScrolling: 'enabled', scrollPositionRestoration: 'enabled' }), withEnabledBlockingInitialNavigation(), withViewTransitions()),
        provideAnimationsAsync(),
        MessageService,
        ConfirmationService,
        providePrimeNG({
            ripple: false,
            theme: {
                preset: ThemePreset,
                options: {
                    darkModeSelector: '.dark'
                }
            }
        }),
        provideMonacoEditor(),
        provideZoneChangeDetection(),
        LayoutService
    ]
};
