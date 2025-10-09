const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api'

export const API_ENDPOINTS = {
  auth: {
    login: `${API_BASE_URL}/Users/login`,
    register: `${API_BASE_URL}/Users/register`,
    logout: `${API_BASE_URL}/Users/logout`,
  },
  attractions: {
    getAll: `${API_BASE_URL}/Attractions`,
    getById: (id: string) => `${API_BASE_URL}/attractions/${id}`,
  },
  schedules: {
    getAll: `${API_BASE_URL}/Schedules`,
    getById: (id: string) => `${API_BASE_URL}/Schedules/${id}`,
  },
  registrations: {
    getAll: `${API_BASE_URL}/Registrations`,
    create: `${API_BASE_URL}/Registrations`,
  },
};
