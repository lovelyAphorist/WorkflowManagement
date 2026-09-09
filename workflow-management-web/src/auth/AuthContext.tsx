import { createContext, useContext, useState } from 'react';

import type { ReactNode } from 'react';
import type { LoginRequest, User } from '../types/auth';

import { login as loginApi } from '../api/authApi';

interface AuthState {
    token: string;
    expiresAtUtc: string;
    user: User;
    roles: string[];
}

interface AuthContextValue {
    user: User | null;
    token: string | null;
    roles: string[];
    isAuthenticated: boolean;
    isAdmin: boolean;
    login: (request: LoginRequest) => Promise<void>;
    logout: () => void;
}

const STORAGE_KEY = 'workflow-management-auth';

const AuthContext =
    createContext<AuthContextValue | undefined>(
        undefined
    );

function loadStoredAuth(): AuthState | null {
    const stored =
        sessionStorage.getItem(STORAGE_KEY);

    if (!stored) {
        return null;
    }

    try {
        const auth =
            JSON.parse(stored) as AuthState;

        const expiresAt =
            new Date(auth.expiresAtUtc).getTime();

        if (expiresAt <= Date.now()) {
            sessionStorage.removeItem(STORAGE_KEY);
            return null;
        }

        return {
            ...auth,
            roles: auth.roles ?? []
        };
    }
    catch {
        sessionStorage.removeItem(STORAGE_KEY);
        return null;
    }
}

export function AuthProvider({
    children
}: {
    children: ReactNode;
}) {
    const [auth, setAuth] =
        useState<AuthState | null>(
            () => loadStoredAuth()
        );

    async function login(
        request: LoginRequest
    ) {
        const result =
            await loginApi(request);

        const nextAuth: AuthState = {
            token: result.token,
            expiresAtUtc: result.expiresAtUtc,
            user: result.user,
            roles: result.roles
        };

        sessionStorage.setItem(
            STORAGE_KEY,
            JSON.stringify(nextAuth)
        );

        setAuth(nextAuth);
    }

    function logout() {
        sessionStorage.removeItem(
            STORAGE_KEY
        );

        setAuth(null);
    }

    const value: AuthContextValue = {
        user: auth?.user ?? null,
        token: auth?.token ?? null,
        roles: auth?.roles ?? [],
        isAuthenticated: auth !== null,
        isAdmin:
            auth?.roles.includes('Admin') ??
            false,
        login,
        logout
    };

    return (
        <AuthContext.Provider value={value}>
            {children}
        </AuthContext.Provider>
    );
}

export function useAuth() {
    const context =
        useContext(AuthContext);

    if (!context) {
        throw new Error(
            'useAuth must be used inside AuthProvider.'
        );
    }

    return context;
}