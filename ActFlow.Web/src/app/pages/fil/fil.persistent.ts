import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TuiHandler } from '@taiga-ui/cdk/types';
import { TuiButton, TuiDropdown, TuiHint, TuiIcon, TuiInput, TuiLoader, TuiNotificationService, TuiTextfieldComponent } from '@taiga-ui/core';
import { TuiChip, TuiTree } from "@taiga-ui/kit";
import { TuiBlockStatusComponent } from "@taiga-ui/layout";
import { firstValueFrom } from 'rxjs';
import FloatFileUpload from '../../common/components/floatfileupload';
import { FileHelpers } from './Helpers/fil.helpers';
import { DirectoryRoot } from './Models/DirectoryRoot';
import { TreeNode } from './Models/TreeNode';

@Component({
    selector: 'app-fil-persistent',
    imports: [
    CommonModule,
    TuiTree,
    TuiChip,
    TuiIcon,
    TuiBlockStatusComponent,
    TuiLoader,
    TuiButton,
    TuiDropdown,
    TuiTextfieldComponent,
    TuiInput,
    FloatFileUpload,
	TuiHint
],
    template: `
		<tui-loader [inheritColor]="true" [overlay]="true" size="xxl" [loading]="isLoading()">
			<div class="flex flex-row gap-2">
				<button
					tuiButton
					iconStart="rotate-cw"
					size="s"
					appearance="secondary"
					(click)="loadTree()"
				></button>
				<button
					tuiButton
					iconStart="plus"
					size="s"
					appearance="secondary"
					[tuiDropdown]="createdirpop"
					[tuiDropdownOpen]="createdirpopPath() == '.'"
					(click)="createdirpopPath.set('.')"
				></button>
				<button
					tuiButton
					iconStart="upload"
					size="s"
					appearance="secondary"
					[tuiDropdown]="uploadfilespop"
					[tuiDropdownOpen]="uploadfilespopPath() == '.'"
					(click)="uploadfilespopPath.set('.')"
				></button>
			</div>

			@let highlightTarget = this.highlightTarget();
			@let data = files();
			@if(data.length == 0){
				<tui-block-status>
					<tui-icon tuiSlot="top" icon="grid-2x2-x" />

					<h3>No Data</h3>

					No data to display.
				</tui-block-status>
			}
			@else {
				@for (file of data; track file.data.path) {
					<tui-tree
						[childrenHandler]="handler"
						[content]="content"
						[tuiTreeController]="false"
						[value]="file"
						[map]="map()"
					/>
				}

				<ng-template
					#content
					let-item
				>
					@if(item.data.type == 'dir'){
						<tui-icon style="margin-right:10px" icon="folder"/>
					}
					@else{
						<tui-icon style="margin-right:10px" icon="file"/>
					}

					@if (item.data.path == highlightTarget){
						<span tuiChip iconStart="move-right"></span>
						<span style="font-weight: bold;">{{ item.label }}</span>
					}
					@else {
						<span>{{ item.label }}</span>
					}
					<div class="fileAction">
						@if(item.data.type == 'dir'){
							<span class="item" tuiChip size="xs"><i style="opacity:0.5">Directory</i></span>
							<span class="item" tuiChip size="xs">{{FileHelpers.HumanFileSize(item.data.size)}}</span>
							<span class="item flex flex-row gap-2">
								<button
									tuiButton
									iconStart="plus"
									size="xs"
									appearance="secondary"
									[tuiDropdown]="createdirpop"
									[tuiDropdownOpen]="createdirpopPath() == item.data.path"
									(click)="createdirpopPath.set(item.data.path)"
									tuiHint="Create a new directory"
								></button>
								<button
									tuiButton size="xs"
									iconStart="x"
									tuiAppearanceMode="invalid"
									appearance="secondary"
									[tuiDropdown]="deletedirpop"
									[tuiDropdownOpen]="deletedirpopPath() == item.data.path"
									(click)="deletedirpopPath.set(item.data.path)"
									tuiHint="Delete Directory"
								></button>
								<button
									tuiButton size="xs"
									iconStart="upload"
									appearance="secondary"
									[tuiDropdown]="uploadfilespop"
									[tuiDropdownOpen]="uploadfilespopPath() == item.data.path"
									(click)="uploadfilespopPath.set(item.data.path)"
									tuiHint="Upload file"
								></button>
							</span>
						}
						@else {
							<span class="item" tuiChip size="xs">{{item.data.extension}}</span>
							<span class="item" tuiChip size="xs">{{FileHelpers.HumanFileSize(item.data.size)}}</span>
							<span class="item flex flex-row gap-2">
								<a [href]="'/api/fs/persistent/files' + '?path=' + item.data.path" [download]="item.data.name" (click)="$event.stopPropagation()">
                                    <tui-icon icon="download" tuiHint="Download"/>
                                </a>
								<button
									tuiButton size="xs"
									iconStart="x"
									tuiAppearanceMode="invalid"
									appearance="secondary"
									[tuiDropdown]="deletefilepop"
									[tuiDropdownOpen]="deletefilepopPath() == item.data.path"
									(click)="deletefilepopPath.set(item.data.path)"
									tuiHint="Delete file"
								></button>
							</span>
						}
					</div>
				</ng-template>
			}
		</tui-loader>

		<ng-template #createdirpop>
            <div class="flex flex-col gap-2 p-4" style="width:40vw">
                <span>Name of the new directory</span>
				<tui-textfield iconEnd="folder-pen">
					<input #createDirInput tuiInput />
				</tui-textfield>
                @if(createDirInput.value){
                    <button tuiButton iconStart="plus" (click)="createDirectory(createdirpopPath(), createDirInput.value);createDirInput.value = '';createdirpopPath.set('')">Create</button>
                }
            </div>
		</ng-template>

		<ng-template #uploadfilespop>
			<div class="flex flex-col gap-2 p-4" style="width:40vw">
                <app-floatfileupload #fu/>
                @if(fu.files().length > 0){
                    <button tuiButton iconStart="upload" (click)="uploadFiles(fu, uploadfilespopPath());fu.clear();uploadfilespopPath.set('')">Upload</button>
                }
            </div>
		</ng-template>

		<ng-template #deletefilepop>
			<div class="flex flex-col gap-2 p-4">
				<span>Are you sure you want to delete this file?</span>
				<div class="flex flex-row gap-2" style="justify-content:center">
					<button tuiButton tuiAppearanceMode="checked invalid" appearance="outline" (click)="deleteFile(deletefilepopPath())">Yes</button>
					<button tuiButton appearance="outline" (click)="deletefilepopPath.set('')">No</button>
				</div>
			</div>
		</ng-template>

		<ng-template #deletedirpop>
			<div class="flex flex-col gap-2 p-4">
				<span>Are you sure you want to delete this directory and all its content?</span>
				<div class="flex flex-row gap-2" style="justify-content:center">
					<button tuiButton tuiAppearanceMode="checked invalid" appearance="outline" (click)="deleteDirectory(deletedirpopPath())">Yes</button>
					<button tuiButton appearance="outline" (click)="deletedirpopPath.set('')">No</button>
				</div>
			</div>
		</ng-template>
    `,
    host:{
        class: 'base-view'
    },
	styles: `
		.fileAction {
			display: flex;
			justify-content: end;
			flex-grow:1;
			gap:2rem;
			margin-top: 5px;

			.item {
				justify-content: center;
				width:7rem;
			}
		}
	`
})
export class FILPersistent {
    isLoading = signal<boolean>(false);
    files = signal<TreeNode[]>([]);
    highlightTarget = signal<string | null>(null);

	createdirpopPath = signal<string>("");
	uploadfilespopPath = signal<string>("");
	deletefilepopPath = signal<string>("");
	deletedirpopPath = signal<string>("");

    FileHelpers = FileHelpers;

	map = signal<Map<TreeNode, boolean>>(new Map<TreeNode, boolean>());

    private route = inject(ActivatedRoute);
    constructor(private http : HttpClient, public service: TuiNotificationService, public router : Router){}

	protected readonly handler: TuiHandler<TreeNode, readonly TreeNode[]> = (item) =>
		item.children || [];

    async ngOnInit(){
        this.isLoading.set(true);
        await this.loadTree();
        var highlight = this.route.snapshot.queryParamMap.get('path');
        if (highlight){
            highlight = highlight.replaceAll('/', '\\');
			this.highlightTarget.set(highlight);
			setTimeout(() => {
				var map = FileHelpers.ExpandToTarget(this.files(), this.highlightTarget()!, new Map<TreeNode, boolean>());
				this.map.set(map);
			}, 500);
        }
        this.isLoading.set(false);
    }

    async loadTree(){
        this.isLoading.set(true);
		this.map.set(new Map<TreeNode, boolean>());
        var highlight = this.highlightTarget();
        if (highlight){
            this.router.navigate(["files/persistent"]);
            this.highlightTarget.set(null);
        }
        var root = await firstValueFrom(this.http.get<DirectoryRoot>("/api/fs/persistent/root"))
        var files : TreeNode[] = [];
        root.directories.forEach(x => files.push(FileHelpers.BuildTreeNodeDir(x)))
        root.files.forEach(x => files.push(FileHelpers.BuildTreeNodeFile(x)))
        this.files.set(files);
        this.isLoading.set(false);
    }

    async createDirectory(path : string, name : string){
        this.isLoading.set(true);
        await firstValueFrom(this.http.post("/api/fs/persistent/directory", { path: path, name: name }))
        await this.loadTree();
		this.service.open("The target directory have been created", {
			label: "Directory Created",
			appearance: 'info',
			icon:'info',
			autoClose: 1000
		}).subscribe();
        this.isLoading.set(false);
    }

    async deleteDirectory(path : string){
        this.isLoading.set(true);
        await firstValueFrom(this.http.delete("/api/fs/persistent/directory", { params: { path: path }}))
        await this.loadTree();
		this.service.open("The target directory have been deleted", {
			label: "Directory Deleted",
			appearance: 'info',
			icon:'info',
			autoClose: 1000
		}).subscribe();
        this.isLoading.set(false);
    }

    async uploadFiles(fu : FloatFileUpload, path : string){
        this.isLoading.set(true);
        var files = fu.files();
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
		this.service.open("The target file have been uploaded", {
			label: "File Uploaded",
			appearance: 'info',
			icon:'info',
			autoClose: 1000
		}).subscribe();
        this.isLoading.set(false);
    }

    async deleteFile(path : string){
        this.isLoading.set(true);
        await firstValueFrom(this.http.delete("/api/fs/persistent/files", { params: { path: path }}))
        await this.loadTree();
		this.service.open("The target file have been deleted", {
			label: "File Deleted",
			appearance: 'info',
			icon:'info',
			autoClose: 1000
		}).subscribe();
        this.isLoading.set(false);
    }
}
