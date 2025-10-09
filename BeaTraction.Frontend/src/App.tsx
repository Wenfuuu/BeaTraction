import { Routes, Route } from 'react-router'
import './App.css'
import { Toaster } from '@/components/ui/sonner'
import LoginPage from './pages/auth/LoginPage'
import RegisterPage from './pages/auth/RegisterPage'

function App() {
  return (
    <>
      <Routes>
        <Route path="/" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
      </Routes>
      <Toaster />
    </>
  )
}

export default App
