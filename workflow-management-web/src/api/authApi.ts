import { apiRequest } from './apiClient';
import type {
    LoginRequest,
    LoginResult
} from '../types/auth';

export async function login(
    request: LoginRequest
): Promise<LoginResult> {
    return apiRequest<LoginResult>(
        '/api/auth/login',
        null,
        {
            method: 'POST',
            body: JSON.stringify(request)
        }
    );
}