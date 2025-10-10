import { API_ENDPOINTS } from "@/lib/api";
import type { Schedule, CreateScheduleRequest, UpdateScheduleRequest } from "@/types/schedule.types";
import { parseValidationError } from "@/lib/errorParser";

export const scheduleService = {
  async getAll(): Promise<Schedule[]> {
    const response = await fetch(API_ENDPOINTS.schedules.getAll, {
      credentials: "include",
    });

    if (!response.ok) {
      throw new Error("Failed to fetch schedules");
    }

    return response.json();
  },

  async getById(id: string): Promise<Schedule> {
    const response = await fetch(API_ENDPOINTS.schedules.getById(id), {
      credentials: "include",
    });

    if (!response.ok) {
      throw new Error("Failed to fetch schedule");
    }

    return response.json();
  },

  async create(data: CreateScheduleRequest): Promise<Schedule> {
    const response = await fetch(API_ENDPOINTS.schedules.create, {
      method: "POST",
      credentials: "include",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      const error = await response.json().catch(() => ({ message: "Failed to create schedule" }));
      throw new Error(parseValidationError(error));
    }

    return response.json();
  },

  async update(id: string, data: UpdateScheduleRequest): Promise<Schedule> {
    const response = await fetch(API_ENDPOINTS.schedules.update(id), {
      method: "PUT",
      credentials: "include",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      const error = await response.json().catch(() => ({ message: "Failed to update schedule" }));
      throw new Error(parseValidationError(error));
    }

    return response.json();
  },

  async delete(id: string): Promise<void> {
    const response = await fetch(API_ENDPOINTS.schedules.delete(id), {
      method: "DELETE",
      credentials: "include",
    });

    if (!response.ok) {
      const error = await response.json().catch(() => ({ message: "Failed to delete schedule" }));
      throw new Error(parseValidationError(error));
    }
  },
};
