import {
    useState
} from 'react';

import type {
    CreateWorkItemRequest,
    WorkItem,
    WorkItemPriority
} from '../types/workItem';

import { createWorkItem } from '../api/workItemsApi';
import { ApiError } from '../api/apiClient';

interface NewWorkItemFormProps {
    token: string;
    onCreated: (workItem: WorkItem) => void;
    onCancel: () => void;
    onUnauthorized: () => void;
}

function NewWorkItemForm({
    token,
    onCreated,
    onCancel,
    onUnauthorized
}: NewWorkItemFormProps) {
    const [title, setTitle] =
        useState('');

    const [description, setDescription] =
        useState('');

    const [priority, setPriority] =
        useState<WorkItemPriority>('Medium');

    const [dueDate, setDueDate] =
        useState('');

    const [isSaving, setIsSaving] =
        useState(false);

    const [error, setError] =
        useState<string | null>(null);

    async function handleCreate() {
        const trimmedTitle = title.trim();

        if (!trimmedTitle) {
            setError('Title is required.');
            return;
        }

        const request: CreateWorkItemRequest = {
            title: trimmedTitle,
            description:
                description.trim() || null,
            priority,
            dueDate: dueDate || null
        };

        try {
            setIsSaving(true);
            setError(null);

            const created =
                await createWorkItem(
                    request,
                    token
                );

            onCreated(created);
        }
        catch (error) {
            if (
                error instanceof ApiError &&
                error.status === 401
            ) {
                onUnauthorized();
                return;
            }

            setError(
                error instanceof Error
                    ? error.message
                    : 'Unable to create work item.'
            );
        }
        finally {
            setIsSaving(false);
        }
    }

    function handlePriorityChange(
        value: string
    ) {
        switch (value) {
            case 'Low':
            case 'Medium':
            case 'High':
            case 'Critical':
                setPriority(value);
                break;
        }
    }

    return (
        <div className="new-work-item">
            <div className="new-work-item-heading">
                <div>
                    <h2>New work item</h2>
                    <p>
                        Create a new item for the workspace.
                    </p>
                </div>

                <button
                    type="button"
                    className="close-form-button"
                    onClick={onCancel}
                >
                    ×
                </button>
            </div>

            <div className="new-work-item-form">
                <label>
                    Title

                    <input
                        type="text"
                        value={title}
                        maxLength={200}
                        onChange={(event) =>
                            setTitle(
                                event.target.value
                            )
                        }
                        placeholder="What needs to be done?"
                    />
                </label>

                <label>
                    Description

                    <textarea
                        value={description}
                        maxLength={2000}
                        rows={4}
                        onChange={(event) =>
                            setDescription(
                                event.target.value
                            )
                        }
                        placeholder="Add more context..."
                    />
                </label>

                <div className="new-work-item-grid">
                    <label>
                        Priority

                        <select
                            value={priority}
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
                            value={dueDate}
                            onChange={(event) =>
                                setDueDate(
                                    event.target.value
                                )
                            }
                        />
                    </label>
                </div>

                {error && (
                    <div className="dashboard-error">
                        {error}
                    </div>
                )}

                <div className="new-work-item-actions">
                    <button
                        type="button"
                        className="cancel-button"
                        onClick={onCancel}
                        disabled={isSaving}
                    >
                        Cancel
                    </button>

                    <button
                        type="button"
                        className="create-button"
                        onClick={() =>
                            void handleCreate()
                        }
                        disabled={
                            isSaving ||
                            !title.trim()
                        }
                    >
                        {isSaving
                            ? 'Creating...'
                            : 'Create work item'}
                    </button>
                </div>
            </div>
        </div>
    );
}

export default NewWorkItemForm;