import { API_ENDPOINTS } from "@/lib/api";
import type { AttractionRegistrationStats } from "@/types/registration.types";

export const dashboardService = {
  async getAttractionStats(): Promise<AttractionRegistrationStats[]> {
    const response = await fetch(API_ENDPOINTS.dashboard.attractionStats, {
      credentials: "include",
    });

    if (!response.ok) {
      throw new Error("Failed to fetch attraction statistics");
    }

    return response.json();
  },
};
