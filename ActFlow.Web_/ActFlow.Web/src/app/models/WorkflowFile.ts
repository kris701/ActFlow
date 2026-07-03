export interface WorkflowFile {
    path: string;
    action: 'Load' | "Save";
    directory: 'Persistent' | 'Temporary';
}
