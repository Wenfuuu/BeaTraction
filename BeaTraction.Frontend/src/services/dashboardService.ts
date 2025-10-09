import { attractionService } from "./attractionService";
import { scheduleService } from "./scheduleService";
import { scheduleAttractionService } from "./scheduleAttractionService";
import { registrationService } from "./registrationService";
import type { AttractionRegistrationStats } from "@/types/registration.types";

export const dashboardService = {
  async getAttractionStats(): Promise<AttractionRegistrationStats[]> {
    const [attractions, schedules, scheduleAttractions, registrations] = await Promise.all([
      attractionService.getAll(),
      scheduleService.getAll(),
      scheduleAttractionService.getAll(),
      registrationService.getAll(),
    ]);

    const stats: AttractionRegistrationStats[] = attractions.map((attraction) => {
      const attractionSchedules = scheduleAttractions.filter(
        (sa) => sa.attractionId === attraction.id
      );

      const scheduleAttractionStats = attractionSchedules.map((sa) => {
        const schedule = schedules.find((s) => s.id === sa.scheduleId);
        
        const registrationCount = registrations.filter(
          (r) => r.scheduleAttractionId === sa.id
        ).length;

        return {
          scheduleAttractionId: sa.id,
          scheduleId: sa.scheduleId,
          scheduleName: schedule?.name || "Unknown Schedule",
          startTime: schedule?.startTime || "",
          endTime: schedule?.endTime || "",
          registrationCount,
        };
      });

      const totalRegistrations = scheduleAttractionStats.reduce(
        (sum, s) => sum + s.registrationCount,
        0
      );

      return {
        attractionId: attraction.id,
        attractionName: attraction.name,
        capacity: attraction.capacity,
        totalRegistrations,
        scheduleAttractions: scheduleAttractionStats,
      };
    });

    return stats;
  },
};
