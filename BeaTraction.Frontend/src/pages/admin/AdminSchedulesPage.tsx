import { useState, useEffect } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
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
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import type { Schedule, CreateScheduleRequest } from "@/types/schedule.types";
import type { Attraction } from "@/types/attraction.types";
import type { ScheduleAttractionWithDetails } from "@/types/schedule-attraction.types";
import { Plus, Pencil, Trash2, Calendar, Clock, MapPin, X } from "lucide-react";
import { toast } from "@/lib/toast";
import { scheduleService } from "@/services/scheduleService";
import { attractionService } from "@/services/attractionService";
import { scheduleAttractionService } from "@/services/scheduleAttractionService";

export default function AdminSchedulesPage() {
  const [schedules, setSchedules] = useState<Schedule[]>([]);
  const [attractions, setAttractions] = useState<Attraction[]>([]);
  const [scheduleAttractions, setScheduleAttractions] = useState<
    ScheduleAttractionWithDetails[]
  >([]);
  const [isLoading, setIsLoading] = useState(false);

  const [isCreateDialogOpen, setIsCreateDialogOpen] = useState(false);
  const [isEditDialogOpen, setIsEditDialogOpen] = useState(false);
  const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
  const [isManageAttractionsDialogOpen, setIsManageAttractionsDialogOpen] =
    useState(false);

  const [selectedSchedule, setSelectedSchedule] = useState<Schedule | null>(
    null
  );
  const [scheduleToDelete, setScheduleToDelete] = useState<string | null>(null);
  const [selectedAttractionId, setSelectedAttractionId] = useState<string>("");

  const [formData, setFormData] = useState<CreateScheduleRequest>({
    name: "",
    startTime: "",
    endTime: "",
  });

  useEffect(() => {
    const loadData = async () => {
      try {
        setIsLoading(true);
        const [schedulesData, attractionsData, scheduleAttractionsData] =
          await Promise.all([
            scheduleService.getAll(),
            attractionService.getAll(),
            scheduleAttractionService.getAll(),
          ]);
        setSchedules(schedulesData);
        setAttractions(attractionsData);
        setScheduleAttractions(scheduleAttractionsData);
      } catch (error) {
        toast.error("Failed to load data", {
          description:
            error instanceof Error ? error.message : "An error occurred",
        });
      } finally {
        setIsLoading(false);
      }
    };
    loadData();
  }, []);

  const fetchSchedules = async () => {
    try {
      setIsLoading(true);
      const [schedulesData, scheduleAttractionsData] = await Promise.all([
        scheduleService.getAll(),
        scheduleAttractionService.getAll(),
      ]);
      setSchedules(schedulesData);
      setScheduleAttractions(scheduleAttractionsData);
    } catch (error) {
      toast.error("Failed to load schedules", {
        description:
          error instanceof Error ? error.message : "An error occurred",
      });
    } finally {
      setIsLoading(false);
    }
  };

  const getScheduleAttractions = (scheduleId: string) => {
    return scheduleAttractions.filter((sa) => sa.scheduleId === scheduleId);
  };

  const handleManageAttractions = (schedule: Schedule) => {
    setSelectedSchedule(schedule);
    setIsManageAttractionsDialogOpen(true);
  };

  const handleAddAttraction = async () => {
    if (!selectedSchedule || !selectedAttractionId) {
      toast.error("Please select an attraction");
      return;
    }

    const alreadyExists = scheduleAttractions.some(
      (sa) =>
        sa.scheduleId === selectedSchedule.id &&
        sa.attractionId === selectedAttractionId
    );

    if (alreadyExists) {
      toast.error("Attraction already added", {
        description: "This attraction is already in this schedule",
      });
      return;
    }

    try {
      setIsLoading(true);
      await scheduleAttractionService.create({
        scheduleId: selectedSchedule.id,
        attractionId: selectedAttractionId,
      });

      const updatedScheduleAttractions =
        await scheduleAttractionService.getAll();
      setScheduleAttractions(updatedScheduleAttractions);

      setSelectedAttractionId("");
      toast.success("Success!", {
        description: "Attraction added to schedule",
      });
    } catch (error) {
      toast.error("Failed to add attraction", {
        description:
          error instanceof Error ? error.message : "An error occurred",
      });
    } finally {
      setIsLoading(false);
    }
  };

  const handleRemoveAttraction = async (scheduleAttractionId: string) => {
    try {
      setIsLoading(true);
      await scheduleAttractionService.delete(scheduleAttractionId);

      const updatedScheduleAttractions =
        await scheduleAttractionService.getAll();
      setScheduleAttractions(updatedScheduleAttractions);

      toast.success("Success!", {
        description: "Attraction removed from schedule",
      });
    } catch (error) {
      toast.error("Failed to remove attraction", {
        description:
          error instanceof Error ? error.message : "An error occurred",
      });
    } finally {
      setIsLoading(false);
    }
  };

  const formatDateTime = (dateString: string) => {
    return new Date(dateString).toLocaleString("en-US", {
      dateStyle: "medium",
      timeStyle: "short",
    });
  };

  const handleCreate = async () => {
    try {
      setIsLoading(true);
      await scheduleService.create(formData);
      toast.success("Success!", {
        description: "Schedule created successfully",
      });
      setIsCreateDialogOpen(false);
      resetForm();
      await fetchSchedules();
    } catch (error) {
      toast.error("Failed to create schedule", {
        description:
          error instanceof Error ? error.message : "An error occurred",
      });
    } finally {
      setIsLoading(false);
    }
  };

  const handleEdit = (schedule: Schedule) => {
    setSelectedSchedule(schedule);
    setFormData({
      name: schedule.name,
      startTime: schedule.startTime,
      endTime: schedule.endTime,
    });
    setIsEditDialogOpen(true);
  };

  const handleUpdate = async () => {
    if (!selectedSchedule) return;

    try {
      setIsLoading(true);
      await scheduleService.update(selectedSchedule.id, {
        ...formData,
        id: selectedSchedule.id,
        rowVersion: selectedSchedule.rowVersion,
      });
      toast.success("Success!", {
        description: "Schedule updated successfully",
      });
      setIsEditDialogOpen(false);
      resetForm();
      await fetchSchedules();
    } catch (error) {
      toast.error("Failed to update schedule", {
        description:
          error instanceof Error ? error.message : "An error occurred",
      });
    } finally {
      setIsLoading(false);
    }
  };

  const handleDeleteClick = (id: string) => {
    setScheduleToDelete(id);
    setIsDeleteDialogOpen(true);
  };

  const handleDeleteConfirm = async () => {
    if (!scheduleToDelete) return;

    try {
      setIsLoading(true);
      await scheduleService.delete(scheduleToDelete);
      toast.success("Success!", {
        description: "Schedule deleted successfully",
      });
      await fetchSchedules();
    } catch (error) {
      toast.error("Failed to delete schedule", {
        description:
          error instanceof Error ? error.message : "An error occurred",
      });
    } finally {
      setIsLoading(false);
      setIsDeleteDialogOpen(false);
      setScheduleToDelete(null);
    }
  };

  const resetForm = () => {
    setFormData({
      name: "",
      startTime: "",
      endTime: "",
    });
    setSelectedSchedule(null);
  };

  return (
    <AdminLayout>
      <div className="container mx-auto py-8 px-4">
        <div className="flex justify-between items-center mb-8">
          <div>
            <h1 className="text-3xl font-bold">Manage Schedules</h1>
            <p className="text-gray-600 mt-2">
              Create and manage time slots for attractions
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
                  <Label htmlFor="scheduleName">Schedule Name</Label>
                  <Input
                    id="scheduleName"
                    placeholder="e.g., Morning Session"
                    value={formData.name}
                    onChange={(e) =>
                      setFormData({ ...formData, name: e.target.value })
                    }
                  />
                </div>

                <div className="grid grid-cols-2 gap-4">
                  <div className="space-y-2">
                    <Label htmlFor="startTime">Start Time</Label>
                    <Input
                      id="startTime"
                      type="datetime-local"
                      value={formData.startTime}
                      onChange={(e) =>
                        setFormData({ ...formData, startTime: e.target.value })
                      }
                    />
                  </div>

                  <div className="space-y-2">
                    <Label htmlFor="endTime">End Time</Label>
                    <Input
                      id="endTime"
                      type="datetime-local"
                      value={formData.endTime}
                      onChange={(e) =>
                        setFormData({ ...formData, endTime: e.target.value })
                      }
                    />
                  </div>
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
                  {isLoading ? "Creating..." : "Create Schedule"}
                </Button>
              </DialogFooter>
            </DialogContent>
          </Dialog>
        </div>

        <div className="space-y-6">
          {isLoading ? (
            <Card>
              <CardContent className="py-12 text-center">
                <p className="text-gray-500">Loading schedules...</p>
              </CardContent>
            </Card>
          ) : schedules.length === 0 ? (
            <Card>
              <CardContent className="py-12 text-center">
                <Calendar className="h-12 w-12 mx-auto text-gray-400 mb-4" />
                <h3 className="text-lg font-semibold mb-2">No schedules yet</h3>
                <p className="text-gray-600 mb-4">
                  Create your first schedule to get started
                </p>
              </CardContent>
            </Card>
          ) : (
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  <Calendar className="h-5 w-5" />
                  All Schedules
                </CardTitle>
                <CardDescription>
                  {schedules.length} schedule(s) available
                </CardDescription>
              </CardHeader>
              <CardContent>
                <div className="space-y-3">
                  {schedules.map((schedule) => (
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
                        <div className="flex items-center gap-2 mt-2">
                          <MapPin className="h-4 w-4 text-gray-400" />
                          <span className="text-sm text-gray-600">
                            {getScheduleAttractions(schedule.id).length}{" "}
                            attraction(s)
                          </span>
                        </div>
                      </div>
                      <div className="flex gap-2">
                        <Button
                          variant="outline"
                          size="sm"
                          onClick={() => handleManageAttractions(schedule)}
                        >
                          <MapPin className="h-4 w-4 mr-1" />
                          Manage
                        </Button>
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
                          onClick={() => handleDeleteClick(schedule.id)}
                        >
                          <Trash2 className="h-4 w-4" />
                        </Button>
                      </div>
                    </div>
                  ))}
                </div>
              </CardContent>
            </Card>
          )}
        </div>

        <Dialog
          open={isEditDialogOpen}
          onOpenChange={(open) => {
            setIsEditDialogOpen(open);
            if (!open) resetForm();
          }}
        >
          <DialogContent className="max-w-xl">
            <DialogHeader>
              <DialogTitle>Edit Schedule</DialogTitle>
              <DialogDescription>Update schedule details</DialogDescription>
            </DialogHeader>

            <div className="space-y-4 py-4">
              <div className="space-y-2">
                <Label htmlFor="edit-scheduleName">Schedule Name</Label>
                <Input
                  id="edit-scheduleName"
                  placeholder="e.g., Morning Session"
                  value={formData.name}
                  onChange={(e) =>
                    setFormData({ ...formData, name: e.target.value })
                  }
                />
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div className="space-y-2">
                  <Label htmlFor="edit-startTime">Start Time</Label>
                  <Input
                    id="edit-startTime"
                    type="datetime-local"
                    value={formData.startTime}
                    onChange={(e) =>
                      setFormData({ ...formData, startTime: e.target.value })
                    }
                  />
                </div>

                <div className="space-y-2">
                  <Label htmlFor="edit-endTime">End Time</Label>
                  <Input
                    id="edit-endTime"
                    type="datetime-local"
                    value={formData.endTime}
                    onChange={(e) =>
                      setFormData({ ...formData, endTime: e.target.value })
                    }
                  />
                </div>
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
                {isLoading ? "Updating..." : "Update Schedule"}
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
                schedule and all associated data.
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

        <Dialog
          open={isManageAttractionsDialogOpen}
          onOpenChange={setIsManageAttractionsDialogOpen}
        >
          <DialogContent className="max-w-2xl">
            <DialogHeader>
              <DialogTitle>Manage Attractions</DialogTitle>
              <DialogDescription>
                Add or remove attractions for "{selectedSchedule?.name}"
              </DialogDescription>
            </DialogHeader>

            <div className="space-y-6 py-4">
              <div className="space-y-4">
                <h3 className="text-sm font-semibold">Add Attraction</h3>
                <div className="flex gap-2">
                  <Select
                    value={selectedAttractionId}
                    onValueChange={setSelectedAttractionId}
                  >
                    <SelectTrigger className="flex-1">
                      <SelectValue placeholder="Select an attraction" />
                    </SelectTrigger>
                    <SelectContent>
                      {attractions
                        .filter(
                          (attr) =>
                            !getScheduleAttractions(
                              selectedSchedule?.id || ""
                            ).some((sa) => sa.attractionId === attr.id)
                        )
                        .map((attraction) => (
                          <SelectItem key={attraction.id} value={attraction.id}>
                            {attraction.name}
                          </SelectItem>
                        ))}
                    </SelectContent>
                  </Select>
                  <Button
                    onClick={handleAddAttraction}
                    disabled={!selectedAttractionId || isLoading}
                  >
                    <Plus className="h-4 w-4 mr-1" />
                    Add
                  </Button>
                </div>
              </div>

              <div className="space-y-4">
                <h3 className="text-sm font-semibold">Current Attractions</h3>
                {getScheduleAttractions(selectedSchedule?.id || "").length ===
                0 ? (
                  <p className="text-sm text-gray-500 text-center py-8">
                    No attractions added yet
                  </p>
                ) : (
                  <div className="space-y-2">
                    {getScheduleAttractions(selectedSchedule?.id || "").map(
                      (sa) => {
                        const attraction = attractions.find(
                          (a) => a.id === sa.attractionId
                        );
                        if (!attraction) return null;

                        return (
                          <div
                            key={sa.id}
                            className="flex items-center justify-between p-3 border rounded-lg"
                          >
                            <div className="flex items-center gap-3">
                              {attraction.imageUrl ? (
                                <img
                                  src={attraction.imageUrl}
                                  alt={attraction.name}
                                  className="w-12 h-12 object-cover rounded"
                                />
                              ) : (
                                <div className="w-12 h-12 bg-gray-200 rounded flex items-center justify-center">
                                  <MapPin className="h-6 w-6 text-gray-400" />
                                </div>
                              )}
                              <div>
                                <h4 className="font-semibold">
                                  {attraction.name}
                                </h4>
                                <p className="text-sm text-gray-600">
                                  Capacity: {attraction.capacity}
                                </p>
                              </div>
                            </div>
                            <Button
                              variant="ghost"
                              size="icon"
                              onClick={() => handleRemoveAttraction(sa.id)}
                              disabled={isLoading}
                            >
                              <X className="h-4 w-4" />
                            </Button>
                          </div>
                        );
                      }
                    )}
                  </div>
                )}
              </div>
            </div>

            <DialogFooter>
              <Button
                variant="outline"
                onClick={() => {
                  setIsManageAttractionsDialogOpen(false);
                  setSelectedAttractionId("");
                }}
              >
                Close
              </Button>
            </DialogFooter>
          </DialogContent>
        </Dialog>
      </div>
    </AdminLayout>
  );
}
