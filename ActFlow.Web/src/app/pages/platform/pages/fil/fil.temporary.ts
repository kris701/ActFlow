import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MessageService, TreeNode } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { PopoverModule } from 'primeng/popover';
import { TagModule } from 'primeng/tag';
import { Tooltip } from "primeng/tooltip";
import { TreeTableModule } from 'primeng/treetable';
import { firstValueFrom } from 'rxjs';
import { DirectoryModel } from './models/DirectoryModel';
import { DirectoryRoot } from './models/DirectoryRoot';
import { FilesModel } from './models/FilesModel';

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
        @if(runId === null){
            <span style="align-self:center;text-align: center;height:100%;align-content:center;">
                No workflow run selected!
                Go to <b><a style="text-decoration: underline;" routerLink="/workflows/results">results</a></b>, find a workflow that you want to see the temporary files for and click the "View" button under files.
            </span>
        }
        @else {
            <span style="opacity:0.5;align-self:center">The temporary file viewer is in read only mode!</span>
            <p-treetable [value]="files" [scrollable]="true">
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
                                @if (rowData.path == this.highlightTarget){
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
                                {{ humanFileSize(rowData.size) }}
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
    files: TreeNode[] = [];
    root : DirectoryRoot = { directories: [], files: [] } as DirectoryRoot
    runId : string | null = null;
    highlightTarget : string | null = null;

    private route = inject(ActivatedRoute);
    constructor(private http : HttpClient, public service: MessageService, public router : Router){}

    async ngOnInit(){
        this.runId = this.route.snapshot.queryParamMap.get('id');
        if (this.runId === null)
            return;
        await this.loadTree();
        this.highlightTarget = this.route.snapshot.queryParamMap.get('path');
        if (this.highlightTarget){
            this.highlightTarget = this.highlightTarget.replaceAll('/', '\\');
            var cpy = [...this.files]
            this.expandToTarget(cpy, this.highlightTarget);
            this.files = cpy;
        }
    }

    async loadTree(){
        if (this.runId === null)
            return;
        if (this.highlightTarget){
            this.router.navigate(["files/temporary"], { queryParams: { id:this.runId } });
            this.highlightTarget = null;
        }
        this.root = await firstValueFrom(this.http.get<DirectoryRoot>("/api/fs/temporary/root", { params: { id:this.runId } }))
        var files : TreeNode[] = [];
        this.root.directories.forEach(x => files.push(this.buildTreeNodeDir(x)))
        this.root.files.forEach(x => files.push(this.buildTreeNodeFile(x)))
        this.files = files;
    }

    buildTreeNodeDir(dir : DirectoryModel) : TreeNode {
        var children : TreeNode[] = [];

        dir.directories.forEach(x => children.push(this.buildTreeNodeDir(x)))
        dir.files.forEach(x => children.push(this.buildTreeNodeFile(x)))

        return {
            label: dir.name,
            children: children,
            data: {
                type: "dir",
                name: dir.name,
                path: dir.path,
            }
        } as TreeNode
    }

    buildTreeNodeFile(file : FilesModel) : TreeNode {
        return {
            label: file.name,
            data: {
                type: "file",
                name: file.name,
                extension: file.extension,
                path: file.path,
                size: file.sizeB
            }
        } as TreeNode
    }

    //https://stackoverflow.com/a/14919494
    humanFileSize(bytes : number, si=false, dp=1) {
        const thresh = si ? 1000 : 1024;

        if (Math.abs(bytes) < thresh) {
            return bytes + ' B';
        }

        const units = si
            ? ['kB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB']
            : ['KiB', 'MiB', 'GiB', 'TiB', 'PiB', 'EiB', 'ZiB', 'YiB'];
        let u = -1;
        const r = 10**dp;

        do {
            bytes /= thresh;
            ++u;
        } while (Math.round(Math.abs(bytes) * r) / r >= thresh && u < units.length - 1);


        return bytes.toFixed(dp) + ' ' + units[u];
    }

    expandToTarget(nodes : TreeNode[], target : string){
        for(var node of nodes){
            if (node.data.type != "dir")
                continue;
            if (target.startsWith(node.data.path))
                node.expanded = true;
            if (node.children)
                this.expandToTarget(node.children, target);
        }
    }
}
