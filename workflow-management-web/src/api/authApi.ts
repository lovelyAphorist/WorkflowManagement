import type { LoginRequest, LoginResult } from '../types/auth';

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL;

export async function login(
    request: LoginRequest
): Promise<LoginResult> {
    const response = await fetch(
        `${apiBaseUrl}/api/auth/login`,
        {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(request)
        }
    );

    if (!response.ok) {
        const error = await response.json();

        throw new Error(
            error.errors?.[0] ??
            'Unable to sign in.'
        );
    }

    return response.json();
}