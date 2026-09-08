import {useEffect,useState} from 'react';
import {Link,useNavigate,useParams} from 'react-router-dom';
import { ApiError } from '../api/apiClient';
import {getWorkItemById,getWorkItemComments,getWorkItemHistory} from '../api/workItemsApi';
import { useAuth } from '../auth/AuthContext';
import type {WorkItem,WorkItemComment,WorkItemHistory} from '../types/workItem';

import './WorkItemDetailsPage.css';

function WorkItemDetailsPage() {
    const { id } = useParams();
    const { token, logout } = useAuth();
    const navigate = useNavigate();

    const [workItem, setWorkItem] =
        useState<WorkItem | null>(null);

    const [comments, setComments] =
        useState<WorkItemComment[]>([]);

    const [history, setHistory] =
        useState<WorkItemHistory[]>([]);

    const [isLoading, setIsLoading] =
        useState(true);

    const [error, setError] =
        useState<string | null>(null);

    useEffect(() => {
        if (!id || !token) {
            return;
        }

        let cancelled = false;

        async function loadDetails() {
            try {
                setIsLoading(true);
                setError(null);

                const [
                    workItemData,
                    commentsData,
                    historyData
                ] = await Promise.all([
                    getWorkItemById(id!, token!),
                    getWorkItemComments(id!, token!),
                    getWorkItemHistory(id!, token!)
                ]);

                if (cancelled) {
                    return;
                }

                setWorkItem(workItemData);
                setComments(commentsData);
                setHistory(historyData);
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

                if (
                    error instanceof ApiError &&
                    error.status === 404
                ) {
                    setError('Work item not found.');
                    return;
                }

                setError(
                    error instanceof Error
                        ? error.message
                        : 'Unable to load work item.'
                );
            }
            finally {
                if (!cancelled) {
                    setIsLoading(false);
                }
            }
        }

        loadDetails();

        return () => {
            cancelled = true;
        };
    }, [id, token, logout, navigate]);

    if (isLoading) {
        return (
            <main className="details-state">
                Loading work item...
            </main>
        );
    }

    if (error || !workItem) {
        return (
            <main className="details-state">
                <Link to="/dashboard">
                    ← Back to dashboard
                </Link>

                <p>
                    {error ?? 'Work item not found.'}
                </p>
            </main>
        );
    }

    return (
        <div className="details-page">
            <header className="details-header">
                <div className="details-header-inner">
                    <Link
                        to="/dashboard"
                        className="back-link"
                    >
                        ← Back to dashboard
                    </Link>

                    <div className="details-title-row">
                        <div>
                            <span className="details-eyebrow">
                                Work item
                            </span>

                            <h1>{workItem.title}</h1>
                        </div>

                        <span
                            className={`details-priority priority-${workItem.priority.toLowerCase()}`}
                        >
                            {workItem.priority}
                        </span>
                    </div>
                </div>
            </header>

            <main className="details-content">
                <div className="details-main-column">
                    <section className="details-card">
                        <h2>Description</h2>

                        <p className="description-text">
                            {workItem.description ??
                                'No description provided.'}
                        </p>
                    </section>

                    <section className="details-card">
                        <div className="card-heading">
                            <div>
                                <h2>Comments</h2>
                                <p>
                                    Discussion about this work item.
                                </p>
                            </div>

                            <span className="count-badge">
                                {comments.length}
                            </span>
                        </div>

                        {comments.length === 0 ? (
                            <p className="empty-text">
                                No comments yet.
                            </p>
                        ) : (
                            <div className="comments-list">
                                {comments.map(comment => (
                                    <article
                                        className="comment"
                                        key={comment.id}
                                    >
                                        <div className="comment-avatar">
                                            {getInitials(
                                                comment.author.displayName
                                            )}
                                        </div>

                                        <div className="comment-content">
                                            <div className="comment-heading">
                                                <strong>
                                                    {
                                                        comment.author
                                                            .displayName
                                                    }
                                                </strong>

                                                <span>
                                                    {formatDateTime(
                                                        comment.createdAtUtc
                                                    )}
                                                </span>
                                            </div>

                                            <p>{comment.body}</p>

                                            {comment.editedAtUtc && (
                                                <span className="edited-label">
                                                    Edited
                                                </span>
                                            )}
                                        </div>
                                    </article>
                                ))}
                            </div>
                        )}
                    </section>
                </div>

                <aside className="details-sidebar">
                    <section className="details-card">
                        <h2>Details</h2>

                        <dl className="details-list">
                            <div>
                                <dt>Status</dt>
                                <dd>
                                    {formatStatus(
                                        workItem.status
                                    )}
                                </dd>
                            </div>

                            <div>
                                <dt>Priority</dt>
                                <dd>{workItem.priority}</dd>
                            </div>

                            <div>
                                <dt>Assignee</dt>
                                <dd>
                                    {workItem.assignee
                                        ?.displayName ??
                                        'Unassigned'}
                                </dd>
                            </div>

                            <div>
                                <dt>Due date</dt>
                                <dd>
                                    {formatDueDate(
                                        workItem.dueDate
                                    )}
                                </dd>
                            </div>

                            <div>
                                <dt>Created</dt>
                                <dd>
                                    {formatDateTime(
                                        workItem.createdAtUtc
                                    )}
                                </dd>
                            </div>
                        </dl>
                    </section>

                    <section className="details-card">
                        <div className="card-heading">
                            <div>
                                <h2>Activity</h2>
                                <p>Recent changes.</p>
                            </div>
                        </div>

                        {history.length === 0 ? (
                            <p className="empty-text">
                                No activity yet.
                            </p>
                        ) : (
                            <div className="activity-list">
                                {history.map(entry => (
                                    <article
                                        className="activity-item"
                                        key={entry.id}
                                    >
                                        <div className="activity-dot" />

                                        <div>
                                            <strong>
                                                {formatChangeType(
                                                    entry.changeType
                                                )}
                                            </strong>

                                            <p>
                                                <span>
                                                    {entry.oldValue ??
                                                        'None'}
                                                </span>

                                                {' → '}

                                                <span>
                                                    {entry.newValue ??
                                                        'None'}
                                                </span>
                                            </p>

                                            <time>
                                                {formatDateTime(
                                                    entry.changedAtUtc
                                                )}
                                            </time>
                                        </div>
                                    </article>
                                ))}
                            </div>
                        )}
                    </section>
                </aside>
            </main>
        </div>
    );
}

function formatStatus(status: string) {
    return status
        .replace(/([A-Z])/g, ' $1')
        .trim();
}

function formatChangeType(changeType: string) {
    if (changeType === 'DueDate') {
        return 'Due date changed';
    }

    return `${formatStatus(changeType)} changed`;
}

function formatDueDate(dueDate: string | null) {
    if (!dueDate) {
        return 'No due date';
    }

    return new Date(
        `${dueDate}T00:00:00`
    ).toLocaleDateString();
}

function formatDateTime(value: string) {
    return new Date(value).toLocaleString();
}

function getInitials(name: string) {
    return name
        .split(' ')
        .map(part => part[0])
        .join('')
        .slice(0, 2)
        .toUpperCase();
}

export default WorkItemDetailsPage;