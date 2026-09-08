import {Navigate,Route,Routes} from 'react-router-dom';
import WorkItemDetailsPage from './pages/WorkItemDetailsPage';
import ProtectedRoute from './components/ProtectedRoute';
import DashboardPage from './pages/DashboardPage';
import LoginPage from './pages/LoginPage';

function App() {
    return (
        <Routes>
            <Route
                path="/login"
                element={<LoginPage />}
            />

            <Route
                path="/dashboard"
                element={
                    <ProtectedRoute>
                        <DashboardPage />
                    </ProtectedRoute>
                }
            />

            <Route
                path="/"
                element={
                    <Navigate
                        to="/dashboard"
                        replace
                    />
                }
            />
            <Route
                path="/work-items/:id"
                element={
                    <ProtectedRoute>
                        <WorkItemDetailsPage />
                    </ProtectedRoute>
                }
            />
        </Routes>
    );
}

export default App;