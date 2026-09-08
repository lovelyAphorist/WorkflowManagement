const apiBaseUrl = import.meta.env.VITE_API_BASE_URL;

export class ApiError extends Error {
    status: number;

    constructor(status: number, message: string) {
        super(message);
        this.status = status;
    }
}

export async function apiRequest<T>(
    path: string,
    token: string | null,
    options: RequestInit = {}
): Promise<T> {
    const headers = new Headers(options.headers);

    if (token) {
        headers.set(
            'Authorization',
            `Bearer ${token}`
        );
    }

    if (options.body && !headers.has('Content-Type')) {
        headers.set(
            'Content-Type',
            'application/json'
        );
    }

    const response = await fetch(
        `${apiBaseUrl}${path}`,
        {
            ...options,
            headers
        }
    );

    if (!response.ok) {
        let message =
            `Request failed with status ${response.status}.`;

        try {
            const error = await response.json();

            message =
                error.message ??
                error.errors?.[0] ??
                message;
        }
        catch {
            // Response did not contain JSON.
        }

        throw new ApiError(
            response.status,
            message
        );
    }

    if (response.status === 204) {
        return undefined as T;
    }

    return response.json();
}