# SignalR Real-Time Notifications Implementation

## Overview

This document provides a comprehensive overview of the real-time notification system implemented in the Collaborative Shopping List application. The system uses SignalR to enable instant updates across multiple clients, allowing users to see changes to shopping lists and group memberships in real time.

## Architecture

The implementation follows a layered architecture:

- **SignalR Hub (Backend)**: Manages client connections and broadcasts events
- **SignalR Service (Frontend)**: Handles connection to the hub and event registration
- **Notification Service (Frontend)**: Processes events and displays user-friendly notifications

## Key Components

### Backend

#### ShoppingListHub.cs

- Central SignalR hub for all real-time communication
- Manages group-based message routing
- Provides methods for clients to join/leave channels:
  - `JoinShoppingListGroup(int listId)`
  - `LeaveShoppingListGroup(int listId)`
  - `JoinGroupChannel(int groupId)`
  - `LeaveGroupChannel(int groupId)`

#### INotificationService.cs

- Interface defining notification methods for different events
- Ensures consistent notification pattern across the application

```csharp
public interface INotificationService
{
    Task NotifyShoppingListUpdated(int listId, string updatedById);
    Task NotifyShoppingItemAdded(int listId, int itemId, string addedById);
    Task NotifyShoppingItemUpdated(int listId, int itemId, string updatedById);
    Task NotifyShoppingItemRemoved(int listId, int itemId, string removedById);
    Task NotifyUserAddedToGroup(int groupId, int userId, string addedBy);
    Task NotifyUserRemovedFromGroup(int groupId, int userId, string removedBy);
}
```

#### SignalRNotificationService.cs

- Implementation of INotificationService using SignalR
- Broadcasts events to appropriate client groups

```csharp
public class SignalRNotificationService : INotificationService
{
    private readonly IHubContext<ShoppingListHub> _hubContext;

    public SignalRNotificationService(IHubContext<ShoppingListHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyShoppingListUpdated(int listId, string updatedBy)
    {
        await _hubContext.Clients.Group($"shoppingList_{listId}")
            .SendAsync("ShoppingListUpdated", listId, updatedBy);
    }
    
    // Other notification methods...
}
```

#### Service Integration

- Added notification calls to service methods (e.g., ShoppingItemService, ShoppingListService)
- Each service method that modifies data now triggers appropriate notification

### Frontend

#### signalRService.ts

- Manages the SignalR connection lifecycle
- Handles connection events (connect, disconnect, reconnect)
- Provides methods to join/leave channels
- Registers callbacks for different event types

```typescript
class SignalRService {
  private connection: HubConnection | null;
  private callbacks: CallbackRegistry;

  async start(accessToken: string): Promise<boolean> {
    // Establish connection to hub
  }

  async joinShoppingList(listId: string | number): Promise<boolean> {
    // Join a shopping list channel
  }

  onShoppingListUpdated(callback: (listId: number, updatedBy: string) => void): void {
    this.callbacks.shoppingListUpdated.push(callback);
  }
  
  // Other methods...
}
```

#### notificationService.ts

- Higher-level service that uses signalRService
- Tracks all channels the user has joined
- Joins all relevant channels on initialization
- Displays user-friendly notifications using toast messages
- Prevents notifications for user's own actions

```typescript
export const initializeNotificationService = async (
  toast: any,
  authStore: any,
  shoppingListsStore: any,
  groupsStore: any
): Promise<void> {
  // Join all channels and set up event listeners
}

export const joinShoppingList = async (listId: number | string): Promise<void> => {
  // Join a specific shopping list channel
}

export const shutdownNotificationService = async (): Promise<void> => {
  // Clean up connections and state
}
```

#### App.vue

- Initializes the notification service at application startup
- Passes required services to notification service
- Handles cleanup on application shutdown

```vue
<script setup lang="ts">
import { initializeNotificationService, shutdownNotificationService } from './services/notificationService';

// Component setup...

const init = async () => {
  await authStore.checkAuth();
  if (authStore.authenticatedUser) {
    await signalRService.start(authStore.authenticatedUser.token);
    await initializeNotificationService(toast, authStore, shoppingListsStore, groupsStore);
  }
};

onMounted(() => {
  init();
});

onUnmounted(async () => {
  await shutdownNotificationService();
});
</script>
```

## Notification Scenarios

The system handles the following scenarios:

### Shopping List Operations

- List updated
- Item added
- Item updated
- Item removed

### Group Operations

- User added to group
- User removed from group

## User Experience Improvements

- **Context-aware notifications**: Include list names, group names, and usernames
- **Self-action filtering**: Don't show notifications for user's own actions
- **Global notification reception**: Users receive notifications regardless of which page they're on
- **Automatic channel management**: System tracks and manages all channel subscriptions

## Implementation Challenges

- **CORS Configuration**: Ensuring proper CORS setup for SignalR connections
- **Authentication**: Passing JWT tokens with SignalR connections
- **Channel Management**: Tracking which channels users should join/leave
- **Type Mismatches**: Resolving issues with string vs. number parameters
- **Component Lifecycle**: Managing connection state with Vue component lifecycle

## Future Improvements

- **Notification History**: Store and display a history of recent notifications
- **Notification Preferences**: Allow users to configure which notifications they receive
- **Offline Mode**: Queue notifications when offline and sync when reconnected
- **Notification Grouping**: Group similar notifications to reduce visual noise
- **Push Notifications**: Add support for browser push notifications

## TODO

- Implement a more robust error handling and reconnection strategy
- Add unit and integration tests for the notification system
- Consider performance optimizations for users with many groups and lists

## Conclusion

The implemented real-time notification system provides a collaborative experience for users of the shopping list application. By leveraging SignalR, the system ensures that all users stay informed about relevant changes, enhancing the collaborative nature of the application.
