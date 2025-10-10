import { API_ENDPOINTS } from "@/lib/api";
import type { Attraction } from "@/types/attraction.types";

export interface ScheduleAttractionWithStats {
  scheduleAttractionId: string;
  scheduleId: string;
  scheduleName: string;
  startTime: string;
  endTime: string;
  registrationCount: number;
  isRegistered: boolean;
}

export interface AttractionWithSchedules extends Attraction {
  scheduleAttractions: ScheduleAttractionWithStats[];
}

export const userRegistrationService = {
  async getAttractionsWithSchedules(userId: string): Promise<AttractionWithSchedules[]> {
    try {
      const response = await fetch(API_ENDPOINTS.dashboard.userAttractions(userId), {
        credentials: "include",
      });

      if (!response.ok) {
        throw new Error("Failed to fetch attractions with schedules");
      }

      return response.json();
    } catch (error) {
      console.error("Error fetching attractions with schedules:", error);
      throw error;
    }
  },
};
