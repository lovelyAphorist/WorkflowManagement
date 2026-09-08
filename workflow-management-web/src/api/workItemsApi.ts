import { apiRequest } from './apiClient';

import type {
    PagedResult,
    WorkItem
} from '../types/workItem';

export async function getWorkItems(
    token: string
): Promise<PagedResult<WorkItem>> {
    return apiRequest<PagedResult<WorkItem>>(
        '/api/work-items?page=1&pageSize=10&sortBy=UpdatedAt&sortDirection=Descending',
        token
    );
}