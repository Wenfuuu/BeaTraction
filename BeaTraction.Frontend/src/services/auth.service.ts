import type { AuthResponse, LoginCredentials, RegisterCredentials } from '@/types/auth.types'
import { API_ENDPOINTS } from '@/lib/api'

export const authService = {
  async login(credentials: LoginCredentials): Promise<AuthResponse> {
    const response = await fetch(API_ENDPOINTS.auth.login, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      credentials: 'include', // Include cookies in request
      body: JSON.stringify(credentials),
    })

    if (!response.ok) {
      const error = await response.json().catch(() => ({ message: 'Login failed' }))
      throw new Error(error.message || 'Login failed')
    }

    const data = await response.json()
    
    // Store user data in localStorage for persistence
    if (data.user) {
      localStorage.setItem('user', JSON.stringify(data.user))
    }
    
    return data
  },

  async register(credentials: RegisterCredentials): Promise<AuthResponse> {
    // Set default role to "user" if not provided
    const registrationData = {
      ...credentials,
      role: credentials.role || 'user',
    }
    
    const response = await fetch(API_ENDPOINTS.auth.register, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      credentials: 'include', // Include cookies in request
      body: JSON.stringify(registrationData),
    })

    if (!response.ok) {
      const error = await response.json().catch(() => ({ message: 'Registration failed' }))
      throw new Error(error.message || 'Registration failed')
    }

    const data = await response.json()
    
    // Store user data in localStorage for persistence
    if (data.user) {
      localStorage.setItem('user', JSON.stringify(data.user))
    }
    
    return data
  },

  async logout() {
    // Clear localStorage
    localStorage.removeItem('user')
    
    // Optionally call logout endpoint to clear server-side session/cookie
    try {
      await fetch(API_ENDPOINTS.auth.logout, {
        method: 'POST',
        credentials: 'include',
      })
    } catch (error) {
      console.error('Logout error:', error)
    }
  },

  getUser(): string | null {
    return localStorage.getItem('user')
  },

  isAuthenticated(): boolean {
    // Check if user exists in localStorage
    return !!this.getUser()
  },
}
