import { DirectoryModel } from "./DirectoryModel";
import { FilesModel } from "./FilesModel";

export interface DirectoryRoot {
    directories : DirectoryModel[];
    files : FilesModel[];
}
