export interface User {
  id: string
  name: string
  email: string
  role: string
  createdAt: string
}

export interface LoginCredentials {
  email: string
  password: string
}

export interface RegisterCredentials {
  name: string
  email: string
  password: string
  role?: string // Optional, defaults to "user"
}

export interface AuthResponse {
  token: string
  user: User
}
