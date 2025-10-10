export interface Schedule {
  id: string;
  name: string;
  startTime: string;
  endTime: string;
  rowVersion: number;
}

export interface AttractionInfo {
  scheduleAttractionId: string;
  attractionId: string;
  attractionName: string;
  description: string;
  imageUrl?: string;
  capacity: number;
}

export interface ScheduleWithAttractions {
  scheduleId: string;
  scheduleName: string;
  startTime: string;
  endTime: string;
  rowVersion: number;
  attractions: AttractionInfo[];
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
