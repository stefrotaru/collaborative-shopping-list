<template>
  <div class="shopping-list">
    <div class="shopping-list__header">
      <h2 class="shopping-list__header__title">{{ shoppingListName }}</h2>
    </div>

    <!-- Add item form -->
    <div class="shopping-list__add-item-container">
      <div class="shopping-list__input-wrapper">
        <i class="pi pi-shopping-cart shopping-list__input-icon"></i>
        <InputText 
          v-model="newItemName" 
          placeholder="Add item (e.g. Milk x 2)" 
          class="shopping-list__input" 
          @keyup.enter.prevent="onItemAdded"
        />
        <div v-if="newItemName.trim()" class="shopping-list__quantity-control">
          <button 
            class="shopping-list__quantity-btn shopping-list__quantity-btn--decrease" 
            @click="() => newItemQuantity = Math.max(1, newItemQuantity - 1)"
            aria-label="Decrease quantity"
          >
            <i class="pi pi-minus"></i>
          </button>
          <span class="shopping-list__quantity-display">{{ newItemQuantity }}</span>
          <button 
            class="shopping-list__quantity-btn shopping-list__quantity-btn--increase" 
            @click="() => newItemQuantity++"
            aria-label="Increase quantity"
          >
            <i class="pi pi-plus"></i>
          </button>
        </div>
        <Button 
          icon="pi pi-plus" 
          class="p-button-rounded p-button-primary add-button" 
          aria-label="Add item"
          @click="onItemAdded" 
        />
      </div>
    </div>

    <!-- Shopping list items -->
    <div v-if="listItems.length > 0" class="shopping-list__content">
      <TransitionGroup name="list" tag="ul" class="shopping-list__items">
        <li v-for="item in listItems" :key="item.id" class="shopping-list__item-container">
          <div class="shopping-list__item" :class="{'shopping-list__item--checked': item.isChecked}">
            <div class="shopping-list__item-content">
              <div class="shopping-list__checkbox-container">
                <Checkbox
                  v-model="item.isChecked"
                  :binary="true"
                  class="shopping-list__item-checkbox"
                  @change="toggleCheckShoppingListItem(item)"
                />
              </div>
              <div class="shopping-list__item-details">
                <span 
                  class="shopping-list__item-name" 
                  :class="{'shopping-list__item-name--checked': item.isChecked}"
                >
                  {{ item.name }}
                </span>
                <div v-if="item.quantity > 1" class="shopping-list__item-quantity">× {{ item.quantity }}</div>
              </div>
            </div>
            <div class="shopping-list__item-actions">
              <Button
                icon="pi pi-pencil"
                class="p-button-rounded p-button-outlined p-button-sm p-button-info shopping-list__action-button"
                aria-label="Edit item"
                @click="editItem(item)"
              />
              <Button
                icon="pi pi-trash"
                class="p-button-rounded p-button-outlined p-button-sm p-button-danger shopping-list__action-button"
                aria-label="Delete item"
                @click="confirmDeleteItem(item)"
              />
            </div>
          </div>
        </li>
      </TransitionGroup>
    </div>
    
    <!-- Empty state -->
    <div v-else class="shopping-list__empty-state">
      <i class="pi pi-shopping-bag shopping-list__empty-icon"></i>
      <p>Your shopping list is empty</p>
      <p class="shopping-list__empty-subtext">Add some items to get started</p>
    </div>

    <!-- Delete confirmation dialog -->
    <Dialog v-model:visible="deleteDialog" header="Confirm" :style="{width: '90vw', maxWidth: '400px'}" :modal="true">
      <p>Are you sure you want to delete <strong>{{ itemToDelete?.name }}</strong>?</p>
      <template #footer>
        <Button label="Cancel" icon="pi pi-times" class="p-button-text" @click="deleteDialog = false" />
        <Button label="Delete" icon="pi pi-trash" class="p-button-danger" @click="confirmDelete" />
      </template>
    </Dialog>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from "vue";
import { useRoute, onBeforeRouteUpdate } from "vue-router";
import { useShoppingListsStore } from "../store/shoppingLists";
import { useAuthStore } from "../store/auth";
import { useToast } from "primevue/usetoast";

import InputText from "primevue/inputtext";
import Checkbox from "primevue/checkbox";
import Button from "primevue/button";
import Dialog from "primevue/dialog";

const shoppingListsStore = useShoppingListsStore();
const authStore = useAuthStore();
const toast = useToast();
const route = useRoute();

const shoppingListName = ref("");
const listItems = ref([]);
const newItemName = ref("");
const newItemQuantity = ref(1);
// const newItemQuantity = computed(() => {
//   const match = newItemName.value.match(/x\s*(\d+)$/i);
//   return match ? parseInt(match[1]) : 1;
// });
const deleteDialog = ref(false);
const itemToDelete = ref(null);

const checkNewItemNameForQuantity = () => {
  const match = newItemName.value.match(/x\s*(\d+)$/i);
  if (match) {
    newItemName.value = newItemName.value.replace(/x\s*\d+$/i, "").trim();
    newItemQuantity.value = parseInt(match[1]);
  }
};

const toggleCheckShoppingListItem = async (item) => {
  try {
    // Call API to update the state
    await shoppingListsStore.updateItemCheckedState(item.id, item.isChecked);
    
    // Force update the listItems to ensure reactivity
    listItems.value = [...listItems.value];
    
    // Success notification if needed
    // toast.add({
    //   severity: 'success',
    //   summary: 'Updated',
    //   detail: `Item ${item.isChecked ? 'checked' : 'unchecked'}`,
    //   life: 2000
    // });
  } catch (error) {
    console.error("Error updating item:", error);
    // Revert on error
    item.isChecked = !item.isChecked;
    // Force update again
    listItems.value = [...listItems.value];
    
    toast.add({
      severity: 'error',
      summary: 'Error',
      detail: 'Could not update item',
      life: 3000
    });
  }
};

const onItemAdded = async () => {
  if (!newItemName.value.trim()) {
    toast.add({
      severity: 'warn',
      summary: 'Empty Item',
      detail: 'Please enter an item name',
      life: 3000
    });
    newItemQuantity.value = 1;
    return;
  }

  checkNewItemNameForQuantity();
  
  let listId = route.params.id;
  let itemName = newItemName.value.replace(/\s*x\s*\d+$/i, "").trim();
  let quantity = newItemQuantity.value;
  let userId = authStore.authenticatedUser.id;
  
  try {
    const response = await shoppingListsStore.addListItem(
      listId,
      itemName,
      quantity,
      userId
    );
    
    // Add new item at the top for better visibility
    listItems.value.push(response);
    newItemName.value = "";
    newItemQuantity.value = 1;
    
    toast.add({
      severity: 'success',
      summary: 'Item Added',
      detail: `${itemName} added to your list`,
      life: 2000
    });
  } catch (error) {
    console.error("Error adding item:", error);
    toast.add({
      severity: 'error',
      summary: 'Error',
      detail: 'Could not add item to list',
      life: 3000
    });
  }
};

const editItem = (item) => {
  // Populate input with current item for editing
  newItemName.value = item.name;
  itemQuantity.value = item.quantity;
};

const confirmDeleteItem = (item) => {
  itemToDelete.value = item;
  deleteDialog.value = true;
};

const confirmDelete = async () => {
  if (!itemToDelete.value) return;
  
  try {
    const response = await shoppingListsStore.removeListItem(itemToDelete.value.id);
    
    if (response.ok) {
      let removedItemIndex = listItems.value.findIndex(i => i.id === itemToDelete.value.id);
      listItems.value.splice(removedItemIndex, 1);
      
      toast.add({
        severity: 'info',
        summary: 'Item Removed',
        detail: `${itemToDelete.value.name} removed from list`,
        life: 2000
      });
    }
  } catch (error) {
    console.error("Error deleting item:", error);
    toast.add({
      severity: 'error',
      summary: 'Error',
      detail: 'Could not delete item',
      life: 3000
    });
  } finally {
    deleteDialog.value = false;
    itemToDelete.value = null;
  }
};

onBeforeRouteUpdate(async (to) => {
  if (to.params.id) {
    try {
      const listResponse = await shoppingListsStore.getListItems(to.params.id);
      const listNameResponse = await shoppingListsStore.getShoppingListName(to.params.id);

      listItems.value = listResponse;
      shoppingListName.value = listNameResponse.name;
    } catch (error) {
      console.error("Error fetching shopping list data:", error);
      toast.add({
        severity: 'error',
        summary: 'Error',
        detail: 'Could not load shopping list',
        life: 3000
      });
    }
  }
});

onMounted(async () => {
  const listId = route.params.id;

  try {
    // Fetch shopping list data from the backend API
    const listResponse = await shoppingListsStore.getListItems(listId);
    const listNameResponse = await shoppingListsStore.getShoppingListName(listId);

    listItems.value = listResponse;
    shoppingListName.value = listNameResponse.name;
  } catch (error) {
    console.error("Error fetching shopping list data:", error);
    toast.add({
      severity: 'error',
      summary: 'Error',
      detail: 'Could not load shopping list',
      life: 3000
    });
  }
});
</script>
