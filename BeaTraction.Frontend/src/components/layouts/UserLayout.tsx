import type { ReactNode } from 'react'
import { Link, useNavigate } from 'react-router'
import { Button } from '@/components/ui/button'
import { useAuthContext } from '@/contexts/AuthContext'
import { Ticket, LogOut, Menu } from 'lucide-react'
import { useState } from 'react'

interface UserLayoutProps {
  children: ReactNode
}

export default function UserLayout({ children }: UserLayoutProps) {
  const navigate = useNavigate()
  const { user, logout } = useAuthContext()
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false)

  const handleLogout = async () => {
    await logout()
    navigate('/')
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <header className="bg-white border-b border-gray-200 sticky top-0 z-10">
        <div className="container mx-auto px-4">
          <div className="flex items-center justify-between h-16">
            <div className="flex items-center gap-8">
              <Link to="/attractions" className="font-bold text-xl">
                BeaTraction
              </Link>
              
              <nav className="hidden md:flex items-center gap-1">
                <Link to="/attractions">
                  <Button variant="ghost" className="gap-2">
                    <Ticket className="h-4 w-4" />
                    Attractions
                  </Button>
                </Link>
              </nav>
            </div>

            <div className="hidden md:flex items-center gap-4">
              <span className="text-sm text-gray-600">Welcome, {user?.name || 'Guest'}</span>
              <Button variant="outline" onClick={handleLogout} className="gap-2">
                <LogOut className="h-4 w-4" />
                Logout
              </Button>
            </div>

            <Button
              variant="ghost"
              size="sm"
              className="md:hidden"
              onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
            >
              <Menu className="h-5 w-5" />
            </Button>
          </div>

          {isMobileMenuOpen && (
            <nav className="md:hidden py-4 border-t">
              <div className="flex flex-col gap-2">
                <div className="px-4 py-2 text-sm text-gray-600">
                  Welcome, {user?.name || 'Guest'}
                </div>
                <Link to="/attractions" onClick={() => setIsMobileMenuOpen(false)}>
                  <Button variant="ghost" className="w-full justify-start gap-2">
                    <Ticket className="h-4 w-4" />
                    Attractions
                  </Button>
                </Link>
                <Button
                  variant="outline"
                  onClick={handleLogout}
                  className="w-full justify-start gap-2"
                >
                  <LogOut className="h-4 w-4" />
                  Logout
                </Button>
              </div>
            </nav>
          )}
        </div>
      </header>

      <main>{children}</main>
    </div>
  )
}
