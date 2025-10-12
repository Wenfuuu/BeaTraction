import type { ReactNode } from "react";
import { Navigate } from "react-router";
import { useAuthContext } from "@/contexts/AuthContext";

interface GuestRouteProps {
  children: ReactNode;
}

export function GuestRoute({ children }: GuestRouteProps) {
  const { user, isLoading } = useAuthContext();

  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-gray-900 mx-auto"></div>
          <p className="mt-4 text-gray-600">Loading...</p>
        </div>
      </div>
    );
  }

  if (user) {
    const redirectPath =
      user.role === "admin" ? "/admin/dashboard" : "/attractions";
    return <Navigate to={redirectPath} replace />;
  }

  return <>{children}</>;
}
