import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
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
import { FileHelpers } from './helpers/fil.helpers';
import { DirectoryRoot } from './models/DirectoryRoot';

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
        <p-treetable [value]="files()" [scrollable]="true" [loading]="isLoading()">
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
                            {{ FileHelpers.HumanFileSize(rowData.size) }}
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
    isLoading = signal<boolean>(false);
    files = signal<TreeNode[]>([]);
    root = signal<DirectoryRoot>({ directories: [], files: [] } as DirectoryRoot)
    highlightTarget = signal<string | null>(null);

    FileHelpers = FileHelpers;

    private route = inject(ActivatedRoute);
    constructor(private http : HttpClient, public service: MessageService, public router : Router){}

    async ngOnInit(){
        this.isLoading.set(true);
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
        var highlight = this.highlightTarget();
        if (highlight){
            this.router.navigate(["files/persistent"]);
            this.highlightTarget.set(null);
        }
        var root = await firstValueFrom(this.http.get<DirectoryRoot>("/api/fs/persistent/root"))
        var files : TreeNode[] = [];
        root.directories.forEach(x => files.push(FileHelpers.BuildTreeNodeDir(x)))
        root.files.forEach(x => files.push(FileHelpers.BuildTreeNodeFile(x)))
        this.root.set(root)
        this.files.set(files);
        this.isLoading.set(false);
    }

    async createDirectory(path : string, name : string){
        this.isLoading.set(true);
        await firstValueFrom(this.http.post("/api/fs/persistent/directory", { path: path, name: name }))
        await this.loadTree();
        this.service.add({ severity: 'success', summary: 'Directory Created!', detail: 'The target directory have been created' });
        this.isLoading.set(false);
    }

    async deleteDirectory(path : string){
        this.isLoading.set(true);
        await firstValueFrom(this.http.delete("/api/fs/persistent/directory", { params: { path: path }}))
        await this.loadTree();
        this.service.add({ severity: 'success', summary: 'Directory Deleted!', detail: 'The target directory have been deleted' });
        this.isLoading.set(false);
    }

    async uploadFiles(fu : FileUpload, path : string){
        this.isLoading.set(true);
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
        this.isLoading.set(false);
    }

    async deleteFile(path : string){
        this.isLoading.set(true);
        await firstValueFrom(this.http.delete("/api/fs/persistent/files", { params: { path: path }}))
        await this.loadTree();
        this.service.add({ severity: 'success', summary: 'File Deleted!', detail: 'The target file have been deleted' });
        this.isLoading.set(false);
    }
}
