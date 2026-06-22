import { FilesModel } from "./FilesModel";

export interface DirectoryModel {
    path : string;
    name : string;
    directories : DirectoryModel[];
    files : FilesModel[];
}
