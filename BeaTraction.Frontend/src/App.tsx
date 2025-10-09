import { Routes, Route } from "react-router";
import "./App.css";
import { Toaster } from "@/components/ui/sonner";
import { ProtectedRoute } from "@/components/ProtectedRoute";
import LoginPage from "./pages/auth/LoginPage";
import RegisterPage from "./pages/auth/RegisterPage";
import AdminDashboardPage from "./pages/admin/AdminDashboardPage";
import AdminAttractionsPage from "./pages/admin/AdminAttractionsPage";
import AdminSchedulesPage from "./pages/admin/AdminSchedulesPage";
import UserRegistrationPage from "./pages/user/UserRegistrationPage";

function App() {
  return (
    <>
      <Routes>
        {/* Public Routes */}
        <Route path="/" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />

        {/* Admin Routes - Protected */}
        <Route
          path="/admin/dashboard"
          element={
            <ProtectedRoute requiredRole="admin">
              <AdminDashboardPage />
            </ProtectedRoute>
          }
        />
        <Route
          path="/admin/attractions"
          element={
            <ProtectedRoute requiredRole="admin">
              <AdminAttractionsPage />
            </ProtectedRoute>
          }
        />
        <Route
          path="/admin/schedules"
          element={
            <ProtectedRoute requiredRole="admin">
              <AdminSchedulesPage />
            </ProtectedRoute>
          }
        />

        {/* User Routes - Protected */}
        <Route
          path="/attractions"
          element={
            <ProtectedRoute>
              <UserRegistrationPage />
            </ProtectedRoute>
          }
        />
      </Routes>
      <Toaster />
    </>
  );
}

export default App;
