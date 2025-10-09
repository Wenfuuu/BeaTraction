export interface Schedule {
  id: string
  attractionId: string
  name: string
  startTime: string
  endTime: string
  rowVersion: number
}

export interface ScheduleWithAttraction extends Schedule {
  attraction?: {
    id: string
    name: string
    capacity: number
  }
}

export interface CreateScheduleRequest {
  attractionId: string
  name: string
  startTime: string
  endTime: string
}

export interface UpdateScheduleRequest {
  id: string
  attractionId: string
  name: string
  startTime: string
  endTime: string
  rowVersion: number
}
