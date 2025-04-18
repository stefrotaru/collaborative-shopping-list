import { ref, type Ref } from 'vue';
import signalRService from './signalRService';

// Track which lists/groups we're already listening to
const joinedShoppingLists: Ref<Set<number>> = ref(new Set<number>());
const joinedGroups: Ref<Set<number>> = ref(new Set<number>());

// Whether the notification service is initialized
const isInitialized: Ref<boolean> = ref(false);

/**
 * Initializes the notification service, connecting to all user's lists and groups
 */
export const initializeNotificationService = async (
  toast: any,
  authStore: any,
  shoppingListsStore: any,
  groupsStore: any
): Promise<void> => {
  // Avoid duplicate initialization
  if (isInitialized.value) {
    console.log("Notification service already initialized");
    return;
  }

  if (!authStore.authenticatedUser) {
    console.warn("Cannot initialize notification service: User not authenticated");
    return;
  }

  const currentUserId = authStore.authenticatedUser.id;
  const currentUserIdentifier = `User ID: ${currentUserId}`;

  console.log("🔔 Starting notification service initialization");

  // First, set up all the event listeners
  setupEventListeners(toast, authStore, shoppingListsStore, groupsStore, currentUserId, currentUserIdentifier);

  // Then, connect to all groups
  try {
    // Get all user's groups
    const groups = await authStore.getUserGroups(true);
    if (groups?.length) {
      console.log(`🔔 Joining ${groups.length} group channels...`);
      
      // Join all group channels
      for (const group of groups) {
        if (!joinedGroups.value.has(group.id)) {
          await signalRService.joinGroupChannel(group.id);
          joinedGroups.value.add(group.id);
          console.log(`🔔 Joined group channel: ${group.name} (ID: ${group.id})`);
        }
      }
      
      // Fetch all shopping lists across groups
      const groupIds = groups.map((group: any) => group.id);
      const lists = await shoppingListsStore.fetchAllGroupsShoppingLists(groupIds);
      
      // Join all shopping list channels
      if (lists?.length) {
        console.log(`🔔 Joining ${lists.length} shopping list channels...`);
        
        for (const list of lists) {
          if (!joinedShoppingLists.value.has(list.id)) {
            await signalRService.joinShoppingList(list.id);
            joinedShoppingLists.value.add(list.id);
            console.log(`🔔 Joined shopping list channel: ${list.name} (ID: ${list.id})`);
          }
        }
      } else {
        console.log("🔔 No shopping lists to join");
      }
    } else {
      console.log("🔔 No groups to join");
    }
    
    isInitialized.value = true;
    console.log("✅ Notification service initialization complete");
  } catch (error) {
    console.error("❌ Error initializing notification service:", error);
    throw error;
  }
};

/**
 * Sets up all event listeners for real-time notifications
 */
const setupEventListeners = (
  toast: any, 
  authStore: any, 
  shoppingListsStore: any, 
  groupsStore: any, 
  currentUserId: number, 
  currentUserIdentifier: string
): void => {
  console.log("🔔 Setting up SignalR event listeners");
  
  // Shopping list events
  signalRService.onShoppingListUpdated(async (listId: number, updatedBy: string) => {
    console.log(`📣 ShoppingListUpdated event: list ${listId} by ${updatedBy}`);
    if (updatedBy !== currentUserIdentifier) {
      try {
        const username = await authStore.getUsernameById(updatedBy);
        const listData = await shoppingListsStore.getShoppingListName(listId);
        const listName = listData?.name || `List ${listId}`;
        
        toast.add({
          severity: "info",
          summary: "List Updated",
          detail: `"${listName}" was updated by ${username}`,
          life: 3000,
        });
      } catch (error) {
        console.error("Error processing shopping list update notification:", error);
      }
    }
  });

  signalRService.onShoppingItemAdded(async (listId: number, itemId: number, addedBy: string) => {
    console.log(`📣 ShoppingItemAdded event: item ${itemId} to list ${listId} by ${addedBy}`);
    if (addedBy !== currentUserIdentifier) {
      try {
        const username = await authStore.getUsernameById(addedBy);
        const listData = await shoppingListsStore.getShoppingListName(listId);
        const listName = listData?.name || `List ${listId}`;
        
        toast.add({
          severity: "info",
          summary: "Item Added",
          detail: `${username} added an item to "${listName}"`,
          life: 3000,
        });
      } catch (error) {
        console.error("Error processing item added notification:", error);
      }
    }
  });

  signalRService.onShoppingItemUpdated(async (listId: number, itemId: number, updatedBy: string) => {
    console.log(`📣 ShoppingItemUpdated event: item ${itemId} in list ${listId} by ${updatedBy}`);
    if (updatedBy !== currentUserIdentifier) {
      try {
        const username = await authStore.getUsernameById(updatedBy);
        const listData = await shoppingListsStore.getShoppingListName(listId);
        const listName = listData?.name || `List ${listId}`;
        
        toast.add({
          severity: "info",
          summary: "Item Updated",
          detail: `${username} updated an item in "${listName}"`,
          life: 3000,
        });
      } catch (error) {
        console.error("Error processing item update notification:", error);
      }
    }
  });

  signalRService.onShoppingItemRemoved(async (listId: number, itemId: number, removedBy: string) => {
    console.log(`📣 ShoppingItemRemoved event: item ${itemId} from list ${listId} by ${removedBy}`);
    if (removedBy !== currentUserIdentifier) {
      try {
        const username = await authStore.getUsernameById(removedBy);
        const listData = await shoppingListsStore.getShoppingListName(listId);
        const listName = listData?.name || `List ${listId}`;
        
        toast.add({
          severity: "info",
          summary: "Item Removed",
          detail: `${username} removed an item from "${listName}"`,
          life: 3000,
        });
      } catch (error) {
        console.error("Error processing item removal notification:", error);
      }
    }
  });

  // Group events
  signalRService.onUserAddedToGroup(async (groupId: number, userId: number, addedBy: string) => {
    console.log(`📣 UserAddedToGroup event: user ${userId} to group ${groupId} by ${addedBy}`);
    
    try {
      // If we're the user being added
      if (userId === currentUserId) {
        // Join the new group channel
        if (!joinedGroups.value.has(groupId)) {
          await signalRService.joinGroupChannel(groupId);
          joinedGroups.value.add(groupId);
          console.log(`🔔 Joined new group channel: ${groupId}`);
          
          // Also join all shopping lists in this group
          try {
            const lists = await shoppingListsStore.fetchGroupShoppingLists(groupId);
            if (lists?.length) {
              for (const list of lists) {
                if (!joinedShoppingLists.value.has(list.id)) {
                  await signalRService.joinShoppingList(list.id);
                  joinedShoppingLists.value.add(list.id);
                  console.log(`🔔 Joined shopping list channel: ${list.name} (ID: ${list.id})`);
                }
              }
            }
          } catch (error) {
            console.error("Error joining shopping lists for new group:", error);
          }
        }
        
        if (addedBy !== currentUserIdentifier) {
          const groupData = await groupsStore.getGroupNameById(groupId);
          const groupName = groupData || `Group ${groupId}`;
          const adderName = await authStore.getUsernameById(addedBy);
          
          toast.add({
            severity: "info",
            summary: "Added to Group",
            detail: `${adderName} added you to "${groupName}"`,
            life: 3000,
          });
        }
      } else if (addedBy !== currentUserIdentifier) {
        // If someone else was added to a group we're in
        const username = await authStore.getUsernameById(userId);
        const groupData = await groupsStore.getGroupNameById(groupId);
        const groupName = groupData || `Group ${groupId}`;
        const adderName = await authStore.getUsernameById(addedBy);
        
        toast.add({
          severity: "info",
          summary: "New Group Member",
          detail: `${adderName} added ${username} to "${groupName}"`,
          life: 3000,
        });
      }
    } catch (error) {
      console.error("Error processing user added to group notification:", error);
    }
  });

  signalRService.onUserRemovedFromGroup(async (groupId: number, userId: number, removedBy: string) => {
    console.log(`📣 UserRemovedFromGroup event: user ${userId} from group ${groupId} by ${removedBy}`);
    
    try {
      // If we're the user being removed
      if (userId === currentUserId) {
        if (removedBy !== currentUserIdentifier) {
          const groupData = await groupsStore.getGroupNameById(groupId);
          const groupName = groupData || `Group ${groupId}`;
          const removerName = await authStore.getUsernameById(removedBy);
          
          toast.add({
            severity: "info",
            summary: "Removed from Group",
            detail: `${removerName} removed you from "${groupName}"`,
            life: 3000,
          });
        }
        
        // Leave the group channel
        if (joinedGroups.value.has(groupId)) {
          await signalRService.leaveGroupChannel(groupId);
          joinedGroups.value.delete(groupId);
          console.log(`🔔 Left group channel: ${groupId}`);
          
          // Also leave all shopping lists only related to this group
          // This requires checking if these lists belong to other groups too
          const groups = await authStore.getUserGroups(true);
          const myGroupIds = groups.map((g: any) => g.id);
          
          // Get lists that might need to be left
          const listsToCheck = [...joinedShoppingLists.value];
          
          for (const listId of listsToCheck) {
            try {
              const listData = await shoppingListsStore.getShoppingListName(listId);
              
              // If list belongs only to the group we're leaving, leave it
              if (listData?.groupId === groupId && !myGroupIds.includes(listData.groupId)) {
                await signalRService.leaveShoppingList(listId);
                joinedShoppingLists.value.delete(listId);
                console.log(`🔔 Left shopping list channel: ${listData.name} (ID: ${listId})`);
              }
            } catch (error) {
              console.error(`Error checking if should leave list ${listId}:`, error);
            }
          }
        }
      } else if (removedBy !== currentUserIdentifier) {
        // If someone else was removed from a group we're in
        const username = await authStore.getUsernameById(userId);
        const groupData = await groupsStore.getGroupNameById(groupId);
        const groupName = groupData || `Group ${groupId}`;
        const removerName = await authStore.getUsernameById(removedBy);
        
        toast.add({
          severity: "info",
          summary: "Group Member Removed",
          detail: `${removerName} removed ${username} from "${groupName}"`,
          life: 3000,
        });
      }
    } catch (error) {
      console.error("Error processing user removed from group notification:", error);
    }
  });
  
  console.log("🔔 Event listeners setup complete");
};

/**
 * Join a shopping list channel and track it
 */
export const joinShoppingList = async (listId: number | string): Promise<void> => {
  const listIdNum = typeof listId === 'string' ? parseInt(listId, 10) : listId;
  if (!joinedShoppingLists.value.has(listIdNum)) {
    await signalRService.joinShoppingList(listIdNum);
    joinedShoppingLists.value.add(listIdNum);
    console.log(`🔔 Joined shopping list channel: ${listIdNum}`);
  }
};

/**
 * Join a group channel and track it
 */
export const joinGroupChannel = async (groupId: number | string): Promise<void> => {
  const groupIdNum = typeof groupId === 'string' ? parseInt(groupId, 10) : groupId;
  if (!joinedGroups.value.has(groupIdNum)) {
    await signalRService.joinGroupChannel(groupIdNum);
    joinedGroups.value.add(groupIdNum);
    console.log(`🔔 Joined group channel: ${groupIdNum}`);
  }
};

/**
 * Leave all channels and stop the SignalR connection
 */
export const shutdownNotificationService = async (): Promise<void> => {
  if (!isInitialized.value) {
    return;
  }
  
  console.log("🔔 Shutting down notification service");
  
  // Stop SignalR connection
  await signalRService.stop();
  
  // Clear tracked lists/groups
  joinedShoppingLists.value.clear();
  joinedGroups.value.clear();
  
  isInitialized.value = false;
  console.log("✅ Notification service shutdown complete");
};

export default {
  initializeNotificationService,
  joinShoppingList,
  joinGroupChannel,
  shutdownNotificationService,
};