export interface ScheduleAttraction {
  id: string;
  scheduleId: string;
  attractionId: string;
  rowVersion: number;
}

export interface ScheduleAttractionWithDetails extends ScheduleAttraction {
  schedule?: {
    id: string;
    name: string;
    startTime: string;
    endTime: string;
  };
  attraction?: {
    id: string;
    name: string;
    description: string;
    imageUrl?: string;
    capacity: number;
  };
}

export interface CreateScheduleAttractionRequest {
  scheduleId: string;
  attractionId: string;
}
