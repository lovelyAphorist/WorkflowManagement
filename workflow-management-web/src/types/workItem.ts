export type WorkItemStatus =
    | 'Backlog'
    | 'Todo'
    | 'InProgress'
    | 'Blocked'
    | 'Completed'
    | 'Cancelled';

export type WorkItemPriority =
    | 'Low'
    | 'Medium'
    | 'High'
    | 'Critical';

export interface UserSummary {
    id: string;
    displayName: string;
}

export interface WorkItem {
    id: string;
    title: string;
    description: string | null;
    status: WorkItemStatus;
    priority: WorkItemPriority;
    dueDate: string | null;
    createdAtUtc: string;
    updatedAtUtc: string;
    assigneeId: string | null;
    assignee: UserSummary | null;
}

export interface PagedResult<T> {
    items: T[];
    page: number;
    pageSize: number;
    totalCount: number;
    totalPages: number;
}