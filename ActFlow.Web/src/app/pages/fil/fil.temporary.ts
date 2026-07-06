import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TuiHandler } from '@taiga-ui/cdk/types';
import { TuiButton, TuiHint, TuiIcon, TuiLoader } from '@taiga-ui/core';
import { TuiChip, TuiTree } from "@taiga-ui/kit";
import { TuiBlockStatusComponent } from "@taiga-ui/layout";
import { firstValueFrom } from 'rxjs';
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
	TuiHint
],
	template: `
	 @if(runId() === null){
            <span style="align-self:center;text-align: center;height:100%;align-content:center;">
                No workflow run selected!
                Go to <b><a style="text-decoration: underline;" routerLink="/workflows/results">results</a></b>, find a workflow that you want to see the temporary files for and click the "View" button under files.
            </span>
        }
		@else {
			<tui-loader [inheritColor]="true" [overlay]="true" size="xxl" [loading]="isLoading()">
				<div class="flex flex-row gap-2">
					<button
						tuiButton
						iconStart="rotate-cw"
						size="s"
						appearance="secondary"
						(click)="loadTree()"
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
								</span>
							}
							@else {
								<span class="item" tuiChip size="xs">{{item.data.extension}}</span>
								<span class="item" tuiChip size="xs">{{FileHelpers.HumanFileSize(item.data.size)}}</span>
								<span class="item flex flex-row gap-2">
									<a [href]="'/api/fs/temporary/files' + '?id=' + runId() + '&path=' + item.data.path" [download]="item.data.name" (click)="$event.stopPropagation()">
										<tui-icon icon="download" tuiHint="Download"/>
									</a>
								</span>
							}
						</div>
					</ng-template>
				}
			</tui-loader>
		}
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
export class FILTemporary {
    isLoading = signal<boolean>(false);
    files = signal<TreeNode[]>([]);
    root = signal<DirectoryRoot>({ directories: [], files: [] } as DirectoryRoot)
    runId = signal<string | null>(null);
    highlightTarget = signal<string | null>(null);

    FileHelpers = FileHelpers;

	map = signal<Map<TreeNode, boolean>>(new Map<TreeNode, boolean>());

    private route = inject(ActivatedRoute);
    constructor(private http : HttpClient, public router : Router){}

	protected readonly handler: TuiHandler<TreeNode, readonly TreeNode[]> = (item) =>
		item.children || [];

    async ngOnInit(){
        this.isLoading.set(true);
        this.runId.set(this.route.snapshot.queryParamMap.get('id'));
        if (this.runId() === null)
            return;
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
