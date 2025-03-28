import { defineStore } from "pinia";
import { ref } from "vue";
import { useAuthStore } from "./auth";

export interface AccessibleShoppingList {
  shoppingListId: number;
  shoppingListName: string;
  groupId: number;
  groupName: string;
  accessReason: string;
}

export const useAccessibleShoppingListsStore = defineStore("accessibleShoppingListsStore", () => {
  const accessibleLists = ref<AccessibleShoppingList[]>([]);
  const isLoading = ref(false);
  const error = ref<string | null>(null);

  // Fetch all shopping lists the user can access
  const fetchAccessibleLists = async () => {
    isLoading.value = true;
    error.value = null;

    try {
      const response = await fetch('/CollaborativeShoppingListAPI/AccessibleShoppingLists', {
        method: 'GET',
        headers: {
          Authorization: `Bearer ${localStorage.getItem('token')}`,
          'Accept': 'application/json'  // Explicitly request JSON response
        },
      });

      if (!response.ok) {
        throw new Error(`Failed to fetch accessible shopping lists: ${response.status} ${response.statusText}`);
      }

      // Check if the response is JSON
      const contentType = response.headers.get('content-type');
      if (!contentType || !contentType.includes('application/json')) {
        console.warn('Response is not JSON:', contentType);
        accessibleLists.value = [];
        return [];
      }

      accessibleLists.value = await response.json();
      return accessibleLists.value;
    } catch (err) {
      console.error('Error fetching accessible shopping lists:', err);
      if (err instanceof Error) {
        error.value = err.message;
      } else {
        error.value = 'Unknown error occurred';
      }
      return [];
    } finally {
      isLoading.value = false;
    }
  };

  // Check if user can access a specific shopping list
  const canAccessShoppingList = async (shoppingListId: number): Promise<boolean> => {
    try {
      // Fallback approach: Check if the list exists and if the user can access it
      const authStore = useAuthStore();
      if (!authStore.authenticatedUser) {
        return false;
      }
      
      // Try to get the shopping list directly first
      const listResponse = await fetch(`/CollaborativeShoppingListAPI/ShoppingLists/${shoppingListId}`, {
        method: 'GET',
        headers: {
          Authorization: `Bearer ${localStorage.getItem('token')}`,
          'Accept': 'application/json'  // Explicitly request JSON response
        },
      });
      
      // If we can fetch the list successfully, the user has access
      if (listResponse.ok) {
        // Check if the response is JSON
        const contentType = listResponse.headers.get('content-type');
        if (contentType && contentType.includes('application/json')) {
          const listData = await listResponse.json();
          
          // If user is the creator, they definitely have access
          if (listData.createdById === authStore.authenticatedUser.id) {
            console.log('User is creator of the list, access granted');
            return true;
          }
        }
      }
      
      // Now try the actual authorization endpoint as a backup
      const response = await fetch(`/CollaborativeShoppingListAPI/AccessibleShoppingLists/can-access/${shoppingListId}`, {
        method: 'GET',
        headers: {
          Authorization: `Bearer ${localStorage.getItem('token')}`,
          'Accept': 'application/json'  // Explicitly request JSON response
        },
      });

      if (!response.ok) {
        console.warn(`Access check failed with status: ${response.status}`);
        return false;
      }

      // Check if the response is JSON
      const contentType = response.headers.get('content-type');
      if (!contentType || !contentType.includes('application/json')) {
        console.warn('Access check response is not JSON:', contentType);
        
        // If we couldn't verify access via the API but could retrieve the list,
        // we'll cautiously grant access
        if (listResponse.ok) {
          console.log('Granting access based on successful list retrieval');
          return true;
        }
        
        return false;
      }

      const data = await response.json();
      return data.canAccess === true;
    } catch (err) {
      console.error('Error checking shopping list access:', err);
      return false;
    }
  };

  // Get details for all accessible shopping lists
  const fetchAccessibleListDetails = async () => {
    isLoading.value = true;
    error.value = null;

    try {
      const response = await fetch('/CollaborativeShoppingListAPI/AccessibleShoppingLists/details', {
        method: 'GET',
        headers: {
          Authorization: `Bearer ${localStorage.getItem('token')}`,
          'Accept': 'application/json'  // Explicitly request JSON response
        },
      });

      if (!response.ok) {
        throw new Error('Failed to fetch accessible shopping list details');
      }

      // Check if the response is JSON
      const contentType = response.headers.get('content-type');
      if (!contentType || !contentType.includes('application/json')) {
        console.warn('Response is not JSON:', contentType);
        return [];
      }

      const listDetails = await response.json();
      return listDetails;
    } catch (err) {
      console.error('Error fetching accessible shopping list details:', err);
      if (err instanceof Error) {
        error.value = err.message;
      } else {
        error.value = 'Unknown error occurred';
      }
      return [];
    } finally {
      isLoading.value = false;
    }
  };

  // Filter lists by ownership (lists user owns)
  const getOwnedLists = () => {
    return accessibleLists.value.filter(list => list.accessReason === 'Owner');
  };

  // Filter lists by sharing (lists shared with user via groups)
  const getSharedLists = () => {
    return accessibleLists.value.filter(list => list.accessReason !== 'Owner');
  };

  // Reset the store
  const resetStore = () => {
    accessibleLists.value = [];
    error.value = null;
  };

  return {
    accessibleLists,
    isLoading,
    error,
    fetchAccessibleLists,
    canAccessShoppingList,
    fetchAccessibleListDetails,
    getOwnedLists,
    getSharedLists,
    resetStore
  };
});