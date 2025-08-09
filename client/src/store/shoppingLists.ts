import { defineStore } from "pinia";
import { ref } from "vue";

export interface ShoppingList {
  id: number;
  guid: string;
  name: string;
  groupId: number;
  createdById: number;
  lastSynced?: Date;
  isOffline?: boolean;
  pendingChanges?: boolean;
}

export interface ShoppingItem {
  id: number;
  name: string;
  quantity: number;
  isChecked: boolean;
  shoppingListId: number;
  createdById?: number;

  // For offline created items
  tempId?: string;
  lastSynced?: Date;
  isOffline?: boolean;
  pendingChanges?: boolean;
  pendingDelete?: boolean;
}

export const useShoppingListsStore = defineStore("shoppingListsStore", () => {
  const allUserShoppingLists = ref<ShoppingList[]>([]);
  const currentGroupShoppingLists = ref<ShoppingList[]>([]);

  const fetchGroupShoppingLists = async (groupId: number) => {
    try {
      const response = await fetch(
        `/CollaborativeShoppingListAPI/ShoppingLists/group/${groupId}`,
        {
          method: "GET",
          headers: {
            Authorization: `Bearer ${localStorage.getItem("token")}`,
            'Accept': 'application/json'
          },
        }
      );
  
      if (!response.ok) {
        // Try to parse error message from JSON response
        let errorMessage = `Failed to fetch shopping lists (${response.status})`;
        try {
          const errorData = await response.json();
          if (errorData.message) {
            errorMessage = errorData.message;
          }
        } catch (e) {
          // If we can't parse JSON, use the status text
          errorMessage = `Failed to fetch shopping lists: ${response.statusText}`;
        }
        
        console.error(errorMessage);
        throw new Error(errorMessage);
      }
  
      const contentType = response.headers.get('content-type');
      if (!contentType || !contentType.includes('application/json')) {
        console.warn('Response is not JSON:', contentType);
        return [];
      }
  
      currentGroupShoppingLists.value = await response.json();
      return currentGroupShoppingLists.value as ShoppingList[];
    } catch (error) {
      console.error("Error fetching group shopping lists:", error);
      throw error;
    }
  };
  
  const fetchAllGroupsShoppingLists = async (groupIds: number[]) => {
    if (!groupIds || groupIds.length === 0) {
      allUserShoppingLists.value = [];
      return [];
    }
  
    try {
      const response = await fetch(
        `/CollaborativeShoppingListAPI/ShoppingLists/allgroups?groupIds=${groupIds.join(",")}`,
        {
          method: "GET",
          headers: {
            Authorization: `Bearer ${localStorage.getItem("token")}`,
            'Accept': 'application/json'
          },
        }
      );
  
      if (!response.ok) {
        // Try to parse error message from JSON response
        let errorMessage = `Failed to fetch shopping lists (${response.status})`;
        try {
          const errorData = await response.json();
          if (errorData.message) {
            errorMessage = errorData.message;
          }
        } catch (e) {
          // If we can't parse JSON, use the status text
          errorMessage = `Failed to fetch shopping lists: ${response.statusText}`;
        }
        
        console.error(errorMessage);
        throw new Error(errorMessage);
      }
  
      const contentType = response.headers.get('content-type');
      if (!contentType || !contentType.includes('application/json')) {
        console.warn('Response is not JSON:', contentType);
        return [];
      }
  
      allUserShoppingLists.value = await response.json();
      return allUserShoppingLists.value as ShoppingList[];
    } catch (error) {
      console.error("Error fetching all groups shopping lists:", error);
      throw error;
    }
  };

  const createNewList = async (listName: string, groupId: number, userId: number) => {
    try {
      const response = await fetch(
        `/CollaborativeShoppingListAPI/ShoppingLists`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
          body: JSON.stringify({
            name: listName,
            groupId: groupId,
            createdById: userId,
          }),
        }
      );

      if (!response.ok) {
        throw new Error("Failed to create shopping list");
      }

      return await response.json();
    } catch (error) {}
  };

  const deleteList = async (listId: number) => {
    try {
      const response = await fetch(
        `/CollaborativeShoppingListAPI/ShoppingLists/${listId}`,
        {
          method: "DELETE",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
          body: JSON.stringify({ id: listId }),
        }
      );

      if (!response.ok) {
        throw new Error("Failed to delete shopping list");
      }
      
      return response;
    } catch (error) {
      console.error("Error deleting list:", error);
    }
  }

  const createGroup = async (groupName: string, description: string, createdById: number) => { //TODO: Move to groups store
    try {
      const response = await fetch(`CollaborativeShoppingListAPI/Groups`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          name: groupName,
          description,
          createdById
        }),
      });

      console.log(groupName, description, createdById);

      if (!response.ok) {
        throw new Error("Failed to create group");
      }
    } catch (error) {
      console.error("Error creating group:", error);
    }
  };

  const getListItems = async (listId: number) => {
    try {
      const response = await fetch(
        `/CollaborativeShoppingListAPI/ShoppingItems/shoppinglist/${listId}`,
        {
          method: "GET",
          headers: {
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
        }
      );

      if (!response.ok) {
        throw new Error("Failed to fetch shopping list items");
      }

      return await response.json();
    } catch (error) {}
  };

  const getListItemsByGuid = async (guid) => {
    try {
      const response = await fetch(
        `/CollaborativeShoppingListAPI/ShoppingItems/shoppinglist/guid/${guid}`,
        {
          method: "GET",
          headers: {
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
        }
      );
  
      if (!response.ok) {
        throw new Error("Failed to fetch shopping list items");
      }
  
      return await response.json();
    } catch (error) {
      console.error("Error fetching shopping list items by GUID:", error);
      throw error;
    }
  };

  const getShoppingListName = async (listId: number) => {
    try {
      const response = await fetch(
        `/CollaborativeShoppingListAPI/ShoppingLists/${listId}`,
        {
          method: "GET",
          headers: {
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
        }
      );

      if (!response.ok) {
        throw new Error("Failed to fetch shopping list name");
      }

      return await response.json();
    } catch (error) {
      console.error("Error fetching shopping list name:", error);
    }
  }

  const getShoppingListNameByGuid = async (guid) => {
    try {
      const response = await fetch(
        `/CollaborativeShoppingListAPI/ShoppingLists/guid/${guid}`,
        {
          method: "GET",
          headers: {
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
        }
      );
  
      if (!response.ok) {
        throw new Error("Failed to fetch shopping list name");
      }
  
      return await response.json();
    } catch (error) {
      console.error("Error fetching shopping list name by GUID:", error);
      throw error;
    }
  };

  const addListItem = async (listId: number, itemName: string, quantity: number, userId: number) => {
    try {
      const response = await fetch(
        `/CollaborativeShoppingListAPI/ShoppingItems`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
          body: JSON.stringify({
            name: itemName,
            quantity,
            shoppingListId: listId,
            createdById: userId,
          }),
        }
      );

      console.log("Added the following item to the shopping list:", itemName, " x ", quantity);

      if (!response.ok) {
        throw new Error("Failed to add item to shopping list");
      }

      return await response.json();
    } catch (error) {
      console.error("Error adding item to shopping list:", error);
    }
  }

  const removeListItem = async (itemId: number) => {
    try {
      const response = await fetch(
      `/CollaborativeShoppingListAPI/ShoppingItems/${itemId}`,
      {
        method: "DELETE",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        }
      })

      if (!response.ok) {
        throw new Error()
      }

      return response;
    } catch {

    }
  }

  const updateItemCheckedState = async (itemId: number, checkedState: boolean) => {
    try {
      const response = await fetch(
        `/CollaborativeShoppingListAPI/ShoppingItems/updateItemChecked/${itemId}?isChecked=${checkedState}`,
        {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
          body: JSON.stringify({
            id: itemId,
            isChecked: checkedState
          })
        }
      );

      console.log(response)

      if (!response.ok) {
        throw new Error("Failed to add item to shopping list");
      }

      return;
    } catch (error) {
      console.error("Error adding item to shopping list:", error);
    }
  }

  // TODO: add list rename service

  const populateStore = async (groupId: number) => {
    //Should receive an array of groupdId's ti display all Shopping Lists
    console.log('Shopping List populateStore 🛒🛒🛒', groupId)
    await fetchGroupShoppingLists(groupId); // TODO: Should fetch all groupId's for logged user
  }

  const resetStore = () => {
    allUserShoppingLists.value = [];
    currentGroupShoppingLists.value = [];
  }

  return {
    allUserShoppingLists,
    currentGroupShoppingLists,
    fetchGroupShoppingLists,
    fetchAllGroupsShoppingLists,

    createGroup, // move to groups store
    createNewList,
    deleteList,

    getListItems,
    getListItemsByGuid,
    getShoppingListName,
    getShoppingListNameByGuid,
    addListItem,
    removeListItem,
    updateItemCheckedState,

    populateStore,
    resetStore
  };
});
