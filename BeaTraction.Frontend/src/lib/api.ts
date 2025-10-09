const API_BASE_URL =
  import.meta.env.VITE_API_URL || "http://localhost:5000/api";

export const API_ENDPOINTS = {
  auth: {
    login: `${API_BASE_URL}/Users/login`,
    register: `${API_BASE_URL}/Users/register`,
    logout: `${API_BASE_URL}/Users/logout`,
    me: `${API_BASE_URL}/Users/me`,
  },
  attractions: {
    getAll: `${API_BASE_URL}/Attractions`,
    getById: (id: string) => `${API_BASE_URL}/Attractions/${id}`,
    create: `${API_BASE_URL}/Attractions`,
    update: (id: string) => `${API_BASE_URL}/Attractions/${id}`,
    delete: (id: string) => `${API_BASE_URL}/Attractions/${id}`,
  },
  schedules: {
    getAll: `${API_BASE_URL}/Schedules`,
    getById: (id: string) => `${API_BASE_URL}/Schedules/${id}`,
    create: `${API_BASE_URL}/Schedules`,
    update: (id: string) => `${API_BASE_URL}/Schedules/${id}`,
    delete: (id: string) => `${API_BASE_URL}/Schedules/${id}`,
  },
  scheduleAttractions: {
    getAll: `${API_BASE_URL}/ScheduleAttractions`,
    getById: (id: string) => `${API_BASE_URL}/ScheduleAttractions/${id}`,
    getByScheduleId: (scheduleId: string) => `${API_BASE_URL}/ScheduleAttractions/schedule/${scheduleId}`,
    getByAttractionId: (attractionId: string) => `${API_BASE_URL}/ScheduleAttractions/attraction/${attractionId}`,
    create: `${API_BASE_URL}/ScheduleAttractions`,
    delete: (id: string) => `${API_BASE_URL}/ScheduleAttractions/${id}`,
  },
  registrations: {
    getAll: `${API_BASE_URL}/Registrations`,
    create: `${API_BASE_URL}/Registrations`,
  },
};
