import { useState } from 'react';
import type { FormEvent } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../auth/AuthContext';
import './LoginPage.css';

function LoginPage() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState<string | null>(null);
    const [isLoading, setIsLoading] = useState(false);

    const { login } = useAuth();
    const navigate = useNavigate();

    async function handleSubmit(
        event: FormEvent<HTMLFormElement>
    ) {
        event.preventDefault();

        setError(null);
        setIsLoading(true);

        try {
            await login({
                email,
                password
            });

            navigate('/dashboard');
        }
        catch (error) {
            setError(
                error instanceof Error
                    ? error.message
                    : 'Unable to sign in.'
            );
        }
        finally {
            setIsLoading(false);
        }
    }

    return (
        <main className="login-page">
            <section className="login-card">
                <div className="login-heading">
                    <span className="login-eyebrow">
                        Workflow Management
                    </span>

                    <h1>Welcome back</h1>

                    <p>
                        Sign in to manage work, assignments,
                        comments, and activity.
                    </p>
                </div>

                <form onSubmit={handleSubmit}>
                    <div className="form-field">
                        <label htmlFor="email">
                            Email
                        </label>

                        <input
                            id="email"
                            type="email"
                            value={email}
                            onChange={(event) =>
                                setEmail(event.target.value)
                            }
                            autoComplete="email"
                            required
                        />
                    </div>

                    <div className="form-field">
                        <label htmlFor="password">
                            Password
                        </label>

                        <input
                            id="password"
                            type="password"
                            value={password}
                            onChange={(event) =>
                                setPassword(event.target.value)
                            }
                            autoComplete="current-password"
                            required
                        />
                    </div>

                    {error && (
                        <div className="login-error">
                            {error}
                        </div>
                    )}

                    <button
                        type="submit"
                        disabled={isLoading}
                    >
                        {isLoading
                            ? 'Signing in...'
                            : 'Sign in'}
                    </button>
                </form>
            </section>
        </main>
    );
}

export default LoginPage;