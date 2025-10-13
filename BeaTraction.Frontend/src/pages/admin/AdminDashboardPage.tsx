import { useState, useEffect, useCallback, useRef } from "react";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Progress } from "@/components/ui/progress";
import AdminLayout from "@/components/layouts/AdminLayout";
import type { AttractionRegistrationStats } from "@/types/registration.types";
import { Users, TrendingUp, Calendar, AlertCircle, Wifi, WifiOff } from "lucide-react";
import { dashboardService } from "@/services/dashboardService";
import { toast } from "@/lib/toast";
import { useSignalR } from "@/hooks/useSignalR";
import type { RegistrationCreatedEvent, RegistrationDeletedEvent } from "@/services/signalRService";

export default function AdminDashboardPage() {
  const [stats, setStats] = useState<AttractionRegistrationStats[]>([]);
  const [isLoading, setIsLoading] = useState(false);

  const reloadTimerRef = useRef<number | null>(null);
  const RELOAD_DEBOUNCE_MS = 2000;

  const loadStats = useCallback(async () => {
    try {
      setIsLoading(true);
      const data = await dashboardService.getAttractionStats();
      setStats(data);
    } catch (error) {
      toast.error("Failed to load dashboard data", {
        description: error instanceof Error ? error.message : "An error occurred",
      });
    } finally {
      setIsLoading(false);
    }
  }, []);

  const updateStatsOptimistically = useCallback(
    (scheduleAttractionId: string, isRegistered: boolean) => {
      setStats((prev) =>
        prev.map((attraction) => {
          const hasSchedule = attraction.scheduleAttractions.some(
            (sa) => sa.scheduleAttractionId === scheduleAttractionId
          );

          if (!hasSchedule) {
            return attraction;
          }

          return {
            ...attraction,
            totalRegistrations: isRegistered
              ? attraction.totalRegistrations + 1
              : attraction.totalRegistrations - 1,
            scheduleAttractions: attraction.scheduleAttractions.map((sa) => {
              if (sa.scheduleAttractionId === scheduleAttractionId) {
                return {
                  ...sa,
                  registrationCount: isRegistered
                    ? sa.registrationCount + 1
                    : sa.registrationCount - 1,
                };
              }
              return sa;
            }),
          };
        })
      );
    },
    []
  );

  const scheduleReload = useCallback(() => {
    if (reloadTimerRef.current !== null) {
      clearTimeout(reloadTimerRef.current);
    }

    reloadTimerRef.current = window.setTimeout(() => {
      loadStats();
    }, RELOAD_DEBOUNCE_MS);
  }, [loadStats]);

  // SignalR real-time updates
  const handleRegistrationCreated = useCallback((event: RegistrationCreatedEvent) => {
    console.log("Admin: Registration created event:", event);
    updateStatsOptimistically(event.scheduleAttractionId, true);
    
    scheduleReload();
  }, [updateStatsOptimistically, scheduleReload]);

  const handleRegistrationDeleted = useCallback((event: RegistrationDeletedEvent) => {
    console.log("Admin: Registration deleted event:", event);
    updateStatsOptimistically(event.scheduleAttractionId, false);
    
    scheduleReload();
  }, [updateStatsOptimistically, scheduleReload]);

  useEffect(() => {
    return () => {
      if (reloadTimerRef.current !== null) {
        clearTimeout(reloadTimerRef.current);
      }
    };
  }, []);

  // Connect to SignalR and subscribe to events
  const { isConnected } = useSignalR({
    autoConnect: true,
    onRegistrationCreated: handleRegistrationCreated,
    onRegistrationDeleted: handleRegistrationDeleted,
  });

  useEffect(() => {
    loadStats();
  }, [loadStats]);

  const formatDateTime = (dateString: string) => {
    return new Date(dateString).toLocaleString("en-US", {
      dateStyle: "medium",
      timeStyle: "short",
    });
  };

  const getUtilizationPercentage = (registered: number, capacity: number) => {
    if (capacity === 0) return 0;
    return Math.round((registered / capacity) * 100);
  };

  const getUtilizationColor = (percentage: number) => {
    if (percentage >= 90) return "text-red-600";
    if (percentage >= 70) return "text-yellow-600";
    return "text-green-600";
  };

  const totalCapacity = stats.reduce(
    (sum, stat) => sum + (stat.capacity * stat.scheduleAttractions.length), 
    0
  );
  const totalRegistrations = stats.reduce(
    (sum, stat) => sum + stat.totalRegistrations,
    0
  );
  const overallUtilization = getUtilizationPercentage(
    totalRegistrations,
    totalCapacity
  );

  return (
    <AdminLayout>
      <div className="container mx-auto py-8 px-4">
        <div className="mb-8">
          <div className="flex items-center justify-between">
            <div>
              <h1 className="text-3xl font-bold">Dashboard</h1>
              <p className="text-gray-600 mt-2">
                Real-time registration monitoring
              </p>
            </div>
            
            {/* SignalR Connection Status */}
            <div className="flex items-center gap-2 text-sm">
              {isConnected ? (
                <>
                  <Wifi className="h-4 w-4 text-green-500" />
                  <span className="text-green-600 font-medium">Live Updates Active</span>
                </>
              ) : (
                <>
                  <WifiOff className="h-4 w-4 text-gray-400" />
                  <span className="text-gray-500">Offline Mode</span>
                </>
              )}
            </div>
          </div>
        </div>

        {isLoading ? (
          <div className="flex items-center justify-center py-12">
            <p className="text-gray-500">Loading dashboard data...</p>
          </div>
        ) : stats.length === 0 ? (
          <Card>
            <CardContent className="py-12 text-center">
              <AlertCircle className="h-12 w-12 mx-auto text-gray-400 mb-4" />
              <h3 className="text-lg font-semibold mb-2">No data available</h3>
              <p className="text-gray-600 mb-4">
                Add some attractions and schedules to see statistics
              </p>
            </CardContent>
          </Card>
        ) : (
          <>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">
                Total Attractions
              </CardTitle>
              <TrendingUp className="h-4 w-4 text-muted-foreground" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold">{stats.length}</div>
              <p className="text-xs text-muted-foreground mt-1">
                Active attractions
              </p>
            </CardContent>
          </Card>

          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">
                Total Capacity
              </CardTitle>
              <Users className="h-4 w-4 text-muted-foreground" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold">{totalCapacity}</div>
              <p className="text-xs text-muted-foreground mt-1">
                Maximum capacity
              </p>
            </CardContent>
          </Card>

          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">
                Total Registrations
              </CardTitle>
              <Calendar className="h-4 w-4 text-muted-foreground" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold">{totalRegistrations}</div>
              <p className="text-xs text-muted-foreground mt-1">
                Registered users
              </p>
            </CardContent>
          </Card>

          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">
                Overall Utilization
              </CardTitle>
              <AlertCircle className="h-4 w-4 text-muted-foreground" />
            </CardHeader>
            <CardContent>
              <div
                className={`text-2xl font-bold ${getUtilizationColor(
                  overallUtilization
                )}`}
              >
                {overallUtilization}%
              </div>
              <Progress value={overallUtilization} className="mt-2" />
            </CardContent>
          </Card>
        </div>

        <div className="space-y-6">
          <h2 className="text-2xl font-bold">Attraction Statistics</h2>

          {stats.map((attraction) => {
            const attractionTotalCapacity = attraction.capacity * attraction.scheduleAttractions.length;
            const utilization = getUtilizationPercentage(
              attraction.totalRegistrations,
              attractionTotalCapacity
            );

            return (
              <Card key={attraction.attractionId}>
                <CardHeader>
                  <div className="flex items-start justify-between">
                    <div>
                      <CardTitle>{attraction.attractionName}</CardTitle>
                      <CardDescription className="mt-2">
                        {attraction.scheduleAttractions.length} schedule(s) available
                      </CardDescription>
                    </div>
                    <div className="text-right">
                      <div className="text-2xl font-bold">
                        {attraction.totalRegistrations} / {attractionTotalCapacity}
                      </div>
                      <div
                        className={`text-sm font-semibold ${getUtilizationColor(
                          utilization
                        )}`}
                      >
                        {utilization}% Utilization
                      </div>
                    </div>
                  </div>
                  <Progress value={utilization} className="mt-4" />
                </CardHeader>
                <CardContent>
                  <div className="space-y-4">
                    <h4 className="font-semibold text-sm text-gray-700">
                      Schedule Breakdown:
                    </h4>
                    {attraction.scheduleAttractions.map((scheduleAttraction) => {
                      const scheduleUtilization = getUtilizationPercentage(
                        scheduleAttraction.registrationCount,
                        attraction.capacity
                      );

                      return (
                        <div
                          key={scheduleAttraction.scheduleAttractionId}
                          className="border rounded-lg p-4 space-y-3"
                        >
                          <div className="flex items-start justify-between">
                            <div>
                              <h5 className="font-semibold">
                                {scheduleAttraction.scheduleName}
                              </h5>
                              <div className="text-sm text-gray-600 mt-1">
                                {formatDateTime(scheduleAttraction.startTime)} -{" "}
                                {formatDateTime(scheduleAttraction.endTime)}
                              </div>
                            </div>
                            <div className="text-right">
                              <div className="font-semibold">
                                {scheduleAttraction.registrationCount} /{" "}
                                {attraction.capacity}
                              </div>
                              <div
                                className={`text-sm ${getUtilizationColor(
                                  scheduleUtilization
                                )}`}
                              >
                                {scheduleUtilization}%
                              </div>
                            </div>
                          </div>
                          <Progress
                            value={scheduleUtilization}
                            className="h-2"
                          />
                          {scheduleUtilization === 100 ? (
                            <div className="flex items-center gap-2 text-sm text-red-600 bg-red-50 p-2 rounded">
                              <AlertCircle className="h-4 w-4" />
                              <span>Fully booked! No spots left.</span>
                            </div>
                          ) : scheduleUtilization >= 90 ? (
                            <div className="flex items-center gap-2 text-sm text-red-600 bg-red-50 p-2 rounded">
                              <AlertCircle className="h-4 w-4" />
                              <span>
                                Nearly full! Only{" "}
                                {attraction.capacity -
                                  scheduleAttraction.registrationCount}{" "}
                                spots left
                              </span>
                            </div>
                          ) : null}
                        </div>
                      );
                    })}
                  </div>
                </CardContent>
              </Card>
            );
          })}
        </div>
          </>
        )}
      </div>
    </AdminLayout>
  );
}
