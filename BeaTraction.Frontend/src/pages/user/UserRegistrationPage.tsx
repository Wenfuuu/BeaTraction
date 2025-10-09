import { useState, useEffect } from "react";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import UserLayout from "@/components/layouts/UserLayout";
import { useAuthContext } from "@/contexts/AuthContext";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import type { Attraction } from "@/types/attraction.types";
import { toast } from "@/lib/toast";
import {
  Calendar,
  Clock,
  Users,
  CheckCircle,
  Image as ImageIcon,
  AlertCircle,
} from "lucide-react";
import { userRegistrationService, type AttractionWithSchedules, type ScheduleAttractionWithStats } from "@/services/userRegistrationService";
import { registrationService } from "@/services/registrationService";

export default function UserRegistrationPage() {
  const { user } = useAuthContext();
  const currentUserId = user?.id || "";

  const [attractions, setAttractions] = useState<AttractionWithSchedules[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [selectedSchedule, setSelectedSchedule] = useState<{
    attraction: Attraction;
    scheduleAttraction: ScheduleAttractionWithStats;
  } | null>(null);
  const [isRegistering, setIsRegistering] = useState(false);

  useEffect(() => {
    if (currentUserId) {
      loadAttractions();
    }
  }, [currentUserId]);

  const loadAttractions = async () => {
    if (!currentUserId) {
      toast.error("Please log in to view attractions");
      setIsLoading(false);
      return;
    }

    try {
      setIsLoading(true);
      const data = await userRegistrationService.getAttractionsWithSchedules(currentUserId);
      setAttractions(data);
    } catch (error) {
      console.error("Error loading attractions:", error);
      toast.error("Failed to load attractions");
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

  const getAvailableSpots = (capacity: number, registered: number) => {
    return capacity - registered;
  };

  const getUtilizationPercentage = (registered: number, capacity: number) => {
    return Math.round((registered / capacity) * 100);
  };

  const handleRegisterClick = (
    attraction: Attraction,
    scheduleAttraction: ScheduleAttractionWithStats
  ) => {
    setSelectedSchedule({ attraction, scheduleAttraction });
  };

  const handleConfirmRegistration = async () => {
    if (!selectedSchedule || !currentUserId) return;

    try {
      setIsRegistering(true);
      await registrationService.create({
        userId: currentUserId,
        scheduleAttractionId: selectedSchedule.scheduleAttraction.scheduleAttractionId,
        registeredAt: new Date().toISOString(),
      });

      toast.success("Registration successful!", {
        description: `You're registered for ${selectedSchedule.attraction.name} - ${selectedSchedule.scheduleAttraction.scheduleName}`,
      });

      setSelectedSchedule(null);
      
      await loadAttractions();
    } catch (error) {
      console.error("Error registering:", error);
      toast.error("Failed to register", {
        description: error instanceof Error ? error.message : "Please try again",
      });
    } finally {
      setIsRegistering(false);
    }
  };

  const handleCancelRegistration = async (
    scheduleAttractionId: string,
    attractionName: string
  ) => {
    try {
      const userRegistrations = await registrationService.getByUserId(currentUserId);
      const registration = userRegistrations.find(
        r => r.scheduleAttractionId === scheduleAttractionId
      );

      if (!registration) {
        toast.error("Registration not found");
        return;
      }

      await registrationService.delete(registration.id);

      toast.success("Registration cancelled", {
        description: `Your registration for ${attractionName} has been cancelled.`,
      });

      await loadAttractions();
    } catch (error) {
      console.error("Error cancelling registration:", error);
      toast.error("Failed to cancel registration", {
        description: error instanceof Error ? error.message : "Please try again",
      });
    }
  };

  return (
    <UserLayout>
      <div className="container mx-auto py-8 px-4">
        <div className="mb-8">
          <h1 className="text-3xl font-bold">Available Attractions</h1>
          <p className="text-gray-600 mt-2">
            Browse and register for your favorite attractions
          </p>
        </div>

        {isLoading ? (
          <div className="flex items-center justify-center py-12">
            <div className="text-center">
              <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto mb-4"></div>
              <p className="text-gray-600">Loading attractions...</p>
            </div>
          </div>
        ) : attractions.length === 0 ? (
          <Card>
            <CardContent className="py-12 text-center">
              <AlertCircle className="h-12 w-12 mx-auto text-gray-400 mb-4" />
              <h3 className="text-lg font-semibold mb-2">
                No attractions available
              </h3>
              <p className="text-gray-600">
                Check back later for new attractions!
              </p>
            </CardContent>
          </Card>
        ) : (
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
                  <h4 className="font-semibold text-sm">
                    Available Schedules:
                  </h4>
                  {attraction.scheduleAttractions.map((scheduleAttraction) => {
                    const availableSpots = getAvailableSpots(
                      attraction.capacity,
                      scheduleAttraction.registrationCount
                    );
                    const utilization = getUtilizationPercentage(
                      scheduleAttraction.registrationCount,
                      attraction.capacity
                    );
                    const isFull = availableSpots <= 0;

                    return (
                      <div
                        key={scheduleAttraction.scheduleAttractionId}
                        className="border rounded-lg p-3 space-y-2"
                      >
                        <div className="flex items-start justify-between">
                          <div className="flex-1">
                            <h5 className="font-semibold text-sm">
                              {scheduleAttraction.scheduleName}
                            </h5>
                            <div className="flex items-center gap-1 text-xs text-gray-600 mt-1">
                              <Calendar className="h-3 w-3" />
                              <span>{formatDateTime(scheduleAttraction.startTime)}</span>
                            </div>
                            <div className="flex items-center gap-1 text-xs text-gray-600">
                              <Clock className="h-3 w-3" />
                              <span>{formatDateTime(scheduleAttraction.endTime)}</span>
                            </div>
                          </div>
                          {scheduleAttraction.isRegistered && (
                            <Badge variant="default" className="bg-green-500">
                              <CheckCircle className="h-3 w-3 mr-1" />
                              Registered
                            </Badge>
                          )}
                        </div>

                        <div className="flex items-center justify-between text-xs">
                          <span
                            className={
                              isFull
                                ? "text-red-600 font-semibold"
                                : "text-gray-600"
                            }
                          >
                            {isFull
                              ? "Fully Booked"
                              : `${availableSpots} spots left`}
                          </span>
                          <span
                            className={
                              utilization >= 90
                                ? "text-red-600"
                                : "text-gray-600"
                            }
                          >
                            {utilization}% full
                          </span>
                        </div>

                        {scheduleAttraction.isRegistered ? (
                          <Button
                            variant="outline"
                            size="sm"
                            className="w-full"
                            onClick={() =>
                              handleCancelRegistration(
                                scheduleAttraction.scheduleAttractionId,
                                attraction.name
                              )
                            }
                          >
                            Cancel Registration
                          </Button>
                        ) : (
                          <Button
                            size="sm"
                            className="w-full"
                            disabled={isFull}
                            onClick={() =>
                              handleRegisterClick(attraction, scheduleAttraction)
                            }
                          >
                            {isFull ? "Fully Booked" : "Register Now"}
                          </Button>
                        )}
                      </div>
                    );
                  })}
                </div>
              </CardContent>
            </Card>
          ))}
          </div>
        )}

        <Dialog
          open={!!selectedSchedule}
          onOpenChange={() => setSelectedSchedule(null)}
        >
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
                  <h4 className="font-semibold text-lg">
                    {selectedSchedule.attraction.name}
                  </h4>
                  <p className="text-sm text-gray-600 mt-1">
                    {selectedSchedule.attraction.description}
                  </p>
                </div>

                <div className="space-y-2">
                  <div className="flex items-center justify-between">
                    <span className="text-sm text-gray-600">Schedule:</span>
                    <span className="font-semibold">
                      {selectedSchedule.scheduleAttraction.scheduleName}
                    </span>
                  </div>
                  <div className="flex items-center justify-between">
                    <span className="text-sm text-gray-600">Start Time:</span>
                    <span className="font-semibold">
                      {formatDateTime(selectedSchedule.scheduleAttraction.startTime)}
                    </span>
                  </div>
                  <div className="flex items-center justify-between">
                    <span className="text-sm text-gray-600">End Time:</span>
                    <span className="font-semibold">
                      {formatDateTime(selectedSchedule.scheduleAttraction.endTime)}
                    </span>
                  </div>
                  <div className="flex items-center justify-between">
                    <span className="text-sm text-gray-600">
                      Available Spots:
                    </span>
                    <span className="font-semibold">
                      {getAvailableSpots(
                        selectedSchedule.attraction.capacity,
                        selectedSchedule.scheduleAttraction.registrationCount
                      )}
                    </span>
                  </div>
                </div>
              </div>
            )}

            <DialogFooter>
              <Button
                variant="outline"
                onClick={() => setSelectedSchedule(null)}
                disabled={isRegistering}
              >
                Cancel
              </Button>
              <Button 
                onClick={handleConfirmRegistration}
                disabled={isRegistering}
              >
                {isRegistering ? "Registering..." : "Confirm Registration"}
              </Button>
            </DialogFooter>
          </DialogContent>
        </Dialog>
      </div>
    </UserLayout>
  );
}
