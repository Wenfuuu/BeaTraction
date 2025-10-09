import { API_ENDPOINTS } from "@/lib/api";
import type { Attraction, CreateAttractionRequest, UpdateAttractionRequest } from "@/types/attraction.types";

export const attractionService = {
  async getAll(): Promise<Attraction[]> {
    const response = await fetch(API_ENDPOINTS.attractions.getAll, {
      credentials: "include",
    });

    if (!response.ok) {
      throw new Error("Failed to fetch attractions");
    }

    return response.json();
  },

  async getById(id: string): Promise<Attraction> {
    const response = await fetch(API_ENDPOINTS.attractions.getById(id), {
      credentials: "include",
    });

    if (!response.ok) {
      throw new Error("Failed to fetch attraction");
    }

    return response.json();
  },

  async create(data: CreateAttractionRequest): Promise<Attraction> {
    const formData = new FormData();
    formData.append("Name", data.name);
    formData.append("Description", data.description);
    formData.append("Capacity", data.capacity.toString());
    
    if (data.image) {
      formData.append("Image", data.image);
    }

    const response = await fetch(API_ENDPOINTS.attractions.create, {
      method: "POST",
      credentials: "include",
      body: formData,
    });

    if (!response.ok) {
      const error = await response.json().catch(() => ({ message: "Failed to create attraction" }));
      throw new Error(error.message || "Failed to create attraction");
    }

    return response.json();
  },

  async update(id: string, data: UpdateAttractionRequest): Promise<Attraction> {
    const formData = new FormData();
    formData.append("Name", data.name);
    formData.append("Description", data.description);
    formData.append("Capacity", data.capacity.toString());
    
    if (data.image) {
      formData.append("Image", data.image);
    }

    const response = await fetch(API_ENDPOINTS.attractions.update(id), {
      method: "PUT",
      credentials: "include",
      body: formData,
    });

    if (!response.ok) {
      const error = await response.json().catch(() => ({ message: "Failed to update attraction" }));
      throw new Error(error.message || "Failed to update attraction");
    }

    return response.json();
  },

  async delete(id: string): Promise<void> {
    const response = await fetch(API_ENDPOINTS.attractions.delete(id), {
      method: "DELETE",
      credentials: "include",
    });

    if (!response.ok) {
      const error = await response.json().catch(() => ({ message: "Failed to delete attraction" }));
      throw new Error(error.message || "Failed to delete attraction");
    }
  },
};
