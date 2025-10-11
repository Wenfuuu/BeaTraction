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

interface BackendAttractionResponse {
  attractionId: string;
  attractionName: string;
  description: string;
  imageUrl: string | null;
  capacity: number;
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

      const data: BackendAttractionResponse[] = await response.json();
      
      return data.map((item) => ({
        id: item.attractionId,
        name: item.attractionName,
        description: item.description,
        imageUrl: item.imageUrl,
        capacity: item.capacity,
        createdAt: new Date().toISOString(),
        rowVersion: 0,
        scheduleAttractions: item.scheduleAttractions,
      }));
    } catch (error) {
      console.error("Error fetching attractions with schedules:", error);
      throw error;
    }
  },
};
