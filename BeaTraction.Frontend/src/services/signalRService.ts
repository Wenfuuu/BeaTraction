import * as signalR from "@microsoft/signalr";

export interface RegistrationCreatedEvent {
  userId: string;
  scheduleAttractionId: string;
  registrationId: string;
  registeredAt: string;
  attractionId?: string;
  attractionName?: string;
  scheduleName?: string;
}

export interface RegistrationDeletedEvent {
  userId: string;
  scheduleAttractionId: string;
  registrationId: string;
  attractionId?: string;
  attractionName?: string;
  scheduleName?: string;
}

export type RegistrationCreatedCallback = (event: RegistrationCreatedEvent) => void;
export type RegistrationDeletedCallback = (event: RegistrationDeletedEvent) => void;
export type ConnectionStateCallback = (isConnected: boolean) => void;

class SignalRService {
  private connection: signalR.HubConnection | null = null;
  private isConnecting = false;
  private reconnectAttempts = 0;
  private maxReconnectAttempts = 5;
  private reconnectDelay = 3000;

  // event handlers storage
  private registrationCreatedHandlers: Set<RegistrationCreatedCallback> = new Set();
  private registrationDeletedHandlers: Set<RegistrationDeletedCallback> = new Set();
  private connectionStateHandlers: Set<ConnectionStateCallback> = new Set();

  private joinedGroups: Set<string> = new Set();

  constructor() {
    // initialize connection
    this.buildConnection();
  }

  private buildConnection() {
    const hubUrl = `${import.meta.env.VITE_API_BASE_URL}/hubs/attractions`;
    
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl, {
        withCredentials: true,
      })
      .withAutomaticReconnect({
        nextRetryDelayInMilliseconds: (retryContext) => {
          const delay = Math.min(3000 * Math.pow(2, retryContext.previousRetryCount), 48000);
          console.log(`SignalR reconnecting in ${delay}ms (attempt ${retryContext.previousRetryCount + 1})`);
          return delay;
        },
      })
      .configureLogging(signalR.LogLevel.Information)
      .build();

    // event listeners
    this.setupEventListeners();
    this.setupConnectionHandlers();
  }

  private setupEventListeners() {
    if (!this.connection) return;

    // listen for RegistrationCreated events
    this.connection.on("RegistrationCreated", (event: RegistrationCreatedEvent) => {
      console.log("SignalR: RegistrationCreated event received", event);
      this.registrationCreatedHandlers.forEach((handler) => handler(event));
    });

    // listen for RegistrationDeleted events
    this.connection.on("RegistrationDeleted", (event: RegistrationDeletedEvent) => {
      console.log("SignalR: RegistrationDeleted event received", event);
      this.registrationDeletedHandlers.forEach((handler) => handler(event));
    });
  }

  private setupConnectionHandlers() {
    if (!this.connection) return;

    this.connection.onclose((error) => {
      console.log("SignalR connection closed", error);
      this.notifyConnectionState(false);
      this.joinedGroups.clear();
      
      if (error && this.reconnectAttempts < this.maxReconnectAttempts) {
        setTimeout(() => this.start(), this.reconnectDelay);
        this.reconnectAttempts++;
      }
    });

    this.connection.onreconnecting((error) => {
      console.log("SignalR reconnecting...", error);
      this.notifyConnectionState(false);
    });

    this.connection.onreconnected((connectionId) => {
      console.log("SignalR reconnected", connectionId);
      this.notifyConnectionState(true);
      this.reconnectAttempts = 0;
      
      this.rejoinGroups();
    });
  }

  private notifyConnectionState(isConnected: boolean) {
    this.connectionStateHandlers.forEach((handler) => handler(isConnected));
  }

  private async rejoinGroups() {
    const groups = Array.from(this.joinedGroups);
    this.joinedGroups.clear();
    
    for (const groupName of groups) {
      await this.joinGroup(groupName);
    }
  }

  async start(): Promise<boolean> {
    if (!this.connection) {
      this.buildConnection();
    }

    if (this.connection?.state === signalR.HubConnectionState.Connected) {
      console.log("SignalR already connected");
      return true;
    }

    if (this.isConnecting) {
      console.log("SignalR connection already in progress");
      return false;
    }

    try {
      this.isConnecting = true;
      console.log("Starting SignalR connection...");
      
      await this.connection!.start();
      
      console.log("SignalR connected successfully");
      this.notifyConnectionState(true);
      this.reconnectAttempts = 0;
      
      return true;
    } catch (error) {
      console.error("SignalR connection failed:", error);
      this.notifyConnectionState(false);
      
      if (this.reconnectAttempts < this.maxReconnectAttempts) {
        this.reconnectAttempts++;
        setTimeout(() => this.start(), this.reconnectDelay);
      }
      
      return false;
    } finally {
      this.isConnecting = false;
    }
  }

  async stop(): Promise<void> {
    if (!this.connection) return;

    try {
      console.log("Stopping SignalR connection...");
      await this.connection.stop();
      this.joinedGroups.clear();
      console.log("SignalR disconnected");
    } catch (error) {
      console.error("Error stopping SignalR connection:", error);
    }
  }

  async joinGroup(attractionId: string): Promise<void> {
    if (!this.connection || this.connection.state !== signalR.HubConnectionState.Connected) {
      console.warn("Cannot join group: SignalR not connected");
      return;
    }

    try {
      await this.connection.invoke("JoinAttractionGroup", attractionId);
      this.joinedGroups.add(attractionId);
      console.log(`Joined attraction group: ${attractionId}`);
    } catch (error) {
      console.error(`Failed to join attraction group ${attractionId}:`, error);
    }
  }

  async leaveGroup(attractionId: string): Promise<void> {
    if (!this.connection || this.connection.state !== signalR.HubConnectionState.Connected) {
      console.warn("Cannot leave group: SignalR not connected");
      return;
    }

    try {
      await this.connection.invoke("LeaveAttractionGroup", attractionId);
      this.joinedGroups.delete(attractionId);
      console.log(`Left attraction group: ${attractionId}`);
    } catch (error) {
      console.error(`Failed to leave attraction group ${attractionId}:`, error);
    }
  }

  onRegistrationCreated(callback: RegistrationCreatedCallback): () => void {
    this.registrationCreatedHandlers.add(callback);
    
    return () => {
      this.registrationCreatedHandlers.delete(callback);
    };
  }

  onRegistrationDeleted(callback: RegistrationDeletedCallback): () => void {
    this.registrationDeletedHandlers.add(callback);
    
    return () => {
      this.registrationDeletedHandlers.delete(callback);
    };
  }

  onConnectionStateChange(callback: ConnectionStateCallback): () => void {
    this.connectionStateHandlers.add(callback);
    
    return () => {
      this.connectionStateHandlers.delete(callback);
    };
  }

  isConnected(): boolean {
    return this.connection?.state === signalR.HubConnectionState.Connected;
  }

  getConnectionState(): signalR.HubConnectionState | null {
    return this.connection?.state ?? null;
  }
}

export const signalRService = new SignalRService();
