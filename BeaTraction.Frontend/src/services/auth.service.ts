import type { AuthResponse, LoginCredentials, RegisterCredentials } from '@/types/auth.types'
import { API_ENDPOINTS } from '@/lib/api'

export const authService = {
  async login(credentials: LoginCredentials): Promise<AuthResponse> {
    const response = await fetch(API_ENDPOINTS.auth.login, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      credentials: 'include', 
      body: JSON.stringify(credentials),
    })

    if (!response.ok) {
      const error = await response.json().catch(() => ({ message: 'Login failed' }))
      throw new Error(error.message || 'Login failed')
    }

    const data = await response.json()
    
    if (data.user) {
      localStorage.setItem('user', JSON.stringify(data.user))
    }
    
    return data
  },

  async register(credentials: RegisterCredentials): Promise<AuthResponse> {
    const registrationData = {
      ...credentials,
      role: credentials.role || 'user',
    }
    
    const response = await fetch(API_ENDPOINTS.auth.register, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      credentials: 'include',
      body: JSON.stringify(registrationData),
    })

    if (!response.ok) {
      const error = await response.json().catch(() => ({ message: 'Registration failed' }))
      throw new Error(error.message || 'Registration failed')
    }

    const data = await response.json()
    
    if (data.user) {
      localStorage.setItem('user', JSON.stringify(data.user))
    }
    
    return data
  },

  async logout() {
    localStorage.removeItem('user')
    
    try {
      await fetch(API_ENDPOINTS.auth.logout, {
        method: 'POST',
        credentials: 'include',
      })
    } catch (error) {
      console.error('Logout error:', error)
    }
  },

  async getMe() {
    const response = await fetch(API_ENDPOINTS.auth.me, {
      credentials: 'include',
    })

    if (!response.ok) {
      throw new Error('Not authenticated')
    }

    const user = await response.json()
    localStorage.setItem('user', JSON.stringify(user))
    return user
  },

  getUser(): string | null {
    return localStorage.getItem('user')
  },

  isAuthenticated(): boolean {
    return !!this.getUser()
  },
}
