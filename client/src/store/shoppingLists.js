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
            createdById: userId
          }),
        }
      );

      if (!response.ok) {
        throw new Error("Failed to create shopping list");
      }

      return await response.json();
    } catch (error) {}
  }

  return {
    userShoppingLists,
    fetchGroupShoppingLists,

    createNewList
  };
});
