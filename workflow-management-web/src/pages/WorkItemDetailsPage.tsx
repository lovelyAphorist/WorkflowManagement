import { useEffect, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { ApiError } from '../api/apiClient';
import { assignWorkItem, createWorkItemComment, getWorkItemById, getWorkItemComments, getWorkItemHistory, updateWorkItem } from '../api/workItemsApi';
import { useAuth } from '../auth/AuthContext';
import type { UpdateWorkItemRequest, WorkItem, WorkItemComment, WorkItemHistory, WorkItemPriority, WorkItemStatus } from '../types/workItem';
import { getUsers } from '../api/usersApi';
import type { User } from '../types/auth';
import './WorkItemDetailsPage.css';

function WorkItemDetailsPage() {
    const { id } = useParams();
    const {token, logout, isAdmin} = useAuth();
    const navigate = useNavigate();

    const [workItem, setWorkItem] =
        useState<WorkItem | null>(null);

    const [comments, setComments] =
        useState<WorkItemComment[]>([]);

    const [history, setHistory] =
        useState<WorkItemHistory[]>([]);

    const [newComment, setNewComment] =
        useState('');

    const [isSubmittingComment, setIsSubmittingComment] =
        useState(false);

    const [commentError, setCommentError] =
        useState<string | null>(null);

    const [isLoading, setIsLoading] =
        useState(true);

    const [error, setError] =
        useState<string | null>(null);

    const [isEditing, setIsEditing] =
        useState(false);

    const [isSaving, setIsSaving] =
        useState(false);

    const [editError, setEditError] =
        useState<string | null>(null);

    const [editTitle, setEditTitle] =
        useState('');

    const [editDescription, setEditDescription] =
        useState('');

    const [editStatus, setEditStatus] =
        useState<WorkItemStatus>('Backlog');

    const [editPriority, setEditPriority] =
        useState<WorkItemPriority>('Medium');

    const [editDueDate, setEditDueDate] =
        useState('');

    const [users, setUsers] =
        useState<User[]>([]);

    const [isSavingAssignee, setIsSavingAssignee] =
        useState(false);

    const [assigneeError, setAssigneeError] =
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

    useEffect(() => {
        if (!token || !isAdmin) {
            return;
        }

        let cancelled = false;

        async function loadUsers() {
            try {
                const data = await getUsers(token!);

                if (!cancelled) {
                    setUsers(data);
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

                setAssigneeError(
                    error instanceof Error
                        ? error.message
                        : 'Unable to load users.'
                );
            }
        }

        loadUsers();

        return () => {
            cancelled = true;
        };
    }, [token, isAdmin, logout, navigate]);



    async function handleAddComment() {
        if (!id || !token) {
            return;
        }

        const body = newComment.trim();

        if (!body) {
            return;
        }

        try {
            setIsSubmittingComment(true);
            setCommentError(null);

            const comment =
                await createWorkItemComment(
                    id,
                    body,
                    token
                );

            setComments(current => [
                ...current,
                comment
            ]);

            setNewComment('');
        }
        catch (error) {
            if (
                error instanceof ApiError &&
                error.status === 401
            ) {
                logout();
                navigate('/login');
                return;
            }

            setCommentError(
                error instanceof Error
                    ? error.message
                    : 'Unable to add comment.'
            );
        }
        finally {
            setIsSubmittingComment(false);
        }
    }

    async function handleAssigneeChange(
        assigneeId: string
    ) {
        if (
            !id ||
            !token ||
            !isAdmin
        ) {
            return;
        }

        try {
            setIsSavingAssignee(true);
            setAssigneeError(null);

            const updated =
                await assignWorkItem(
                    id,
                    assigneeId || null,
                    token
                );

            setWorkItem(updated);

            const updatedHistory =
                await getWorkItemHistory(
                    id,
                    token
                );

            setHistory(updatedHistory);
        }
        catch (error) {
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
                error.status === 403
            ) {
                setAssigneeError(
                    'You do not have permission to assign work items.'
                );
                return;
            }

            setAssigneeError(
                error instanceof Error
                    ? error.message
                    : 'Unable to change assignee.'
            );
        }
        finally {
            setIsSavingAssignee(false);
        }
    }

    function beginEditing() {
        if (!workItem) {
            return;
        }

        setEditTitle(workItem.title);
        setEditDescription(
            workItem.description ?? ''
        );
        setEditStatus(workItem.status);
        setEditPriority(workItem.priority);
        setEditDueDate(
            workItem.dueDate ?? ''
        );

        setEditError(null);
        setIsEditing(true);
    }

    function cancelEditing() {
        setEditError(null);
        setIsEditing(false);
    }

    function handleStatusChange(value: string) {
        switch (value) {
            case 'Backlog':
            case 'Todo':
            case 'InProgress':
            case 'Blocked':
            case 'Completed':
            case 'Cancelled':
                setEditStatus(value);
                break;
        }
    }

    function handlePriorityChange(value: string) {
        switch (value) {
            case 'Low':
            case 'Medium':
            case 'High':
            case 'Critical':
                setEditPriority(value);
                break;
        }
    }

    async function handleSaveWorkItem() {
        if (!id || !token || !workItem) {
            return;
        }

        const title = editTitle.trim();

        if (!title) {
            setEditError('Title is required.');
            return;
        }

        const request: UpdateWorkItemRequest = {
            title,
            description:
                editDescription.trim() || null,
            status: editStatus,
            priority: editPriority,
            dueDate: editDueDate || null
        };

        try {
            setIsSaving(true);
            setEditError(null);

            const updated =
                await updateWorkItem(
                    id,
                    request,
                    token
                );

            setWorkItem(updated);
            setIsEditing(false);

            const updatedHistory =
                await getWorkItemHistory(
                    id,
                    token
                );

            setHistory(updatedHistory);
        }
        catch (error) {
            if (
                error instanceof ApiError &&
                error.status === 401
            ) {
                logout();
                navigate('/login');
                return;
            }

            setEditError(
                error instanceof Error
                    ? error.message
                    : 'Unable to update work item.'
            );
        }
        finally {
            setIsSaving(false);
        }
    }

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

                            <h1>
                                {workItem.title}
                            </h1>
                        </div>

                        <div className="details-actions">
                            <span
                                className={`details-priority priority-${workItem.priority.toLowerCase()}`}
                            >
                                {workItem.priority}
                            </span>

                            {!isEditing && (
                                <button
                                    type="button"
                                    className="edit-button"
                                    onClick={beginEditing}
                                >
                                    Edit
                                </button>
                            )}
                        </div>
                    </div>
                </div>
            </header>

            <main className="details-content">
                <div className="details-main-column">
                    <section className="details-card">
                        {isEditing ? (
                            <div className="edit-form">
                                <h2>
                                    Edit work item
                                </h2>

                                <label>
                                    Title

                                    <input
                                        type="text"
                                        value={editTitle}
                                        maxLength={200}
                                        onChange={(event) =>
                                            setEditTitle(
                                                event.target.value
                                            )
                                        }
                                    />
                                </label>

                                <label>
                                    Description

                                    <textarea
                                        value={editDescription}
                                        maxLength={2000}
                                        rows={5}
                                        onChange={(event) =>
                                            setEditDescription(
                                                event.target.value
                                            )
                                        }
                                    />
                                </label>

                                <div className="edit-grid">
                                    <label>
                                        Status

                                        <select
                                            value={editStatus}
                                            onChange={(event) =>
                                                handleStatusChange(
                                                    event.target.value
                                                )
                                            }
                                        >
                                            <option value="Backlog">
                                                Backlog
                                            </option>

                                            <option value="Todo">
                                                Todo
                                            </option>

                                            <option value="InProgress">
                                                In Progress
                                            </option>

                                            <option value="Blocked">
                                                Blocked
                                            </option>

                                            <option value="Completed">
                                                Completed
                                            </option>

                                            <option value="Cancelled">
                                                Cancelled
                                            </option>
                                        </select>
                                    </label>

                                    <label>
                                        Priority

                                        <select
                                            value={editPriority}
                                            onChange={(event) =>
                                                handlePriorityChange(
                                                    event.target.value
                                                )
                                            }
                                        >
                                            <option value="Low">
                                                Low
                                            </option>

                                            <option value="Medium">
                                                Medium
                                            </option>

                                            <option value="High">
                                                High
                                            </option>

                                            <option value="Critical">
                                                Critical
                                            </option>
                                        </select>
                                    </label>

                                    <label>
                                        Due date

                                        <input
                                            type="date"
                                            value={editDueDate}
                                            onChange={(event) =>
                                                setEditDueDate(
                                                    event.target.value
                                                )
                                            }
                                        />
                                    </label>
                                </div>

                                {editError && (
                                    <div className="comment-form-error">
                                        {editError}
                                    </div>
                                )}

                                <div className="edit-form-actions">
                                    <button
                                        type="button"
                                        className="cancel-button"
                                        onClick={cancelEditing}
                                        disabled={isSaving}
                                    >
                                        Cancel
                                    </button>

                                    <button
                                        type="button"
                                        className="save-button"
                                        onClick={() =>
                                            void handleSaveWorkItem()
                                        }
                                        disabled={isSaving}
                                    >
                                        {isSaving
                                            ? 'Saving...'
                                            : 'Save changes'}
                                    </button>
                                </div>
                            </div>
                        ) : (
                            <>
                                <h2>Description</h2>

                                <p className="description-text">
                                    {workItem.description ??
                                        'No description provided.'}
                                </p>
                            </>
                        )}
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

                        <form
                            className="comment-form"
                            onSubmit={(event) => {
                                event.preventDefault();
                                void handleAddComment();
                            }}
                        >
                            <textarea
                                value={newComment}
                                onChange={(event) =>
                                    setNewComment(
                                        event.target.value
                                    )
                                }
                                placeholder="Add a comment..."
                                maxLength={2000}
                                rows={3}
                            />

                            <div className="comment-form-footer">
                                <span>
                                    {newComment.length}/2000
                                </span>

                                <button
                                    type="submit"
                                    disabled={
                                        isSubmittingComment ||
                                        !newComment.trim()
                                    }
                                >
                                    {isSubmittingComment
                                        ? 'Adding...'
                                        : 'Add comment'}
                                </button>
                            </div>

                            {commentError && (
                                <div className="comment-form-error">
                                    {commentError}
                                </div>
                            )}
                        </form>

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
                                                comment.author
                                                    .displayName
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

                                            <p>
                                                {comment.body}
                                            </p>

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
                                    {formatStatus(workItem.status)}
                                </dd>
                            </div>

                            <div>
                                <dt>Priority</dt>
                                <dd>{workItem.priority}</dd>
                            </div>

                            <div>
                                <dt>Assignee</dt>

                                <dd>
                                    {isAdmin ? (
                                        <select
                                            className="assignee-select"
                                            value={workItem.assigneeId ?? ''}
                                            disabled={isSavingAssignee}
                                            onChange={(event) =>
                                                void handleAssigneeChange(
                                                    event.target.value
                                                )
                                            }
                                        >
                                            <option value="">
                                                Unassigned
                                            </option>

                                            {users.map(user => (
                                                <option
                                                    key={user.id}
                                                    value={user.id}
                                                >
                                                    {user.displayName}
                                                </option>
                                            ))}
                                        </select>
                                    ) : (
                                        workItem.assignee?.displayName ??
                                        'Unassigned'
                                    )}
                                </dd>
                            </div>

                            <div>
                                <dt>Due date</dt>
                                <dd>
                                    {formatDueDate(workItem.dueDate)}
                                </dd>
                            </div>

                            <div>
                                <dt>Created</dt>
                                <dd>
                                    {formatDateTime(workItem.createdAtUtc)}
                                </dd>
                            </div>
                        </dl>

                        {assigneeError && (
                            <div className="details-inline-error">
                                {assigneeError}
                            </div>
                        )}
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