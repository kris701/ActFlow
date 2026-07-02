import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MessageService, TreeNode } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { PopoverModule } from 'primeng/popover';
import { TagModule } from 'primeng/tag';
import { Tooltip } from "primeng/tooltip";
import { TreeTableModule } from 'primeng/treetable';
import { firstValueFrom } from 'rxjs';
import { FileHelpers } from './helpers/fil.helpers';
import { DirectoryRoot } from './models/DirectoryRoot';

@Component({
    selector: 'app-fil-temporary',
    imports: [
    CommonModule,
    FormsModule,
    TreeTableModule,
    ButtonModule,
    TagModule,
    PopoverModule,
    Tooltip,
    RouterLink
],
    template: `
        @if(runId() === null){
            <span style="align-self:center;text-align: center;height:100%;align-content:center;">
                No workflow run selected!
                Go to <b><a style="text-decoration: underline;" routerLink="/workflows/results">results</a></b>, find a workflow that you want to see the temporary files for and click the "View" button under files.
            </span>
        }
        @else {
            <span style="opacity:0.5;align-self:center">The temporary file viewer is in read only mode!</span>
            <p-treetable [value]="files()" [scrollable]="true" [loading]="isLoading()">
                <ng-template #caption>
                    <div class="flex items-center justify-between">
                        <p-button icon="pi pi-refresh" (onClick)="loadTree()"  pTooltip="Reload the file tree"/>
                    </div>
                </ng-template>
                <ng-template #header>
                    <tr>
                        <th style="width:50%">Name</th>
                        <th>Type</th>
                        <th>Size</th>
                        <th></th>
                    </tr>
                </ng-template>
                <ng-template #body let-rowNode let-rowData="rowData">
                    <tr [ttRow]="rowNode">
                        <td style="width:50%">
                            <div class="flex items-center gap-2">
                                <p-treetable-toggler [rowNode]="rowNode" />
                                @if (rowData.path == this.highlightTarget()){
                                    <p-tag icon="pi pi-arrow-right"></p-tag>
                                    <span style="font-weight: bold;">{{ rowData.name }}</span>
                                }
                                @else {
                                    <span>{{ rowData.name }}</span>
                                }
                            </div>
                        </td>
                        <td>
                            @if(rowData.type == "dir"){
                                <span style="opacity:0.5;font-style: italic;">Directory</span>
                            }
                            @else {
                                <p-tag>{{ rowData.extension }}</p-tag>
                            }
                        </td>
                        <td>
                            @if(rowData.size){
                                {{ FileHelpers.HumanFileSize(rowData.size) }}
                            }
                        </td>
                        <td>
                            @if(rowData.type == "file"){
                                <div class="flex flex-row gap-2">
                                    <a [href]="'/api/fs/temporary/files' + '?id=' + runId + '&path=' + rowData.path" [download]="rowData.name" (click)="$event.stopPropagation()">
                                        <p-tag class="h-full" icon="pi pi-download"></p-tag>
                                    </a>
                                </div>
                            }
                        </td>
                    </tr>
                </ng-template>
            </p-treetable>
        }
    `,
    host:{
        class: 'card flex flex-col flex-grow gap-2'
    }
})
export class FilesTemporary {
    isLoading = signal<boolean>(false);
    files = signal<TreeNode[]>([]);
    root = signal<DirectoryRoot>({ directories: [], files: [] } as DirectoryRoot)
    runId = signal<string | null>(null);
    highlightTarget = signal<string | null>(null);

    FileHelpers = FileHelpers;

    private route = inject(ActivatedRoute);
    constructor(private http : HttpClient, public service: MessageService, public router : Router){}

    async ngOnInit(){
        this.isLoading.set(true);
        this.runId.set(this.route.snapshot.queryParamMap.get('id'));
        if (this.runId() === null)
            return;
        await this.loadTree();
        var highlight = this.route.snapshot.queryParamMap.get('path');
        if (highlight){
            highlight = highlight.replaceAll('/', '\\');
            var cpy = [...this.files()]
            FileHelpers.ExpandToTarget(cpy, highlight);
            this.files.set(cpy);
        }
        this.highlightTarget.set(highlight);
        this.isLoading.set(false);
    }

    async loadTree(){
        this.isLoading.set(true);
        var runId = this.runId();
        if (runId === null)
            return;
        var highlight = this.highlightTarget();
        if (highlight){
            this.router.navigate(["files/temporary"], { queryParams: { id:runId } });
            this.highlightTarget.set(null);
        }
        var root = await firstValueFrom(this.http.get<DirectoryRoot>("/api/fs/temporary/root", { params: { id:runId } }))
        var files : TreeNode[] = [];
        root.directories.forEach(x => files.push(FileHelpers.BuildTreeNodeDir(x)))
        root.files.forEach(x => files.push(FileHelpers.BuildTreeNodeFile(x)))
        this.root.set(root)
        this.files.set(files);
        this.isLoading.set(false);
    }
}
