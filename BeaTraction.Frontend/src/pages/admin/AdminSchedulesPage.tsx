import { useState } from 'react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import AdminLayout from '@/components/layouts/AdminLayout'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from '@/components/ui/dialog'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'
import type { Schedule, CreateScheduleRequest } from '@/types/schedule.types'
import type { Attraction } from '@/types/attraction.types'
import { Plus, Pencil, Trash2, Calendar, Clock } from 'lucide-react'

export default function AdminSchedulesPage() {
  const [attractions] = useState<Attraction[]>([
    {
      id: '1',
      name: 'Roller Coaster',
      description: 'An exciting roller coaster ride',
      imageUrl: null,
      capacity: 50,
      createdAt: new Date().toISOString(),
      rowVersion: 1,
    },
    {
      id: '2',
      name: 'Ferris Wheel',
      description: 'Enjoy panoramic views',
      imageUrl: null,
      capacity: 40,
      createdAt: new Date().toISOString(),
      rowVersion: 1,
    },
  ])

  const [schedules, setSchedules] = useState<Schedule[]>([
    {
      id: '1',
      attractionId: '1',
      name: 'Morning Session',
      startTime: '2025-10-10T09:00:00',
      endTime: '2025-10-10T12:00:00',
      rowVersion: 1,
    },
    {
      id: '2',
      attractionId: '1',
      name: 'Afternoon Session',
      startTime: '2025-10-10T13:00:00',
      endTime: '2025-10-10T16:00:00',
      rowVersion: 1,
    },
    {
      id: '3',
      attractionId: '2',
      name: 'Evening Session',
      startTime: '2025-10-10T17:00:00',
      endTime: '2025-10-10T20:00:00',
      rowVersion: 1,
    },
  ])

  const [isCreateDialogOpen, setIsCreateDialogOpen] = useState(false)
  const [isEditDialogOpen, setIsEditDialogOpen] = useState(false)
  const [selectedSchedule, setSelectedSchedule] = useState<Schedule | null>(null)

  const [formData, setFormData] = useState<CreateScheduleRequest>({
    attractionId: '',
    name: '',
    startTime: '',
    endTime: '',
  })

  const getAttractionName = (attractionId: string) => {
    return attractions.find(a => a.id === attractionId)?.name || 'Unknown'
  }

  const formatDateTime = (dateString: string) => {
    return new Date(dateString).toLocaleString('en-US', {
      dateStyle: 'medium',
      timeStyle: 'short',
    })
  }

  const handleCreate = () => {
    console.log('Create schedule:', formData)
    // TODO: API call
    setIsCreateDialogOpen(false)
    resetForm()
  }

  const handleEdit = (schedule: Schedule) => {
    setSelectedSchedule(schedule)
    setFormData({
      attractionId: schedule.attractionId,
      name: schedule.name,
      startTime: schedule.startTime,
      endTime: schedule.endTime,
    })
    setIsEditDialogOpen(true)
  }

  const handleUpdate = () => {
    console.log('Update schedule:', selectedSchedule?.id, formData)
    // TODO: API call
    setIsEditDialogOpen(false)
    resetForm()
  }

  const handleDelete = (id: string) => {
    if (confirm('Are you sure you want to delete this schedule?')) {
      console.log('Delete schedule:', id)
      // TODO: API call
      setSchedules(schedules.filter(s => s.id !== id))
    }
  }

  const resetForm = () => {
    setFormData({
      attractionId: '',
      name: '',
      startTime: '',
      endTime: '',
    })
    setSelectedSchedule(null)
  }

  // Group schedules by attraction
  const schedulesByAttraction = schedules.reduce((acc, schedule) => {
    if (!acc[schedule.attractionId]) {
      acc[schedule.attractionId] = []
    }
    acc[schedule.attractionId].push(schedule)
    return acc
  }, {} as Record<string, Schedule[]>)

  return (
    <AdminLayout>
      <div className="container mx-auto py-8 px-4">
      <div className="flex justify-between items-center mb-8">
        <div>
          <h1 className="text-3xl font-bold">Manage Schedules</h1>
          <p className="text-gray-600 mt-2">Create and manage time slots for attractions</p>
        </div>

        <Dialog open={isCreateDialogOpen} onOpenChange={setIsCreateDialogOpen}>
          <DialogTrigger asChild>
            <Button onClick={() => setIsCreateDialogOpen(true)}>
              <Plus className="mr-2 h-4 w-4" />
              Add Schedule
            </Button>
          </DialogTrigger>
          <DialogContent className="max-w-xl">
            <DialogHeader>
              <DialogTitle>Create New Schedule</DialogTitle>
              <DialogDescription>
                Add a new time slot for an attraction
              </DialogDescription>
            </DialogHeader>

            <div className="space-y-4 py-4">
              <div className="space-y-2">
                <Label htmlFor="attraction">Attraction</Label>
                <Select
                  value={formData.attractionId}
                  onValueChange={(value) => setFormData({ ...formData, attractionId: value })}
                >
                  <SelectTrigger id="attraction">
                    <SelectValue placeholder="Select an attraction" />
                  </SelectTrigger>
                  <SelectContent>
                    {attractions.map((attraction) => (
                      <SelectItem key={attraction.id} value={attraction.id}>
                        {attraction.name}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>

              <div className="space-y-2">
                <Label htmlFor="scheduleName">Schedule Name</Label>
                <Input
                  id="scheduleName"
                  placeholder="e.g., Morning Session"
                  value={formData.name}
                  onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                />
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div className="space-y-2">
                  <Label htmlFor="startTime">Start Time</Label>
                  <Input
                    id="startTime"
                    type="datetime-local"
                    value={formData.startTime}
                    onChange={(e) => setFormData({ ...formData, startTime: e.target.value })}
                  />
                </div>

                <div className="space-y-2">
                  <Label htmlFor="endTime">End Time</Label>
                  <Input
                    id="endTime"
                    type="datetime-local"
                    value={formData.endTime}
                    onChange={(e) => setFormData({ ...formData, endTime: e.target.value })}
                  />
                </div>
              </div>
            </div>

            <DialogFooter>
              <Button variant="outline" onClick={() => setIsCreateDialogOpen(false)}>
                Cancel
              </Button>
              <Button onClick={handleCreate}>Create Schedule</Button>
            </DialogFooter>
          </DialogContent>
        </Dialog>
      </div>

      <div className="space-y-6">
        {Object.entries(schedulesByAttraction).map(([attractionId, attractionSchedules]) => (
          <Card key={attractionId}>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Calendar className="h-5 w-5" />
                {getAttractionName(attractionId)}
              </CardTitle>
              <CardDescription>
                {attractionSchedules.length} schedule(s) available
              </CardDescription>
            </CardHeader>
            <CardContent>
              <div className="space-y-3">
                {attractionSchedules.map((schedule) => (
                  <div
                    key={schedule.id}
                    className="flex items-center justify-between p-4 border rounded-lg hover:bg-gray-50"
                  >
                    <div className="flex-1">
                      <h4 className="font-semibold">{schedule.name}</h4>
                      <div className="flex items-center gap-4 mt-2 text-sm text-gray-600">
                        <div className="flex items-center gap-1">
                          <Clock className="h-4 w-4" />
                          <span>{formatDateTime(schedule.startTime)}</span>
                        </div>
                        <span>→</span>
                        <span>{formatDateTime(schedule.endTime)}</span>
                      </div>
                    </div>
                    <div className="flex gap-2">
                      <Button
                        variant="outline"
                        size="sm"
                        onClick={() => handleEdit(schedule)}
                      >
                        <Pencil className="h-4 w-4" />
                      </Button>
                      <Button
                        variant="destructive"
                        size="sm"
                        onClick={() => handleDelete(schedule.id)}
                      >
                        <Trash2 className="h-4 w-4" />
                      </Button>
                    </div>
                  </div>
                ))}
              </div>
            </CardContent>
          </Card>
        ))}

        {Object.keys(schedulesByAttraction).length === 0 && (
          <Card>
            <CardContent className="py-12 text-center">
              <Calendar className="h-12 w-12 mx-auto text-gray-400 mb-4" />
              <h3 className="text-lg font-semibold mb-2">No schedules yet</h3>
              <p className="text-gray-600 mb-4">Create your first schedule to get started</p>
            </CardContent>
          </Card>
        )}
      </div>

      <Dialog open={isEditDialogOpen} onOpenChange={setIsEditDialogOpen}>
        <DialogContent className="max-w-xl">
          <DialogHeader>
            <DialogTitle>Edit Schedule</DialogTitle>
            <DialogDescription>
              Update schedule details
            </DialogDescription>
          </DialogHeader>

          <div className="space-y-4 py-4">
            <div className="space-y-2">
              <Label htmlFor="edit-attraction">Attraction</Label>
              <Select
                value={formData.attractionId}
                onValueChange={(value) => setFormData({ ...formData, attractionId: value })}
              >
                <SelectTrigger id="edit-attraction">
                  <SelectValue placeholder="Select an attraction" />
                </SelectTrigger>
                <SelectContent>
                  {attractions.map((attraction) => (
                    <SelectItem key={attraction.id} value={attraction.id}>
                      {attraction.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            <div className="space-y-2">
              <Label htmlFor="edit-scheduleName">Schedule Name</Label>
              <Input
                id="edit-scheduleName"
                placeholder="e.g., Morning Session"
                value={formData.name}
                onChange={(e) => setFormData({ ...formData, name: e.target.value })}
              />
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div className="space-y-2">
                <Label htmlFor="edit-startTime">Start Time</Label>
                <Input
                  id="edit-startTime"
                  type="datetime-local"
                  value={formData.startTime}
                  onChange={(e) => setFormData({ ...formData, startTime: e.target.value })}
                />
              </div>

              <div className="space-y-2">
                <Label htmlFor="edit-endTime">End Time</Label>
                <Input
                  id="edit-endTime"
                  type="datetime-local"
                  value={formData.endTime}
                  onChange={(e) => setFormData({ ...formData, endTime: e.target.value })}
                />
              </div>
            </div>
          </div>

          <DialogFooter>
            <Button variant="outline" onClick={() => setIsEditDialogOpen(false)}>
              Cancel
            </Button>
            <Button onClick={handleUpdate}>Update Schedule</Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
      </div>
    </AdminLayout>
  )
}
