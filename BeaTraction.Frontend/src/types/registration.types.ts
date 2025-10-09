export interface Registration {
  id: string;
  userId: string;
  scheduleId: string;
  registeredAt: string;
  rowVersion: number;
}

export interface RegistrationWithDetails extends Registration {
  user?: {
    id: string;
    name: string;
    email: string;
  };
  schedule?: {
    id: string;
    name: string;
    startTime: string;
    endTime: string;
    attraction?: {
      id: string;
      name: string;
      capacity: number;
    };
  };
}

export interface CreateRegistrationRequest {
  userId: string;
  scheduleId: string;
}

export interface AttractionRegistrationStats {
  attractionId: string;
  attractionName: string;
  capacity: number;
  totalRegistrations: number;
  schedules: {
    scheduleId: string;
    scheduleName: string;
    startTime: string;
    endTime: string;
    registrationCount: number;
  }[];
}
