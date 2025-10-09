export interface Attraction {
  id: string
  name: string
  description: string
  imageUrl: string | null
  capacity: number
  createdAt: string
  rowVersion: number
}

export interface CreateAttractionRequest {
  name: string
  description: string
  imageUrl?: string
  capacity: number
}

export interface UpdateAttractionRequest {
  id: string
  name: string
  description: string
  imageUrl?: string
  capacity: number
  rowVersion: number
}
