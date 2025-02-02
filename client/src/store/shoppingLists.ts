import { defineStore } from "pinia";
import { ref } from "vue";

export const useShoppingListsStore = defineStore("shoppingListsStore", () => {
  const userShoppingLists = ref([]);

  const fetchGroupShoppingLists = async (groupId) => {
    try {
      const response = await fetch(
        `CollaborativeShoppingListAPI/ShoppingLists/group/${groupId}`,
        {
          method: "GET",
          headers: {
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
        }
      );

      if (!response.ok) {
        throw new Error("Failed to fetch shopping lists");
      }

      userShoppingLists.value = await response.json();

      return userShoppingLists.value;
    } catch (error) {}
  };

  const fetchAllGroupsShoppingLists = async (groupIds) => {
    if (!groupIds) {
      return;
    }

    try {
      const response = await fetch(
        `/CollaborativeShoppingListAPI/ShoppingLists/allgroups?groupIds=${groupIds.join(",")}`,
        {
          method: "GET",
          headers: {
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
        }
      );
   
      if (!response.ok) {
        throw new Error("Failed to fetch shopping lists");
      }
   
      userShoppingLists.value = await response.json();
      return userShoppingLists.value;
    } catch (error) {}
   };

  const createNewList = async (listName, groupId, userId) => {
    try {
      const response = await fetch(
        `CollaborativeShoppingListAPI/ShoppingLists`,
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

  const deleteList = async (listId) => {
    try {
      const response = await fetch(
        `CollaborativeShoppingListAPI/ShoppingLists/${listId}`,
        {
          method: "DELETE",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({ id: listId }),
        }
      );
    } catch (error) {
      console.error("Error deleting list:", error);
    }
  }

  const createGroup = async (groupName, description, createdById) => {
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

  const getListItems = async (listId) => {
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

  const getShoppingListName = async (listId) => {
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

  const addListItem = async (listId, itemName, quantity, userId) => {
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

  const removeListItem = async (itemId) => {
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

  const updateItemCheckedState = async (itemId, checkedState) => {
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

  const populateStore = async (groupId) => {
    //Should receive an array of groupdId's ti display all Shopping Lists
    console.log('Shopping List populateStore 🛒🛒🛒', groupId)
    await fetchGroupShoppingLists(groupId); // TODO: Should fetch all groupId's for logged user
  }

  const resetStore = () => {
    userShoppingLists.value = [];
  }

  return {
    userShoppingLists,
    fetchGroupShoppingLists,
    fetchAllGroupsShoppingLists,

    createGroup, // move to groups store
    createNewList,
    deleteList,

    getListItems,
    getShoppingListName,
    addListItem,
    removeListItem,
    updateItemCheckedState,

    populateStore,
    resetStore
  };
});
