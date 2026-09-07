import { useAuth } from '../auth/AuthContext';

function DashboardPage() {
    const { user, logout } = useAuth();

    return (
        <main>
            <h1>Dashboard</h1>

            <p>
                Welcome, {user?.displayName}.
            </p>

            <button onClick={logout}>
                Sign out
            </button>
        </main>
    );
}

export default DashboardPage;