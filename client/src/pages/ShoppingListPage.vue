<template>
  <div class="shopping-list-page">
    <div class="shopping-list-page__title">
      <h2>{{ shoppingListName }}</h2>
    </div>

    <ul class="shopping-list-page__list">
      <li v-for="item in listItems" :key="item.id">
        <div
          class="shopping-list-page__list-item"
          :class="
            item.isChecked ? 'shopping-list-page__list-item--checked' : ''
          "
        >
          <i
            v-if="item.isChecked"
            class="pi pi-check shopping-list-page__list-item__checkmark"
            style="font-size: 1rem"
          ></i>
          <div class="shopping-list-page__list-item__name">{{ item.name }}</div>
          <div class="shopping-list-page__list-item__quantity">
            {{ item.quantity }}
          </div>
          <div class="shopping-list-page__list-item__checked">
            <Checkbox
              v-model="item.isChecked"
              :binary="true"
              @change="toggleCheckShoppingListItem(item)"
            />
          </div>
          <div class="shopping-list-page__list-item__actions">
            <Button
              icon="pi pi-pencil"
              class="p-button-rounded p-button-success mr-2"
              @click="editItem(item)"
            />
            <Button
              icon="pi pi-trash"
              class="p-button-rounded p-button-danger"
              @click="deleteItem(item)"
            />
          </div>
        </div>
      </li>
      <li class="shopping-list-page__list__add-item">
        <InputText
          v-model="newItemName"
          placeholder="item name x quantity"
          @keyup.enter.prevent="onItemAdded"
        />
        <Button label="Add Item" icon="pi pi-plus" @click="onItemAdded" />
      </li>
    </ul>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from "vue";
import { useRoute, onBeforeRouteUpdate } from "vue-router";
import { useShoppingListsStore } from "../store/shoppingLists";
import { useAuthStore } from "../store/auth";

import InputText from "primevue/inputtext";
import Checkbox from "primevue/checkbox";
import Button from "primevue/button";

const shoppingListsStore = useShoppingListsStore();
const authStore = useAuthStore();

const route = useRoute();
const shoppingListName = ref("");

const listItems = ref([]);
const newItemName = ref("");
const newItemQuantity = computed(() => {
  const match = newItemName.value.match(/x\s*(\d+)$/i);
  return match ? parseInt(match[1]) : 1;
});

const toggleCheckShoppingListItem = async (item) => {
  console.log("Toggle Check Shopping List Item", item);
  let itemId = item.id;
  let checkedState = item.isChecked;
  try {
    const response = await shoppingListsStore.updateItemCheckedState(
      itemId,
      checkedState
    );

    // Remove item from current position and add to end
    // if (checkedState) {
    //   const index = listItems.value.findIndex((i) => i.id === item.id);
    //   if (index > -1) {
    //     const [removedItem] = listItems.value.splice(index, 1);
    //     listItems.value.push(removedItem);
    //   }
    // }
  } catch (error) {
    console.error("Error adding item to list:", error);
  }
};

const onItemAdded = async () => {
  // (listId, itemName, quantity, userId)
  if (listItems.value) {
    let listId = route.params.id;
    let itemName = newItemName.value.replace(/\s*x\s*\d+$/i, "").trim();
    let quantity = newItemQuantity.value || 1;
    let userId = authStore.authenticatedUser.id;
    try {
      const response = await shoppingListsStore.addListItem(
        listId,
        itemName,
        quantity,
        userId
      );
      console.log("Item added:", response);
      listItems.value.push(response);
      newItemName.value = "";
    } catch (error) {
      console.error("Error adding item to list:", error);
    }
  }
};

const editItem = (item) => {
  // TODO: Implement edit item functionality
  console.log("Edit item:", item);
};

const deleteItem = async (item) => {
  // TODO: Implement delete item confirmation and functionality
  console.log("Delete item:", item);
  try {
    const itemId = item.id;
    
    const response = await shoppingListsStore.removeListItem(itemId);
    
    if (response.ok) {
      let removedItemIndex = listItems.value.findIndex(i => i.id === itemId);
      listItems.value.splice(removedItemIndex, 1);
    }
  } catch (error) {
    console.error("Error deleting item:", error)
  }
};

// watch for listItems changes
watch(listItems, (newVal) => {
  console.log("List items changed:", newVal);
  listItems.value = newVal;
});

onBeforeRouteUpdate(async (to) => {
  if (to.params.id) {
    console.log(to);
    try {
      const listResponse = await shoppingListsStore.getListItems(to.params.id);
      const listNameResponse = await shoppingListsStore.getShoppingListName(
        to.params.id
      );

      console.log("New list response:", listResponse);

      listItems.value = listResponse;
      shoppingListName.value = listNameResponse.name;
    } catch (error) {
      console.error("Error fetching shopping list data:", error);
    }
  }
});

onMounted(async () => {
  const listId = route.params.id;

  try {
    // Fetch shopping list data from the backend API
    const listResponse = await shoppingListsStore.getListItems(listId);
    const listNameResponse = await shoppingListsStore.getShoppingListName(
      listId
    );

    console.log("list name: ", listNameResponse);
    console.log(listResponse);

    listItems.value = listResponse;
    shoppingListName.value = listNameResponse.name;
  } catch (error) {
    console.error("Error fetching shopping list data:", error);
  }
});
</script>
