import { API_ENDPOINTS } from "@/lib/api";
import type { Registration } from "@/types/registration.types";

export const registrationService = {
  async getAll(): Promise<Registration[]> {
    const response = await fetch(API_ENDPOINTS.registrations.getAll, {
      credentials: "include",
    });

    if (!response.ok) {
      throw new Error("Failed to fetch registrations");
    }

    return response.json();
  },

  async create(data: { userId: string; scheduleAttractionId: string; registeredAt: string }): Promise<Registration> {
    const response = await fetch(API_ENDPOINTS.registrations.create, {
      method: "POST",
      credentials: "include",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      const error = await response.json().catch(() => ({ message: "Failed to create registration" }));
      throw new Error(error.message || "Failed to create registration");
    }

    return response.json();
  },
};
