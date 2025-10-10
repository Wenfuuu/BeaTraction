import { attractionService } from "./attractionService";
import { scheduleAttractionService } from "./scheduleAttractionService";
import { registrationService } from "./registrationService";
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
      const [attractions, scheduleAttractions, allRegistrations] = await Promise.all([
        attractionService.getAll(),
        scheduleAttractionService.getAll(),
        registrationService.getAll(),
      ]);

      const userRegistrations = allRegistrations.filter(r => r.userId === userId);
      const userRegistrationIds = new Set(userRegistrations.map(r => r.scheduleAttractionId));

      const attractionsWithSchedules: AttractionWithSchedules[] = attractions.map(attraction => {
        // Find all schedule-attractions for this attraction
        const attractionSchedules = scheduleAttractions.filter(
          sa => sa.attractionId === attraction.id
        );

        // Build schedule stats
        const scheduleStats: ScheduleAttractionWithStats[] = attractionSchedules.map(sa => {
          // Count registrations for this schedule-attraction
          const registrationCount = allRegistrations.filter(
            r => r.scheduleAttractionId === sa.id
          ).length;

          // Check if user is registered
          const isRegistered = userRegistrationIds.has(sa.id);

          return {
            scheduleAttractionId: sa.id,
            scheduleId: sa.scheduleId,
            scheduleName: sa.schedule?.name || "Unknown Schedule",
            startTime: sa.schedule?.startTime || "",
            endTime: sa.schedule?.endTime || "",
            registrationCount,
            isRegistered,
          };
        });

        return {
          ...attraction,
          scheduleAttractions: scheduleStats,
        };
      });

      return attractionsWithSchedules
    } catch (error) {
      console.error("Error fetching attractions with schedules:", error);
      throw error;
    }
  },
};
