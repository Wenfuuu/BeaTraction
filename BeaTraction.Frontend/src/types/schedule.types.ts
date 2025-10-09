export interface Schedule {
  id: string;
  name: string;
  startTime: string;
  endTime: string;
  rowVersion: number;
}

export interface ScheduleWithAttractions extends Schedule {
  attractions?: {
    id: string;
    scheduleAttractionId: string;
    name: string;
    description: string;
    imageUrl?: string;
    capacity: number;
  }[];
}

export interface CreateScheduleRequest {
  name: string;
  startTime: string;
  endTime: string;
}

export interface UpdateScheduleRequest {
  id: string;
  name: string;
  startTime: string;
  endTime: string;
  rowVersion: number;
}
