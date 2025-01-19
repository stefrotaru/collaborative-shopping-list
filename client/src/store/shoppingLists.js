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

  const populateStore = async () => {
    await fetchGroupShoppingLists(4); // TODO: Should fetch all groupId's for logged user
  }

  return {
    userShoppingLists,
    fetchGroupShoppingLists,

    createGroup,
    createNewList,
    deleteList,

    getListItems,

    populateStore
  };
});
