import { apiRequest } from './apiClient';
import type {PagedResult,WorkItem,WorkItemComment,WorkItemHistory} from '../types/workItem';

export async function getWorkItemById(
    id: string,
    token: string
): Promise<WorkItem> {
    return apiRequest<WorkItem>(
        `/api/work-items/${id}`,
        token
    );
}

export async function getWorkItems(
    token: string
): Promise<PagedResult<WorkItem>> {
    return apiRequest<PagedResult<WorkItem>>(
        '/api/work-items?page=1&pageSize=10&sortBy=UpdatedAt&sortDirection=Descending',
        token
    );
}

export async function getWorkItemComments(
    id: string,
    token: string
): Promise<WorkItemComment[]> {
    return apiRequest<WorkItemComment[]>(
        `/api/work-items/${id}/comments`,
        token
    );
}

export async function getWorkItemHistory(
    id: string,
    token: string
): Promise<WorkItemHistory[]> {
    return apiRequest<WorkItemHistory[]>(
        `/api/work-items/${id}/history`,
        token
    );
}