import { API_ENDPOINTS } from "@/lib/api";
import type { ScheduleWithAttractions } from "@/types/schedule.types";

export const scheduleManagementService = {
  async getSchedulesWithAttractions(): Promise<ScheduleWithAttractions[]> {
    const response = await fetch(API_ENDPOINTS.dashboard.schedulesWithAttractions, {
      credentials: "include",
    });

    if (!response.ok) {
      throw new Error("Failed to fetch schedules with attractions");
    }

    return response.json();
  },
};
