import { useState, useEffect } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Textarea } from "@/components/ui/textarea";
import AdminLayout from "@/components/layouts/AdminLayout";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog";
import type {
  Attraction,
  CreateAttractionRequest,
} from "@/types/attraction.types";
import { Plus, Pencil, Trash2, Image as ImageIcon, Upload, X } from "lucide-react";
import { attractionService } from "@/services/attractionService";
import { toast } from "@/lib/toast";

export default function AdminAttractionsPage() {
  const [attractions, setAttractions] = useState<Attraction[]>([]);
  const [isLoading, setIsLoading] = useState(false);

  const [isCreateDialogOpen, setIsCreateDialogOpen] = useState(false);
  const [isEditDialogOpen, setIsEditDialogOpen] = useState(false);
  const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
  const [selectedAttraction, setSelectedAttraction] =
    useState<Attraction | null>(null);
  const [attractionToDelete, setAttractionToDelete] = useState<string | null>(null);

  const [formData, setFormData] = useState<CreateAttractionRequest>({
    name: "",
    description: "",
    image: null,
    capacity: 0,
  });

  const [imagePreview, setImagePreview] = useState<string | null>(null);

  useEffect(() => {
    const loadAttractions = async () => {
      try {
        setIsLoading(true);
        const data = await attractionService.getAll();
        setAttractions(data);
      } catch (error) {
        toast.error("Failed to load attractions", {
          description: error instanceof Error ? error.message : "An error occurred",
        });
      } finally {
        setIsLoading(false);
      }
    };
    loadAttractions();
  }, []);

  const fetchAttractions = async () => {
    try {
      setIsLoading(true);
      const data = await attractionService.getAll();
      setAttractions(data);
    } catch (error) {
      toast.error("Failed to load attractions", {
        description: error instanceof Error ? error.message : "An error occurred",
      });
    } finally {
      setIsLoading(false);
    }
  };

  const handleImageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      const validTypes = ["image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp"];
      if (!validTypes.includes(file.type)) {
        toast.error("Invalid file type", {
          description: "Please select a valid image file (JPEG, PNG, GIF, or WebP)",
        });
        return;
      }

      const maxSize = 5 * 1024 * 1024;
      if (file.size > maxSize) {
        toast.error("File too large", {
          description: "Image size must not exceed 5MB",
        });
        return;
      }

      setFormData({ ...formData, image: file });
      
      const reader = new FileReader();
      reader.onloadend = () => {
        setImagePreview(reader.result as string);
      };
      reader.readAsDataURL(file);
    }
  };

  const handleRemoveImage = () => {
    setFormData({ ...formData, image: null });
    setImagePreview(null);
  };

  const handleCreate = async () => {
    try {
      setIsLoading(true);
      await attractionService.create(formData);
      toast.success("Success!", {
        description: "Attraction created successfully",
      });
      setIsCreateDialogOpen(false);
      resetForm();
      await fetchAttractions();
    } catch (error) {
      toast.error("Failed to create attraction", {
        description: error instanceof Error ? error.message : "An error occurred",
      });
    } finally {
      setIsLoading(false);
    }
  };

  const handleEdit = (attraction: Attraction) => {
    setSelectedAttraction(attraction);
    setFormData({
      name: attraction.name,
      description: attraction.description,
      image: null,
      capacity: attraction.capacity,
    });
    setImagePreview(attraction.imageUrl);
    setIsEditDialogOpen(true);
  };

  const handleUpdate = async () => {
    if (!selectedAttraction) return;
    
    try {
      setIsLoading(true);
      await attractionService.update(selectedAttraction.id, {
        ...formData,
        id: selectedAttraction.id,
        rowVersion: selectedAttraction.rowVersion,
      });
      toast.success("Success!", {
        description: "Attraction updated successfully",
      });
      setIsEditDialogOpen(false);
      resetForm();
      await fetchAttractions();
    } catch (error) {
      toast.error("Failed to update attraction", {
        description: error instanceof Error ? error.message : "An error occurred",
      });
    } finally {
      setIsLoading(false);
    }
  };

  const handleDeleteClick = (id: string) => {
    setAttractionToDelete(id);
    setIsDeleteDialogOpen(true);
  };

  const handleDeleteConfirm = async () => {
    if (!attractionToDelete) return;

    try {
      setIsLoading(true);
      await attractionService.delete(attractionToDelete);
      toast.success("Success!", {
        description: "Attraction deleted successfully",
      });
      await fetchAttractions();
    } catch (error) {
      toast.error("Failed to delete attraction", {
        description: error instanceof Error ? error.message : "An error occurred",
      });
    } finally {
      setIsLoading(false);
      setIsDeleteDialogOpen(false);
      setAttractionToDelete(null);
    }
  };

  const resetForm = () => {
    setFormData({
      name: "",
      description: "",
      image: null,
      capacity: 0,
    });
    setImagePreview(null);
    setSelectedAttraction(null);
  };

  return (
    <AdminLayout>
      <div className="container mx-auto py-8 px-4">
        <div className="flex justify-between items-center mb-8">
          <div>
            <h1 className="text-3xl font-bold">Manage Attractions</h1>
            <p className="text-gray-600 mt-2">
              Add, edit, or remove attractions
            </p>
          </div>

          <Dialog
            open={isCreateDialogOpen}
            onOpenChange={(open) => {
              setIsCreateDialogOpen(open);
              if (!open) resetForm();
            }}
          >
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
                    onChange={(e) =>
                      setFormData({ ...formData, name: e.target.value })
                    }
                  />
                </div>

                <div className="space-y-2">
                  <Label htmlFor="description">Description</Label>
                  <Textarea
                    id="description"
                    placeholder="Describe the attraction..."
                    rows={4}
                    value={formData.description}
                    onChange={(e) =>
                      setFormData({ ...formData, description: e.target.value })
                    }
                  />
                </div>

                <div className="space-y-2">
                  <Label htmlFor="image">Image (optional)</Label>
                  <div className="space-y-4">
                    {imagePreview ? (
                      <div className="relative">
                        <img
                          src={imagePreview}
                          alt="Preview"
                          className="w-full h-48 object-cover rounded-lg border"
                        />
                        <Button
                          type="button"
                          variant="destructive"
                          size="icon"
                          className="absolute top-2 right-2"
                          onClick={handleRemoveImage}
                        >
                          <X className="h-4 w-4" />
                        </Button>
                      </div>
                    ) : (
                      <div className="border-2 border-dashed border-gray-300 rounded-lg p-6 text-center hover:border-gray-400 transition-colors">
                        <Upload className="mx-auto h-12 w-12 text-gray-400" />
                        <div className="mt-2">
                          <Label
                            htmlFor="image"
                            className="cursor-pointer text-sm text-blue-600 hover:text-blue-500"
                          >
                            Click to upload
                          </Label>
                          <Input
                            id="image"
                            type="file"
                            accept="image/jpeg,image/jpg,image/png,image/gif,image/webp"
                            className="hidden"
                            onChange={handleImageChange}
                          />
                        </div>
                        <p className="text-xs text-gray-500 mt-1">
                          PNG, JPG, GIF, WebP up to 5MB
                        </p>
                      </div>
                    )}
                  </div>
                </div>

                <div className="space-y-2">
                  <Label htmlFor="capacity">Capacity</Label>
                  <Input
                    id="capacity"
                    type="number"
                    min="1"
                    placeholder="50"
                    value={formData.capacity || ""}
                    onChange={(e) =>
                      setFormData({
                        ...formData,
                        capacity: parseInt(e.target.value) || 0,
                      })
                    }
                  />
                </div>
              </div>

              <DialogFooter>
                <Button
                  variant="outline"
                  onClick={() => setIsCreateDialogOpen(false)}
                  disabled={isLoading}
                >
                  Cancel
                </Button>
                <Button onClick={handleCreate} disabled={isLoading}>
                  {isLoading ? "Creating..." : "Create Attraction"}
                </Button>
              </DialogFooter>
            </DialogContent>
          </Dialog>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {isLoading ? (
            <div className="col-span-full text-center py-8">
              <p className="text-gray-500">Loading attractions...</p>
            </div>
          ) : attractions.length === 0 ? (
            <div className="col-span-full text-center py-8">
              <p className="text-gray-500">
                No attractions found. Create one to get started!
              </p>
            </div>
          ) : (
            attractions.map((attraction) => (
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
                    <span className="font-semibold">
                      {attraction.capacity} people
                    </span>
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
                    onClick={() => handleDeleteClick(attraction.id)}
                  >
                    <Trash2 className="h-4 w-4" />
                  </Button>
                </CardFooter>
              </Card>
            ))
          )}
        </div>

        <Dialog
          open={isEditDialogOpen}
          onOpenChange={(open) => {
            setIsEditDialogOpen(open);
            if (!open) resetForm();
          }}
        >
          <DialogContent className="max-w-2xl">
            <DialogHeader>
              <DialogTitle>Edit Attraction</DialogTitle>
              <DialogDescription>Update attraction details</DialogDescription>
            </DialogHeader>

            <div className="space-y-4 py-4">
              <div className="space-y-2">
                <Label htmlFor="edit-name">Attraction Name</Label>
                <Input
                  id="edit-name"
                  placeholder="e.g., Roller Coaster"
                  value={formData.name}
                  onChange={(e) =>
                    setFormData({ ...formData, name: e.target.value })
                  }
                />
              </div>

              <div className="space-y-2">
                <Label htmlFor="edit-description">Description</Label>
                <Textarea
                  id="edit-description"
                  placeholder="Describe the attraction..."
                  rows={4}
                  value={formData.description}
                  onChange={(e) =>
                    setFormData({ ...formData, description: e.target.value })
                  }
                />
              </div>

              <div className="space-y-2">
                <Label htmlFor="edit-image">Image (optional)</Label>
                <div className="space-y-4">
                  {imagePreview ? (
                    <div className="relative">
                      <img
                        src={imagePreview}
                        alt="Preview"
                        className="w-full h-48 object-cover rounded-lg border"
                      />
                      <Button
                        type="button"
                        variant="destructive"
                        size="icon"
                        className="absolute top-2 right-2"
                        onClick={handleRemoveImage}
                      >
                        <X className="h-4 w-4" />
                      </Button>
                    </div>
                  ) : (
                    <div className="border-2 border-dashed border-gray-300 rounded-lg p-6 text-center hover:border-gray-400 transition-colors">
                      <Upload className="mx-auto h-12 w-12 text-gray-400" />
                      <div className="mt-2">
                        <Label
                          htmlFor="edit-image"
                          className="cursor-pointer text-sm text-blue-600 hover:text-blue-500"
                        >
                          Click to upload new image
                        </Label>
                        <Input
                          id="edit-image"
                          type="file"
                          accept="image/jpeg,image/jpg,image/png,image/gif,image/webp"
                          className="hidden"
                          onChange={handleImageChange}
                        />
                      </div>
                      <p className="text-xs text-gray-500 mt-1">
                        PNG, JPG, GIF, WebP up to 5MB
                      </p>
                    </div>
                  )}
                </div>
              </div>

              <div className="space-y-2">
                <Label htmlFor="edit-capacity">Capacity</Label>
                <Input
                  id="edit-capacity"
                  type="number"
                  min="1"
                  placeholder="50"
                  value={formData.capacity || ""}
                  onChange={(e) =>
                    setFormData({
                      ...formData,
                      capacity: parseInt(e.target.value) || 0,
                    })
                  }
                />
              </div>
            </div>

            <DialogFooter>
              <Button
                variant="outline"
                onClick={() => setIsEditDialogOpen(false)}
                disabled={isLoading}
              >
                Cancel
              </Button>
              <Button onClick={handleUpdate} disabled={isLoading}>
                {isLoading ? "Updating..." : "Update Attraction"}
              </Button>
            </DialogFooter>
          </DialogContent>
        </Dialog>

        <AlertDialog
          open={isDeleteDialogOpen}
          onOpenChange={setIsDeleteDialogOpen}
        >
          <AlertDialogContent>
            <AlertDialogHeader>
              <AlertDialogTitle>Are you sure?</AlertDialogTitle>
              <AlertDialogDescription>
                This action cannot be undone. This will permanently delete the
                attraction and remove all associated data.
              </AlertDialogDescription>
            </AlertDialogHeader>
            <AlertDialogFooter>
              <AlertDialogCancel disabled={isLoading}>Cancel</AlertDialogCancel>
              <AlertDialogAction
                onClick={handleDeleteConfirm}
                disabled={isLoading}
                className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
              >
                {isLoading ? "Deleting..." : "Delete"}
              </AlertDialogAction>
            </AlertDialogFooter>
          </AlertDialogContent>
        </AlertDialog>
      </div>
    </AdminLayout>
  );
}
