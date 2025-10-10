import { API_ENDPOINTS } from "@/lib/api";
import type { Registration } from "@/types/registration.types";
import { parseValidationError } from "@/lib/errorParser";

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

  async getByUserId(userId: string): Promise<Registration[]> {
    const response = await fetch(API_ENDPOINTS.registrations.getByUserId(userId), {
      credentials: "include",
    });

    if (!response.ok) {
      throw new Error("Failed to fetch user registrations");
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
      throw new Error(parseValidationError(error));
    }

    return response.json();
  },

  async delete(id: string): Promise<void> {
    const response = await fetch(API_ENDPOINTS.registrations.delete(id), {
      method: "DELETE",
      credentials: "include",
    });

    if (!response.ok) {
      const error = await response.json().catch(() => ({ message: "Failed to delete registration" }));
      throw new Error(parseValidationError(error));
    }
  },
};
