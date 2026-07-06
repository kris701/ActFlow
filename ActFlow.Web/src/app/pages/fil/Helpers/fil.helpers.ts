import { DirectoryModel } from "../Models/DirectoryModel";
import { FilesModel } from "../Models/FilesModel";
import { TreeNode } from "../Models/TreeNode";

export class FileHelpers {
    public static BuildTreeNodeDir(dir : DirectoryModel) : TreeNode {
        var children : TreeNode[] = [];

        dir.directories.forEach(x => children.push(this.BuildTreeNodeDir(x)))
        dir.files.forEach(x => children.push(this.BuildTreeNodeFile(x)))

		var folderSize : number = 0;
		children.forEach(x => folderSize += Number(x.data.size))

        return {
            label: dir.name,
            children: children,
            data: {
                type: "dir",
                name: dir.name,
                path: dir.path,
				size: folderSize
            }
        } as TreeNode
    }

    public static BuildTreeNodeFile(file : FilesModel) : TreeNode {
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
    };

    //https://stackoverflow.com/a/14919494
    public static HumanFileSize(bytes : number, si=false, dp=1) {
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

    public static ExpandToTarget(nodes : TreeNode[], target : string, map : Map<TreeNode, boolean>) : Map<TreeNode, boolean>{
        for(var node of nodes){
            if (node.data.type != "dir")
                continue;
            if (target.startsWith(node.data.path))
				map.set(node, true);
            if (node.children)
                this.ExpandToTarget(node.children, target, map);
        }
		return map;
    }
}
