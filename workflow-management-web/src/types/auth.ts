export interface LoginRequest {
    email: string;
    password: string;
}

export interface User {
    id: string;
    displayName: string;
    email: string;
}

export interface LoginResult {
    succeeded: boolean;
    token: string;
    expiresAtUtc: string;
    user: User;
    roles: string[];
    errors: string[];
}