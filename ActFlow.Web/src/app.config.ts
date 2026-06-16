import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideRouter, withEnabledBlockingInitialNavigation, withInMemoryScrolling, withViewTransitions } from '@angular/router';
import { definePreset } from '@primeuix/themes';
import Aura from '@primeuix/themes/aura';
import indexToPosition from 'index-to-position';
import * as monaco from 'monaco-editor';
import { NgxMonacoEditorConfig, provideMonacoEditor } from 'ngx-monaco-editor-v2';
import { ConfirmationService, MessageService } from 'primeng/api';
import { providePrimeNG } from 'primeng/config';
import { appRoutes } from './app.routes';
import { LayoutService } from './app/services/layoutService';
import * as Theme from "./theme.json";

export var variableNames: { id: string; name: string; type: string }[] = [];

function createDependencyProposals(range: any) {
    var proposals = variableNames.forEach((x) => {
        return {
            label: x.id,
            kind: monaco.languages.CompletionItemKind.Reference,
            insertText: '"${{' + x.id + '}}"',
            range: range
        };
    });
    return proposals;
}

function onMonacoLoad() {
    (window as any).monaco.languages.registerCompletionItemProvider('json', {
        provideCompletionItems: function (model: any, position: any) {
            var word = model.getWordUntilPosition(position);
            var range = {
                startLineNumber: position.lineNumber,
                endLineNumber: position.lineNumber,
                startColumn: word.startColumn,
                endColumn: word.endColumn
            };
            return {
                suggestions: createDependencyProposals(range)
            };
        }
    });

    (window as any).monaco.languages.registerInlayHintsProvider('json', {
        provideInlayHints(model: any, range: any, token: any) {
            var hints: any[] = [];
            var text = <string>model.getValue();
            variableNames.forEach((x) => {
                var targetStr = '${{' + x.id + '}}';
                var index = text.indexOf(targetStr);
                while (index != -1) {
                    var pos = indexToPosition(text, index);
                    hints.push({
                        kind: monaco.languages.InlayHintKind.Type,
                        position: { column: pos.column + x.id.length + 6, lineNumber: pos.line + 1 },
                        label: ` : ` + x.name
                    });
                    index = text.indexOf(targetStr, index + 1);
                }
            });

            return {
                hints: hints,
                dispose: () => {}
            };
        }
    });
}

const monacoConfig: NgxMonacoEditorConfig = {
    baseUrl: window.location.origin + '/assets/monaco/min/vs',
    onMonacoLoad
};


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
        provideMonacoEditor(monacoConfig),
        provideZoneChangeDetection(),
        LayoutService
    ]
};
