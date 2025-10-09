import { useState } from 'react'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'
import UserLayout from '@/components/layouts/UserLayout'
import { useAuthContext } from '@/contexts/AuthContext'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog'
import type { Attraction } from '@/types/attraction.types'
import type { Schedule } from '@/types/schedule.types'
import { toast } from '@/lib/toast'
import { Calendar, Clock, Users, CheckCircle, Image as ImageIcon } from 'lucide-react'

interface AttractionWithSchedules extends Attraction {
  schedules: (Schedule & { 
    registrationCount: number
    isRegistered: boolean
  })[]
}

export default function UserRegistrationPage() {
  const { user } = useAuthContext()
  const currentUserId = user?.id || '123'

  const [attractions] = useState<AttractionWithSchedules[]>([
    {
      id: '1',
      name: 'Roller Coaster',
      description: 'Experience the thrill of loops, drops, and high-speed turns on our world-class roller coaster!',
      imageUrl: 'https://via.placeholder.com/600x400',
      capacity: 50,
      createdAt: new Date().toISOString(),
      rowVersion: 1,
      schedules: [
        {
          id: '1',
          attractionId: '1',
          name: 'Morning Session',
          startTime: '2025-10-10T09:00:00',
          endTime: '2025-10-10T12:00:00',
          rowVersion: 1,
          registrationCount: 20,
          isRegistered: false,
        },
        {
          id: '2',
          attractionId: '1',
          name: 'Afternoon Session',
          startTime: '2025-10-10T13:00:00',
          endTime: '2025-10-10T16:00:00',
          rowVersion: 1,
          registrationCount: 15,
          isRegistered: false,
        },
      ],
    },
    {
      id: '2',
      name: 'Ferris Wheel',
      description: 'Enjoy breathtaking panoramic views of the entire theme park from our giant Ferris wheel.',
      imageUrl: null,
      capacity: 40,
      createdAt: new Date().toISOString(),
      rowVersion: 1,
      schedules: [
        {
          id: '3',
          attractionId: '2',
          name: 'Evening Session',
          startTime: '2025-10-10T17:00:00',
          endTime: '2025-10-10T20:00:00',
          rowVersion: 1,
          registrationCount: 38,
          isRegistered: true,
        },
      ],
    },
    {
      id: '3',
      name: 'Haunted House',
      description: 'Dare to enter our spine-chilling haunted house filled with scares and surprises at every turn!',
      imageUrl: 'https://via.placeholder.com/600x400/333',
      capacity: 30,
      createdAt: new Date().toISOString(),
      rowVersion: 1,
      schedules: [
        {
          id: '4',
          attractionId: '3',
          name: 'Night Session',
          startTime: '2025-10-10T19:00:00',
          endTime: '2025-10-10T22:00:00',
          rowVersion: 1,
          registrationCount: 10,
          isRegistered: false,
        },
      ],
    },
  ])

  const [selectedSchedule, setSelectedSchedule] = useState<{
    attraction: Attraction
    schedule: Schedule & { registrationCount: number; isRegistered: boolean }
  } | null>(null)

  const formatDateTime = (dateString: string) => {
    return new Date(dateString).toLocaleString('en-US', {
      dateStyle: 'medium',
      timeStyle: 'short',
    })
  }

  const getAvailableSpots = (capacity: number, registered: number) => {
    return capacity - registered
  }

  const getUtilizationPercentage = (registered: number, capacity: number) => {
    return Math.round((registered / capacity) * 100)
  }

  const handleRegisterClick = (
    attraction: Attraction,
    schedule: Schedule & { registrationCount: number; isRegistered: boolean }
  ) => {
    setSelectedSchedule({ attraction, schedule })
  }

  const handleConfirmRegistration = () => {
    if (!selectedSchedule) return

    console.log('Register for:', {
      userId: currentUserId,
      scheduleId: selectedSchedule.schedule.id,
      attractionId: selectedSchedule.attraction.id,
    })

    // TODO: API call
    toast.success('Registration successful!', {
      description: `You're registered for ${selectedSchedule.attraction.name} - ${selectedSchedule.schedule.name}`,
    })

    setSelectedSchedule(null)
  }

  const handleCancelRegistration = (scheduleId: string, attractionName: string) => {
    console.log('Cancel registration for schedule:', scheduleId)
    
    // TODO: API call
    toast.success('Registration cancelled', {
      description: `Your registration for ${attractionName} has been cancelled.`,
    })
  }

  return (
    <UserLayout>
      <div className="container mx-auto py-8 px-4">
      <div className="mb-8">
        <h1 className="text-3xl font-bold">Available Attractions</h1>
        <p className="text-gray-600 mt-2">Browse and register for your favorite attractions</p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {attractions.map((attraction) => (
          <Card key={attraction.id} className="overflow-hidden flex flex-col">
            <div className="aspect-video bg-gray-100 relative">
              {attraction.imageUrl ? (
                <img
                  src={attraction.imageUrl}
                  alt={attraction.name}
                  className="w-full h-full object-cover"
                />
              ) : (
                <div className="w-full h-full flex items-center justify-center">
                  <ImageIcon className="h-16 w-16 text-gray-400" />
                </div>
              )}
            </div>
            
            <CardHeader>
              <CardTitle>{attraction.name}</CardTitle>
              <CardDescription className="line-clamp-3">
                {attraction.description}
              </CardDescription>
            </CardHeader>

            <CardContent className="flex-1">
              <div className="flex items-center gap-2 mb-4 text-sm text-gray-600">
                <Users className="h-4 w-4" />
                <span>Capacity: {attraction.capacity} people</span>
              </div>

              <div className="space-y-3">
                <h4 className="font-semibold text-sm">Available Schedules:</h4>
                {attraction.schedules.map((schedule) => {
                  const availableSpots = getAvailableSpots(attraction.capacity, schedule.registrationCount)
                  const utilization = getUtilizationPercentage(schedule.registrationCount, attraction.capacity)
                  const isFull = availableSpots <= 0

                  return (
                    <div
                      key={schedule.id}
                      className="border rounded-lg p-3 space-y-2"
                    >
                      <div className="flex items-start justify-between">
                        <div className="flex-1">
                          <h5 className="font-semibold text-sm">{schedule.name}</h5>
                          <div className="flex items-center gap-1 text-xs text-gray-600 mt-1">
                            <Calendar className="h-3 w-3" />
                            <span>{formatDateTime(schedule.startTime)}</span>
                          </div>
                          <div className="flex items-center gap-1 text-xs text-gray-600">
                            <Clock className="h-3 w-3" />
                            <span>{formatDateTime(schedule.endTime)}</span>
                          </div>
                        </div>
                        {schedule.isRegistered && (
                          <Badge variant="default" className="bg-green-500">
                            <CheckCircle className="h-3 w-3 mr-1" />
                            Registered
                          </Badge>
                        )}
                      </div>

                      <div className="flex items-center justify-between text-xs">
                        <span className={isFull ? 'text-red-600 font-semibold' : 'text-gray-600'}>
                          {isFull ? 'Fully Booked' : `${availableSpots} spots left`}
                        </span>
                        <span className={utilization >= 90 ? 'text-red-600' : 'text-gray-600'}>
                          {utilization}% full
                        </span>
                      </div>

                      {schedule.isRegistered ? (
                        <Button
                          variant="outline"
                          size="sm"
                          className="w-full"
                          onClick={() => handleCancelRegistration(schedule.id, attraction.name)}
                        >
                          Cancel Registration
                        </Button>
                      ) : (
                        <Button
                          size="sm"
                          className="w-full"
                          disabled={isFull}
                          onClick={() => handleRegisterClick(attraction, schedule)}
                        >
                          {isFull ? 'Fully Booked' : 'Register Now'}
                        </Button>
                      )}
                    </div>
                  )
                })}
              </div>
            </CardContent>
          </Card>
        ))}
      </div>

      {attractions.length === 0 && (
        <Card>
          <CardContent className="py-12 text-center">
            <Calendar className="h-12 w-12 mx-auto text-gray-400 mb-4" />
            <h3 className="text-lg font-semibold mb-2">No attractions available</h3>
            <p className="text-gray-600">Check back later for new attractions!</p>
          </CardContent>
        </Card>
      )}

      <Dialog open={!!selectedSchedule} onOpenChange={() => setSelectedSchedule(null)}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Confirm Registration</DialogTitle>
            <DialogDescription>
              Please confirm your registration details
            </DialogDescription>
          </DialogHeader>

          {selectedSchedule && (
            <div className="space-y-4 py-4">
              <div className="border-l-4 border-blue-500 bg-blue-50 p-4 rounded">
                <h4 className="font-semibold text-lg">{selectedSchedule.attraction.name}</h4>
                <p className="text-sm text-gray-600 mt-1">{selectedSchedule.attraction.description}</p>
              </div>

              <div className="space-y-2">
                <div className="flex items-center justify-between">
                  <span className="text-sm text-gray-600">Schedule:</span>
                  <span className="font-semibold">{selectedSchedule.schedule.name}</span>
                </div>
                <div className="flex items-center justify-between">
                  <span className="text-sm text-gray-600">Start Time:</span>
                  <span className="font-semibold">{formatDateTime(selectedSchedule.schedule.startTime)}</span>
                </div>
                <div className="flex items-center justify-between">
                  <span className="text-sm text-gray-600">End Time:</span>
                  <span className="font-semibold">{formatDateTime(selectedSchedule.schedule.endTime)}</span>
                </div>
                <div className="flex items-center justify-between">
                  <span className="text-sm text-gray-600">Available Spots:</span>
                  <span className="font-semibold">
                    {getAvailableSpots(selectedSchedule.attraction.capacity, selectedSchedule.schedule.registrationCount)}
                  </span>
                </div>
              </div>
            </div>
          )}

          <DialogFooter>
            <Button variant="outline" onClick={() => setSelectedSchedule(null)}>
              Cancel
            </Button>
            <Button onClick={handleConfirmRegistration}>
              Confirm Registration
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
      </div>
    </UserLayout>
  )
}
