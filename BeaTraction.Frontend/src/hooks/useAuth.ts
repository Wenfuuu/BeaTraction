import { useState, useEffect } from 'react'
import type { User } from '@/types/auth.types'
import { authService } from '@/services/auth.service'

export function useAuth() {
  const [user, setUser] = useState<User | null>(null)
  const [isLoading, setIsLoading] = useState(true)

  useEffect(() => {
    // check if user is logged in
    const storedUser = localStorage.getItem('user')
    if (storedUser) {
      setUser(JSON.parse(storedUser))
    }
    setIsLoading(false)
  }, [])

  const login = (userData: User) => {
    setUser(userData)
  }

  const logout = async () => {
    await authService.logout()
    setUser(null)
  }

  return {
    user,
    isLoading,
    isAuthenticated: !!user,
    login,
    logout,
  }
}
