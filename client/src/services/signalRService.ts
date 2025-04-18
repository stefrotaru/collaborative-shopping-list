// src/services/signalRService.ts
import {
  HubConnectionBuilder,
  LogLevel,
  HttpTransportType,
  HubConnection
} from "@microsoft/signalr";

type CallbackFunction = (...args: any[]) => void;

interface CallbackRegistry {
  shoppingListUpdated: Array<(listId: number, updatedBy: string) => void>;
  shoppingItemAdded: Array<(listId: number, itemId: number, addedBy: string) => void>;
  shoppingItemUpdated: Array<(listId: number, itemId: number, updatedBy: string) => void>;
  shoppingItemRemoved: Array<(listId: number, itemId: number, removedBy: string) => void>;
}

class SignalRService {
  private connection: HubConnection | null;
  private callbacks: CallbackRegistry;

  constructor() {
    this.connection = null;
    this.callbacks = {
      shoppingListUpdated: [],
      shoppingItemAdded: [],
      shoppingItemUpdated: [],
      shoppingItemRemoved: [],
    };
  }

  async start(accessToken: string): Promise<boolean> {
    try {
      // Check if we already have an active connection
      if (this.connection && this.connection.state === "Connected") {
        console.log("SignalR already connected.");
        return true;
      }

      // Create a new connection
      this.connection = new HubConnectionBuilder()
        .withUrl("http://localhost:5066/shoppinglisthub", {
          accessTokenFactory: () => accessToken,
          transport:
            HttpTransportType.WebSockets | HttpTransportType.LongPolling,
        })
        .configureLogging(LogLevel.Information)
        .withAutomaticReconnect([0, 2000, 5000, 10000, 15000, 30000]) // Exponential backoff
        .build();

      // Set up connection event handlers
      this.connection.onclose((error?: Error) => {
        console.log("SignalR connection closed", error);
      });

      this.connection.onreconnecting((error?: Error) => {
        console.log("SignalR reconnecting:", error);
      });

      this.connection.onreconnected((connectionId?: string) => {
        console.log("SignalR reconnected with connectionId:", connectionId);
      });

      // Register message handlers
      this.registerHandlers();

      // Start the connection
      await this.connection.start();
      console.log("SignalR connected successfully.");
      return true;
    } catch (err) {
      console.error("SignalR connection error:", err);
      return false;
    }
  }

  private registerHandlers(): void {
    if (!this.connection) return;
    
    this.connection.on("ShoppingListUpdated", (listId: number, updatedBy: string) => {
      console.log(`Shopping list ${listId} updated by ${updatedBy}`);
      this.callbacks.shoppingListUpdated.forEach((callback) =>
        callback(listId, updatedBy)
      );
    });

    this.connection.on("ShoppingItemAdded", (listId: number, itemId: number, addedBy: string) => {
      console.log(
        `Shopping item ${itemId} added to list ${listId} by ${addedBy}`
      );
      this.callbacks.shoppingItemAdded.forEach((callback) =>
        callback(listId, itemId, addedBy)
      );
    });

    this.connection.on("ShoppingItemUpdated", (listId: number, itemId: number, updatedBy: string) => {
      console.log(
        `Shopping item ${itemId} in list ${listId} updated by ${updatedBy}`
      );
      this.callbacks.shoppingItemUpdated.forEach((callback) =>
        callback(listId, itemId, updatedBy)
      );
    });

    this.connection.on("ShoppingItemRemoved", (listId: number, itemId: number, removedBy: string) => {
      console.log(
        `Shopping item ${itemId} removed from list ${listId} by ${removedBy}`
      );
      this.callbacks.shoppingItemRemoved.forEach((callback) =>
        callback(listId, itemId, removedBy)
      );
    });
  }

  async joinShoppingList(listId: string | number): Promise<boolean> {
    if (this.connection && this.connection.state === "Connected") {
      try {
        // Convert the listId to an integer explicitly
        const listIdAsInt = typeof listId === 'string' ? parseInt(listId, 10) : listId;
        
        // Check if it's a valid number
        if (isNaN(listIdAsInt)) {
          console.error(`Invalid list ID: ${listId} is not a number`);
          return false;
        }
        
        await this.connection.invoke("JoinShoppingListGroup", listIdAsInt);
        console.log(`Joined shopping list group ${listIdAsInt}`);
        return true;
      } catch (err) {
        console.error(`Error joining shopping list group ${listId}:`, err);
        return false;
      }
    } else {
      console.warn("Cannot join shopping list group: SignalR not connected");
      return false;
    }
  }

  async leaveShoppingList(listId: string | number): Promise<boolean> {
    if (this.connection && this.connection.state === "Connected") {
      try {
        // Convert the listId to an integer explicitly
        const listIdAsInt = typeof listId === 'string' ? parseInt(listId, 10) : listId;
        
        // Check if it's a valid number
        if (isNaN(listIdAsInt)) {
          console.error(`Invalid list ID: ${listId} is not a number`);
          return false;
        }
        
        await this.connection.invoke("LeaveShoppingListGroup", listIdAsInt);
        console.log(`Left shopping list group ${listIdAsInt}`);
        return true;
      } catch (err) {
        console.error(`Error leaving shopping list group ${listId}:`, err);
        return false;
      }
    }
    return false;
  }

  onShoppingListUpdated(callback: (listId: number, updatedBy: string) => void): void {
    this.callbacks.shoppingListUpdated.push(callback);
  }

  onShoppingItemAdded(callback: (listId: number, itemId: number, addedBy: string) => void): void {
    this.callbacks.shoppingItemAdded.push(callback);
  }

  onShoppingItemUpdated(callback: (listId: number, itemId: number, updatedBy: string) => void): void {
    this.callbacks.shoppingItemUpdated.push(callback);
  }

  onShoppingItemRemoved(callback: (listId: number, itemId: number, removedBy: string) => void): void {
    this.callbacks.shoppingItemRemoved.push(callback);
  }

  async stop(): Promise<boolean> {
    if (this.connection) {
      try {
        await this.connection.stop();
        console.log("SignalR connection stopped.");
        this.connection = null;
        return true;
      } catch (err) {
        console.error("Error stopping SignalR connection:", err);
        return false;
      }
    }
    return false;
  }

  // Utility method to check connection status
  isConnected(): boolean {
    return this.connection !== null && this.connection.state === "Connected";
  }
}

export default new SignalRService();