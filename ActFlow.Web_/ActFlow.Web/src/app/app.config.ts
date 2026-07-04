import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideTaiga, tuiScrollbarOptionsProvider } from '@taiga-ui/core';
import { provideCharts, withDefaultRegisterables } from 'ng2-charts';
import { appRoutes } from './app.routes';
import { LayoutService } from './layout/services/layoutService';

import indexToPosition from 'index-to-position';
import * as monaco from 'monaco-editor';
import { NgxMonacoEditorConfig, provideMonacoEditor } from 'ngx-monaco-editor-v2';

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

export const appConfig: ApplicationConfig = {
  providers: [
		provideBrowserGlobalErrorListeners(),
		provideRouter(appRoutes),
		provideTaiga(),
		LayoutService,
		provideCharts(withDefaultRegisterables()),
		provideMonacoEditor(monacoConfig),
		tuiScrollbarOptionsProvider({mode: 'hover'}),
	],
};
