import { API_ENDPOINTS } from "@/lib/api";
import type {
  ScheduleAttraction,
  ScheduleAttractionWithDetails,
  CreateScheduleAttractionRequest,
} from "@/types/schedule-attraction.types";

export const scheduleAttractionService = {
  async getAll(): Promise<ScheduleAttractionWithDetails[]> {
    const response = await fetch(API_ENDPOINTS.scheduleAttractions.getAll, {
      credentials: "include",
    });

    if (!response.ok) {
      throw new Error("Failed to fetch schedule attractions");
    }

    return response.json();
  },

  async getById(id: string): Promise<ScheduleAttractionWithDetails> {
    const response = await fetch(
      API_ENDPOINTS.scheduleAttractions.getById(id),
      {
        credentials: "include",
      }
    );

    if (!response.ok) {
      throw new Error("Failed to fetch schedule attraction");
    }

    return response.json();
  },

  async getByScheduleId(
    scheduleId: string
  ): Promise<ScheduleAttractionWithDetails[]> {
    const response = await fetch(
      API_ENDPOINTS.scheduleAttractions.getByScheduleId(scheduleId),
      {
        credentials: "include",
      }
    );

    if (!response.ok) {
      throw new Error("Failed to fetch attractions for schedule");
    }

    return response.json();
  },

  async getByAttractionId(
    attractionId: string
  ): Promise<ScheduleAttractionWithDetails[]> {
    const response = await fetch(
      API_ENDPOINTS.scheduleAttractions.getByAttractionId(attractionId),
      {
        credentials: "include",
      }
    );

    if (!response.ok) {
      throw new Error("Failed to fetch schedules for attraction");
    }

    return response.json();
  },

  async create(
    data: CreateScheduleAttractionRequest
  ): Promise<ScheduleAttraction> {
    const response = await fetch(API_ENDPOINTS.scheduleAttractions.create, {
      method: "POST",
      credentials: "include",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      const error = await response
        .json()
        .catch(() => ({ message: "Failed to add attraction to schedule" }));
      throw new Error(error.message || "Failed to add attraction to schedule");
    }

    return response.json();
  },

  async delete(id: string): Promise<void> {
    const response = await fetch(API_ENDPOINTS.scheduleAttractions.delete(id), {
      method: "DELETE",
      credentials: "include",
    });

    if (!response.ok) {
      const error = await response
        .json()
        .catch(() => ({
          message: "Failed to remove attraction from schedule",
        }));
      throw new Error(
        error.message || "Failed to remove attraction from schedule"
      );
    }
  },
};
