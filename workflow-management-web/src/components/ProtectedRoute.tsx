import { Navigate } from 'react-router-dom';
import type { ReactNode } from 'react';

import { useAuth } from '../auth/AuthContext';

function ProtectedRoute({
    children
}: {
    children: ReactNode;
}) {
    const { isAuthenticated } = useAuth();

    if (!isAuthenticated) {
        return (
            <Navigate
                to="/login"
                replace
            />
        );
    }

    return children;
}

export default ProtectedRoute;