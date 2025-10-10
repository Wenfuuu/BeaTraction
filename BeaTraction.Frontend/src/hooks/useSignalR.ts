import { useEffect, useState, useCallback } from "react";
import {
  signalRService,
  type RegistrationCreatedEvent,
  type RegistrationDeletedEvent,
} from "@/services/signalRService";
import { useAuthContext } from "@/contexts/AuthContext";

interface UseSignalROptions {
  /** Automatically connect when component mounts */
  autoConnect?: boolean;
  /** Attraction IDs to join groups for */
  joinAttractions?: string[];
  /** Callback when registration is created */
  onRegistrationCreated?: (event: RegistrationCreatedEvent) => void;
  /** Callback when registration is deleted */
  onRegistrationDeleted?: (event: RegistrationDeletedEvent) => void;
}

export function useSignalR(options: UseSignalROptions = {}) {
  const {
    autoConnect = true,
    joinAttractions = [],
    onRegistrationCreated,
    onRegistrationDeleted,
  } = options;

  const { user } = useAuthContext();
  const [isConnected, setIsConnected] = useState(signalRService.isConnected());
  const [isConnecting, setIsConnecting] = useState(false);

  const connect = useCallback(async () => {
    if (!user) {
      console.log("Cannot connect to SignalR: User not authenticated");
      return false;
    }

    setIsConnecting(true);
    try {
      const success = await signalRService.start();
      return success;
    } finally {
      setIsConnecting(false);
    }
  }, [user]);

  const disconnect = useCallback(async () => {
    await signalRService.stop();
  }, []);

  const joinAttractionGroup = useCallback(async (attractionId: string) => {
    await signalRService.joinGroup(attractionId);
  }, []);

  const leaveAttractionGroup = useCallback(async (attractionId: string) => {
    await signalRService.leaveGroup(attractionId);
  }, []);

  useEffect(() => {
    if (autoConnect && user) {
      connect();
    }

    return () => {
      // Only disconnect when user logs out (handled in AuthContext)
    };
  }, [autoConnect, user, connect]);

  useEffect(() => {
    const unsubscribe = signalRService.onConnectionStateChange((connected) => {
      setIsConnected(connected);
    });

    return unsubscribe;
  }, []);

  useEffect(() => {
    if (!onRegistrationCreated) return;

    const unsubscribe = signalRService.onRegistrationCreated(onRegistrationCreated);
    return unsubscribe;
  }, [onRegistrationCreated]);

  useEffect(() => {
    if (!onRegistrationDeleted) return;

    const unsubscribe = signalRService.onRegistrationDeleted(onRegistrationDeleted);
    return unsubscribe;
  }, [onRegistrationDeleted]);

  useEffect(() => {
    if (!isConnected || joinAttractions.length === 0) return;

    // Join all specified attraction groups
    joinAttractions.forEach((attractionId) => {
      signalRService.joinGroup(attractionId);
    });

    // Cleanup: leave groups on unmount or when attractions change
    return () => {
      joinAttractions.forEach((attractionId) => {
        signalRService.leaveGroup(attractionId);
      });
    };
  }, [isConnected, joinAttractions]);

  return {
    isConnected,
    isConnecting,
    connect,
    disconnect,
    joinAttractionGroup,
    leaveAttractionGroup,
  };
}
