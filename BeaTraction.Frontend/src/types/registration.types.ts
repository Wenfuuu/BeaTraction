export interface Registration {
  id: string;
  userId: string;
  scheduleAttractionId: string;
  registeredAt: string;
  rowVersion: number;
}

export interface RegistrationWithDetails extends Registration {
  user?: {
    id: string;
    name: string;
    email: string;
  };
  scheduleAttraction?: {
    id: string;
    scheduleId: string;
    attractionId: string;
  };
}

export interface CreateRegistrationRequest {
  userId: string;
  scheduleAttractionId: string;
}

export interface AttractionRegistrationStats {
  attractionId: string;
  attractionName: string;
  capacity: number;
  totalRegistrations: number;
  scheduleAttractions: {
    scheduleAttractionId: string;
    scheduleId: string;
    scheduleName: string;
    startTime: string;
    endTime: string;
    registrationCount: number;
    availableSpots: number;
    isFull: boolean;
  }[];
}
