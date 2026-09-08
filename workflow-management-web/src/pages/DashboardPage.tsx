import {
    useEffect,
    useState
} from 'react';

import { useNavigate } from 'react-router-dom';

import {
    ApiError
} from '../api/apiClient';

import {
    getWorkItems
} from '../api/workItemsApi';

import {
    useAuth
} from '../auth/AuthContext';

import type {
    PagedResult,
    WorkItem
} from '../types/workItem';

import './DashboardPage.css';

function DashboardPage() {
    const {
        user,
        token,
        logout
    } = useAuth();

    const navigate = useNavigate();

    const [result, setResult] =
        useState<PagedResult<WorkItem> | null>(null);

    const [isLoading, setIsLoading] =
        useState(true);

    const [error, setError] =
        useState<string | null>(null);

    useEffect(() => {
        if (!token) {
            return;
        }

        let cancelled = false;

        async function loadWorkItems() {
            try {
                setIsLoading(true);
                setError(null);

                const data =
                    await getWorkItems(token!);

                if (!cancelled) {
                    setResult(data);
                }
            }
            catch (error) {
                if (cancelled) {
                    return;
                }

                if (
                    error instanceof ApiError &&
                    error.status === 401
                ) {
                    logout();
                    navigate('/login');
                    return;
                }

                setError(
                    error instanceof Error
                        ? error.message
                        : 'Unable to load work items.'
                );
            }
            finally {
                if (!cancelled) {
                    setIsLoading(false);
                }
            }
        }

        loadWorkItems();

        return () => {
            cancelled = true;
        };
    }, [token, logout, navigate]);

    function handleLogout() {
        logout();
        navigate('/login');
    }

    return (
        <div className="dashboard-page">
            <header className="dashboard-header">
                <div>
                    <span className="dashboard-eyebrow">
                        Workflow Management
                    </span>

                    <h1>Dashboard</h1>

                    <p>
                        Welcome back, {user?.displayName}.
                    </p>
                </div>

                <button
                    className="sign-out-button"
                    onClick={handleLogout}
                >
                    Sign out
                </button>
            </header>

            <main className="dashboard-content">
                <section className="dashboard-summary">
                    <div className="summary-card">
                        <span>Total work items</span>

                        <strong>
                            {result?.totalCount ?? '—'}
                        </strong>
                    </div>

                    <div className="summary-card">
                        <span>Signed in as</span>

                        <strong>
                            {user?.displayName ?? '—'}
                        </strong>
                    </div>
                </section>

                <section className="work-items-section">
                    <div className="section-heading">
                        <div>
                            <h2>Recent work</h2>
                            <p>
                                Recently updated work items
                                across the workspace.
                            </p>
                        </div>
                    </div>

                    {isLoading && (
                        <div className="dashboard-message">
                            Loading work items...
                        </div>
                    )}

                    {error && (
                        <div className="dashboard-error">
                            {error}
                        </div>
                    )}

                    {!isLoading &&
                        !error &&
                        result?.items.length === 0 && (
                            <div className="dashboard-message">
                                No work items yet.
                            </div>
                        )}

                    {!isLoading &&
                        !error &&
                        result &&
                        result.items.length > 0 && (
                            <div className="work-item-list">
                                {result.items.map(
                                    (workItem) => (
                                        <article
                                            className="work-item-card"
                                            key={workItem.id}
                                        >
                                            <div className="work-item-main">
                                                <div className="work-item-title-row">
                                                    <h3>
                                                        {workItem.title}
                                                    </h3>

                                                    <span
                                                        className={`priority-badge priority-${workItem.priority.toLowerCase()}`}
                                                    >
                                                        {workItem.priority}
                                                    </span>
                                                </div>

                                                <p>
                                                    {workItem.description ??
                                                        'No description provided.'}
                                                </p>
                                            </div>

                                            <div className="work-item-meta">
                                                <span>
                                                    Status
                                                    <strong>
                                                        {formatStatus(
                                                            workItem.status
                                                        )}
                                                    </strong>
                                                </span>

                                                <span>
                                                    Assignee
                                                    <strong>
                                                        {workItem.assignee
                                                            ?.displayName ??
                                                            'Unassigned'}
                                                    </strong>
                                                </span>

                                                <span>
                                                    Due
                                                    <strong>
                                                        {formatDueDate(
                                                            workItem.dueDate
                                                        )}
                                                    </strong>
                                                </span>
                                            </div>
                                        </article>
                                    )
                                )}
                            </div>
                        )}
                </section>
            </main>
        </div>
    );
}

function formatStatus(status: WorkItem['status']) {
    return status
        .replace(/([A-Z])/g, ' $1')
        .trim();
}

function formatDueDate(
    dueDate: string | null
) {
    if (!dueDate) {
        return 'No due date';
    }

    return new Date(
        `${dueDate}T00:00:00`
    ).toLocaleDateString();
}

export default DashboardPage;