import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MessageService, TreeNode } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { FileUpload } from "primeng/fileupload";
import { PopoverModule } from 'primeng/popover';
import { TagModule } from 'primeng/tag';
import { Tooltip } from "primeng/tooltip";
import { TreeTableModule } from 'primeng/treetable';
import { firstValueFrom } from 'rxjs';
import { FloatTextInput } from "../../../../common/components/floattextinput";
import { DirectoryModel } from './models/DirectoryModel';
import { DirectoryRoot } from './models/DirectoryRoot';
import { FilesModel } from './models/FilesModel';

@Component({
    selector: 'app-fil-persistent',
    imports: [
    CommonModule,
    FormsModule,
    TreeTableModule,
    ButtonModule,
    TagModule,
    PopoverModule,
    FloatTextInput,
    FileUpload,
    Tooltip
],
    template: `
        <p-treetable [value]="files" [scrollable]="true">
            <ng-template #caption>
                <div class="flex items-center justify-between">
                    <p-button icon="pi pi-refresh" (onClick)="loadTree()"  pTooltip="Reload the file tree"/>
                    <p-button icon="pi pi-plus" (onClick)="createrootdirpop.show($event)"  pTooltip="Create a new root directory"/>
                    <p-button icon="pi pi-upload" (onClick)="uploadrootfilespop.show($event)"  pTooltip="Upload a file to the root directory"/>
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
                                <a [href]="'/api/fs/persistent/files' + '?path=' + rowData.path" [download]="rowData.name" (click)="$event.stopPropagation()">
                                    <p-tag class="h-full" icon="pi pi-download"></p-tag>
                                </a>
                                <p-button icon="pi pi-times" severity="danger" pTooltip="Delete this file" text (onClick)="deletefilepop.show($event)"/>

                                <p-popover #deletefilepop>
                                    <div class="flex flex-col gap-2">
                                        <span>Are you sure you want to delete this file?</span>
                                        <div class="flex flex-row gap-2">
                                            <p-button class="w-full" label="Yes" fluid severity="danger" (onClick)="deleteFile(rowData.path)"/>
                                            <p-button class="w-full" label="Cancel" fluid severity="info" (onClick)="deletefilepop.hide()"/>
                                        </div>
                                    </div>
                                </p-popover>
                            </div>
                        }
                        @else if(rowData.type == "dir"){
                            <div class="flex flex-row gap-2">
                                <p-button icon="pi pi-times" severity="danger" text pTooltip="Delete this directory" (onClick)="deletedirpop.show($event)"/>
                                <p-button icon="pi pi-plus" severity="info" text pTooltip="Create a subdirectory" (onClick)="createdirpop.show($event)"/>
                                <p-button icon="pi pi-upload" severity="success" text pTooltip="Upload a file to this directory" (onClick)="uploadfilespop.show($event)"/>

                                <p-popover #deletedirpop>
                                    <div class="flex flex-col gap-2">
                                        <span>Are you sure you want to delete this directory and all its content?</span>
                                        <div class="flex flex-row gap-2">
                                            <p-button class="w-full" label="Yes" fluid severity="danger" (onClick)="deleteDirectory(rowData.path)"/>
                                            <p-button class="w-full" label="Cancel" fluid severity="info" (onClick)="deletedirpop.hide()"/>
                                        </div>
                                    </div>
                                </p-popover>

                                <p-popover #createdirpop>
                                    <div class="flex flex-col gap-2">
                                        <span>Name of the new directory</span>
                                        <app-floattextinput #createdirname/>
                                        @if(createdirname.value){
                                            <p-button class="w-full" icon="pi pi-plus" label="Create" fluid severity="info" (onClick)="createDirectory(rowData.path, createdirname.value);createdirname.value = null"/>
                                        }
                                    </div>
                                </p-popover>

                                <p-popover #uploadfilespop>
                                    <div class="flex flex-col gap-2">
                                        <p-fileupload #fu mode="basic" chooseLabel="Choose" chooseIcon="pi pi-upload"/>
                                        @if(fu.hasFiles()){
                                            <p-button class="w-full" icon="pi pi-upload" label="Upload" fluid severity="info" (onClick)="uploadFiles(fu, rowData.path);fu.clear()"/>
                                        }
                                    </div>
                                </p-popover>
                            </div>
                        }
                    </td>
                </tr>
            </ng-template>
        </p-treetable>

        <p-popover #createrootdirpop>
            <div class="flex flex-col gap-2">
                <span>Name of the new directory</span>
                <app-floattextinput #createrootdirname/>
                @if(createrootdirname.value){
                    <p-button class="w-full" icon="pi pi-plus" label="Create" fluid severity="info" (onClick)="createDirectory('.', createrootdirname.value);createrootdirname.value = null"/>
                }
            </div>
        </p-popover>

        <p-popover #uploadrootfilespop>
            <div class="flex flex-col gap-2">
                <p-fileupload #rootfu mode="basic" chooseLabel="Choose" chooseIcon="pi pi-upload"/>
                @if(rootfu.hasFiles()){
                    <p-button class="w-full" icon="pi pi-upload" label="Upload" fluid severity="info" (onClick)="uploadFiles(rootfu, '.');rootfu.clear()"/>
                }
            </div>
        </p-popover>
    `,
    host:{
        class: 'card flex flex-col flex-grow gap-2'
    }
})
export class FilesPersistent {
    files: TreeNode[] = [];
    root : DirectoryRoot = { directories: [], files: [] } as DirectoryRoot
    highlightTarget : string | null = null;

    private route = inject(ActivatedRoute);
    constructor(private http : HttpClient, public service: MessageService, public router : Router){}

    async ngOnInit(){
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
        if (this.highlightTarget){
            this.router.navigate(["files/persistent"]);
            this.highlightTarget = null;
        }
        this.root = await firstValueFrom(this.http.get<DirectoryRoot>("/api/fs/persistent/root"))
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

    async createDirectory(path : string, name : string){
        await firstValueFrom(this.http.post("/api/fs/persistent/directory", { path: path, name: name }))
        await this.loadTree();
        this.service.add({ severity: 'success', summary: 'Directory Created!', detail: 'The target directory have been created' });
    }

    async deleteDirectory(path : string){
        await firstValueFrom(this.http.delete("/api/fs/persistent/directory", { params: { path: path }}))
        await this.loadTree();
        this.service.add({ severity: 'success', summary: 'Directory Deleted!', detail: 'The target directory have been deleted' });
    }

    async uploadFiles(fu : FileUpload, path : string){
        var files = fu.files;
        var file : File | null = null;
        if (files.length > 0)
            file = files[0];
        if (!file)
            return;

        const formData = new FormData();
        formData.append('path', path);
        formData.append('file', file);
        await firstValueFrom(this.http.post("/api/fs/persistent/files", formData));
        await this.loadTree();
        this.service.add({ severity: 'success', summary: 'File Uploaded!', detail: 'The target file have been uploaded' });
    }

    async deleteFile(path : string){
        await firstValueFrom(this.http.delete("/api/fs/persistent/files", { params: { path: path }}))
        await this.loadTree();
        this.service.add({ severity: 'success', summary: 'File Deleted!', detail: 'The target file have been deleted' });
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
