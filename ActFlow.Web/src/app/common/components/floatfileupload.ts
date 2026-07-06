import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TuiFiles } from '@taiga-ui/kit';

@Component({
	selector: 'app-floatfileupload',
    imports: [FormsModule, TuiFiles],
	template:`
		<label tuiInputFiles>
			<input
				#validator="tuiInputFilesValidator"
				accept="*/*"
				tuiInputFiles
				[ngModel]="files()"
				(ngModelChange)="onChange($event)"
			/>
		</label>

		<tui-files>
			@for(file of files(); track file){
				<tui-file
					state="normal"
					[file]="file"
					(remove)="removeFile(file)"
				/>
			}
		</tui-files>
	`
})
export default class FloatFileUpload {
	public files = signal<File[]>([]);

	public clear(){
		this.files.set([]);
	}

	protected onChange(files: File[] | File): void {
		if (files instanceof File)
			this.files.set([files]);
		else
			this.files.set(files);
	}

	removeFile(file : File){
		var files = this.files();
		var index = files.indexOf(file);
		if (index != -1){
			files.splice(index, 1);
			this.files.set(files);
		}
	}
}
