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

export type WorkItemChangeType =
    | 'Title'
    | 'Description'
    | 'Status'
    | 'Priority'
    | 'DueDate'
    | 'Assignee';

export interface WorkItemComment {
    id: string;
    body: string;
    author: UserSummary;
    createdAtUtc: string;
    editedAtUtc: string | null;
}

export interface WorkItemHistory {
    id: string;
    changeType: WorkItemChangeType;
    oldValue: string | null;
    newValue: string | null;
    changedAtUtc: string;
}

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

export interface UpdateWorkItemRequest {
    title: string;
    description: string | null;
    status: WorkItemStatus;
    priority: WorkItemPriority;
    dueDate: string | null;
}

export interface CreateWorkItemRequest {
    title: string;
    description: string | null;
    priority: WorkItemPriority;
    dueDate: string | null;
}