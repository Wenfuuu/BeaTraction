import { useState } from 'react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from '@/components/ui/card'
import { Textarea } from '@/components/ui/textarea'
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
import type { Attraction, CreateAttractionRequest } from '@/types/attraction.types'
import { Plus, Pencil, Trash2, Image as ImageIcon } from 'lucide-react'

export default function AdminAttractionsPage() {
  const [attractions, setAttractions] = useState<Attraction[]>([
    {
      id: '1',
      name: 'Roller Coaster',
      description: 'An exciting roller coaster ride with loops and turns!',
      imageUrl: 'https://via.placeholder.com/400x300',
      capacity: 50,
      createdAt: new Date().toISOString(),
      rowVersion: 1,
    },
    {
      id: '2',
      name: 'Ferris Wheel',
      description: 'Enjoy panoramic views from our giant Ferris wheel.',
      imageUrl: null,
      capacity: 40,
      createdAt: new Date().toISOString(),
      rowVersion: 1,
    },
  ])

  const [isCreateDialogOpen, setIsCreateDialogOpen] = useState(false)
  const [isEditDialogOpen, setIsEditDialogOpen] = useState(false)
  const [selectedAttraction, setSelectedAttraction] = useState<Attraction | null>(null)

  const [formData, setFormData] = useState<CreateAttractionRequest>({
    name: '',
    description: '',
    imageUrl: '',
    capacity: 0,
  })

  const handleCreate = () => {
    console.log('Create attraction:', formData)
    // TODO: API call
    setIsCreateDialogOpen(false)
    resetForm()
  }

  const handleEdit = (attraction: Attraction) => {
    setSelectedAttraction(attraction)
    setFormData({
      name: attraction.name,
      description: attraction.description,
      imageUrl: attraction.imageUrl || '',
      capacity: attraction.capacity,
    })
    setIsEditDialogOpen(true)
  }

  const handleUpdate = () => {
    console.log('Update attraction:', selectedAttraction?.id, formData)
    // TODO: API call
    setIsEditDialogOpen(false)
    resetForm()
  }

  const handleDelete = (id: string) => {
    if (confirm('Are you sure you want to delete this attraction?')) {
      console.log('Delete attraction:', id)
      // TODO: API call
      setAttractions(attractions.filter(a => a.id !== id))
    }
  }

  const resetForm = () => {
    setFormData({
      name: '',
      description: '',
      imageUrl: '',
      capacity: 0,
    })
    setSelectedAttraction(null)
  }

  return (
    <AdminLayout>
      <div className="container mx-auto py-8 px-4">
      <div className="flex justify-between items-center mb-8">
        <div>
          <h1 className="text-3xl font-bold">Manage Attractions</h1>
          <p className="text-gray-600 mt-2">Add, edit, or remove attractions</p>
        </div>

        <Dialog open={isCreateDialogOpen} onOpenChange={setIsCreateDialogOpen}>
          <DialogTrigger asChild>
            <Button onClick={() => setIsCreateDialogOpen(true)}>
              <Plus className="mr-2 h-4 w-4" />
              Add Attraction
            </Button>
          </DialogTrigger>
          <DialogContent className="max-w-2xl">
            <DialogHeader>
              <DialogTitle>Create New Attraction</DialogTitle>
              <DialogDescription>
                Add a new attraction to your theme park
              </DialogDescription>
            </DialogHeader>

            <div className="space-y-4 py-4">
              <div className="space-y-2">
                <Label htmlFor="name">Attraction Name</Label>
                <Input
                  id="name"
                  placeholder="e.g., Roller Coaster"
                  value={formData.name}
                  onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                />
              </div>

              <div className="space-y-2">
                <Label htmlFor="description">Description</Label>
                <Textarea
                  id="description"
                  placeholder="Describe the attraction..."
                  rows={4}
                  value={formData.description}
                  onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                />
              </div>

              <div className="space-y-2">
                <Label htmlFor="imageUrl">Image URL (optional)</Label>
                <Input
                  id="imageUrl"
                  placeholder="https://example.com/image.jpg"
                  value={formData.imageUrl}
                  onChange={(e) => setFormData({ ...formData, imageUrl: e.target.value })}
                />
              </div>

              <div className="space-y-2">
                <Label htmlFor="capacity">Capacity</Label>
                <Input
                  id="capacity"
                  type="number"
                  min="1"
                  placeholder="50"
                  value={formData.capacity || ''}
                  onChange={(e) => setFormData({ ...formData, capacity: parseInt(e.target.value) || 0 })}
                />
              </div>
            </div>

            <DialogFooter>
              <Button variant="outline" onClick={() => setIsCreateDialogOpen(false)}>
                Cancel
              </Button>
              <Button onClick={handleCreate}>Create Attraction</Button>
            </DialogFooter>
          </DialogContent>
        </Dialog>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {attractions.map((attraction) => (
          <Card key={attraction.id} className="overflow-hidden">
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
              <CardDescription className="line-clamp-2">
                {attraction.description}
              </CardDescription>
            </CardHeader>
            <CardContent>
              <div className="flex items-center justify-between text-sm">
                <span className="text-gray-600">Capacity:</span>
                <span className="font-semibold">{attraction.capacity} people</span>
              </div>
            </CardContent>
            <CardFooter className="flex gap-2">
              <Button
                variant="outline"
                size="sm"
                className="flex-1"
                onClick={() => handleEdit(attraction)}
              >
                <Pencil className="mr-2 h-4 w-4" />
                Edit
              </Button>
              <Button
                variant="destructive"
                size="sm"
                onClick={() => handleDelete(attraction.id)}
              >
                <Trash2 className="h-4 w-4" />
              </Button>
            </CardFooter>
          </Card>
        ))}
      </div>

      <Dialog open={isEditDialogOpen} onOpenChange={setIsEditDialogOpen}>
        <DialogContent className="max-w-2xl">
          <DialogHeader>
            <DialogTitle>Edit Attraction</DialogTitle>
            <DialogDescription>
              Update attraction details
            </DialogDescription>
          </DialogHeader>

          <div className="space-y-4 py-4">
            <div className="space-y-2">
              <Label htmlFor="edit-name">Attraction Name</Label>
              <Input
                id="edit-name"
                placeholder="e.g., Roller Coaster"
                value={formData.name}
                onChange={(e) => setFormData({ ...formData, name: e.target.value })}
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="edit-description">Description</Label>
              <Textarea
                id="edit-description"
                placeholder="Describe the attraction..."
                rows={4}
                value={formData.description}
                onChange={(e) => setFormData({ ...formData, description: e.target.value })}
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="edit-imageUrl">Image URL (optional)</Label>
              <Input
                id="edit-imageUrl"
                placeholder="https://example.com/image.jpg"
                value={formData.imageUrl}
                onChange={(e) => setFormData({ ...formData, imageUrl: e.target.value })}
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="edit-capacity">Capacity</Label>
              <Input
                id="edit-capacity"
                type="number"
                min="1"
                placeholder="50"
                value={formData.capacity || ''}
                onChange={(e) => setFormData({ ...formData, capacity: parseInt(e.target.value) || 0 })}
              />
            </div>
          </div>

          <DialogFooter>
            <Button variant="outline" onClick={() => setIsEditDialogOpen(false)}>
              Cancel
            </Button>
            <Button onClick={handleUpdate}>Update Attraction</Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
      </div>
    </AdminLayout>
  )
}
