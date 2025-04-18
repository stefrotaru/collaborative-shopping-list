<template>
  <div class="shopping-list">
    <LoadingState v-if="delayedLoading" message="Loading shopping list..." />
    <ErrorState
      v-else-if="error"
      :message="error"
      @back="$router.push('/shoppinglists')"
      @retry="fetchShoppingListData"
    />

    <div class="shopping-list__header">
      <h2 class="shopping-list__header__title">{{ shoppingListName }}</h2>
      <!-- Access information badge -->
      <div v-if="accessInfo" class="shopping-list__access-info">
        <span
          class="shopping-list__access-badge"
          :class="{
            'shopping-list__owner-badge': isOwner,
            'shopping-list__shared-badge': !isOwner,
          }"
        >
          {{ isOwner ? "Owner" : accessInfo.accessReason }}
        </span>
      </div>
    </div>

    <!-- Add item form - Only visible to owners or if list allows collaborative editing -->
    <div v-if="isOwner || canEdit" class="shopping-list__add-item-container">
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
            @click="() => (newItemQuantity = Math.max(1, newItemQuantity - 1))"
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
        <li
          v-for="item in listItems"
          :key="item.id"
          class="shopping-list__item-container"
        >
          <div
            class="shopping-list__item"
            :class="{ 'shopping-list__item--checked': item.isChecked }"
          >
            <div class="shopping-list__item-content">
              <div class="shopping-list__checkbox-container">
                <Checkbox
                  v-model="item.isChecked"
                  :binary="true"
                  class="shopping-list__item-checkbox"
                  @change="toggleCheckShoppingListItem(item)"
                  :disabled="!canEdit"
                />
              </div>
              <div class="shopping-list__item-details">
                <span
                  class="shopping-list__item-name"
                  :class="{
                    'shopping-list__item-name--checked': item.isChecked,
                  }"
                >
                  {{ item.name }}
                </span>
                <div
                  v-if="item.quantity > 1"
                  class="shopping-list__item-quantity"
                >
                  × {{ item.quantity }}
                </div>
              </div>
            </div>
            <div class="shopping-list__item-actions">
              <!-- Only show edit/delete buttons to owners or if editing is allowed -->
              <template v-if="isOwner || canEdit">
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
              </template>
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

    <!-- Read-only notification for non-owners -->
    <div
      v-if="!canEdit && listItems.length > 0"
      class="shopping-list__readonly-notice"
    >
      <i class="pi pi-info-circle"></i>
      <span
        >This list is shared with you through a group. You can view but not
        modify it.</span
      >
    </div>

    <!-- Delete confirmation dialog -->
    <Dialog
      v-model:visible="deleteDialog"
      header="Confirm"
      :style="{ width: '90vw', maxWidth: '400px' }"
      :modal="true"
    >
      <p>
        Are you sure you want to delete <strong>{{ itemToDelete?.name }}</strong
        >?
      </p>
      <template #footer>
        <Button
          label="Cancel"
          icon="pi pi-times"
          class="p-button-text"
          @click="deleteDialog = false"
        />
        <Button
          label="Delete"
          icon="pi pi-trash"
          class="p-button-danger"
          @click="confirmDelete"
        />
      </template>
    </Dialog>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted, watch } from "vue";
import { useRoute, useRouter, onBeforeRouteUpdate } from "vue-router";
import { useShoppingListsStore } from "../store/shoppingLists";
import { useAuthStore } from "../store/auth";
import { useAccessibleShoppingListsStore } from "../store/accessibleShoppingLists";
import { useToast } from "primevue/usetoast";

import LoadingState from "../components/common/LoadingState.vue";
import { useDelayedLoading } from "../components/composables/useDelayedLoading";
import ErrorState from "../components/common/ErrorState.vue";

import InputText from "primevue/inputtext";
import Checkbox from "primevue/checkbox";
import Button from "primevue/button";
import Dialog from "primevue/dialog";

import signalRService from '../services/signalRService';

const shoppingListsStore = useShoppingListsStore();
const authStore = useAuthStore();
const accessibleListsStore = useAccessibleShoppingListsStore();
const toast = useToast();
const route = useRoute();
const router = useRouter();

// List data and state
const shoppingListName = ref("");
const listItems = ref([]);
const { loading, delayedLoading, setLoading } = useDelayedLoading(500);
const error = ref(null);

// New item form state
const newItemName = ref("");
const newItemQuantity = ref(1);
const deleteDialog = ref(false);
const itemToDelete = ref(null);

// Access control state
const accessInfo = ref(null);
const isOwner = ref(false);
const canEdit = ref(true); // If you want to allow non-owners to edit (collaborative editing)

const checkNewItemNameForQuantity = () => {
  const match = newItemName.value.match(/x\s*(\d+)$/i);
  if (match) {
    newItemName.value = newItemName.value.replace(/x\s*\d+$/i, "").trim();
    newItemQuantity.value = parseInt(match[1]);
  }
};

// Check if the user has access to this shopping list
const checkListAccess = async (listId) => {
  try {
    const normalizedlistId = Number(listId);

    // First, check if user can access this list at all
    const canAccess = await accessibleListsStore.canAccessShoppingList(normalizedlistId);
    if (!canAccess) {
      error.value = "You do not have permission to view this shopping list";
      toast.add({
        severity: "error",
        summary: "Access Denied",
        detail: "You do not have permission to view this shopping list",
        life: 3000,
      });
      router.push("/access-denied");
      return false;
    }

    // Get the specific access information for this list
    await accessibleListsStore.fetchAccessibleLists();
    accessInfo.value = accessibleListsStore.accessibleLists.find(
      (list) => list.shoppingListId === normalizedlistId
    );

    // Set access control flags
    isOwner.value = accessInfo.value?.accessReason === "Owner";

    return true;
  } catch (error) {
    console.error("Error checking list access:", error);
    error.value = "Could not verify your access to this list";
    toast.add({
      severity: "error",
      summary: "Access Error",
      detail: "Could not verify your access to this list",
      life: 3000,
    });
    return false;
  }
};

const toggleCheckShoppingListItem = async (item) => {
  // Check if user has permission to modify
  if (!canEdit.value) {
    item.isChecked = !item.isChecked; // Revert the check
    toast.add({
      severity: "warn",
      summary: "Permission Denied",
      detail: "You do not have permission to modify this list",
      life: 3000,
    });
    return;
  }

  try {
    // Call API to update the state
    await shoppingListsStore.updateItemCheckedState(item.id, item.isChecked);

    // Force update the listItems to ensure reactivity
    listItems.value = [...listItems.value];
  } catch (error) {
    console.error("Error updating item:", error);
    // Revert on error
    item.isChecked = !item.isChecked;
    // Force update again
    listItems.value = [...listItems.value];

    toast.add({
      severity: "error",
      summary: "Error",
      detail: "Could not update item",
      life: 3000,
    });
  }
};

const onItemAdded = async () => {
  // Check if user has permission to add items
  if (!canEdit.value) {
    toast.add({
      severity: "warn",
      summary: "Permission Denied",
      detail: "You do not have permission to add items to this list",
      life: 3000,
    });
    return;
  }

  if (!newItemName.value.trim()) {
    toast.add({
      severity: "warn",
      summary: "Empty Item",
      detail: "Please enter an item name",
      life: 3000,
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

    // Add new item to the list
    listItems.value.push(response);
    newItemName.value = "";
    newItemQuantity.value = 1;

    toast.add({
      severity: "success",
      summary: "Item Added",
      detail: `${itemName} added to your list`,
      life: 2000,
    });
  } catch (error) {
    console.error("Error adding item:", error);
    toast.add({
      severity: "error",
      summary: "Error",
      detail: "Could not add item to list",
      life: 3000,
    });
  }
};

const editItem = (item) => {
  // Check if user has permission to edit
  if (!canEdit.value) {
    toast.add({
      severity: "warn",
      summary: "Permission Denied",
      detail: "You do not have permission to edit items in this list",
      life: 3000,
    });
    return;
  }

  // Populate input with current item for editing
  newItemName.value = item.name;
  newItemQuantity.value = item.quantity;
};

const confirmDeleteItem = (item) => {
  // Check if user has permission to delete
  if (!canEdit.value) {
    toast.add({
      severity: "warn",
      summary: "Permission Denied",
      detail: "You do not have permission to delete items from this list",
      life: 3000,
    });
    return;
  }

  itemToDelete.value = item;
  deleteDialog.value = true;
};

const confirmDelete = async () => {
  if (!itemToDelete.value) return;

  try {
    const response = await shoppingListsStore.removeListItem(
      itemToDelete.value.id
    );

    if (response.ok) {
      let removedItemIndex = listItems.value.findIndex(
        (i) => i.id === itemToDelete.value.id
      );
      listItems.value.splice(removedItemIndex, 1);

      toast.add({
        severity: "info",
        summary: "Item Removed",
        detail: `${itemToDelete.value.name} removed from list`,
        life: 2000,
      });
    }
  } catch (error) {
    console.error("Error deleting item:", error);
    toast.add({
      severity: "error",
      summary: "Error",
      detail: "Could not delete item",
      life: 3000,
    });
  } finally {
    deleteDialog.value = false;
    itemToDelete.value = null;
  }
};

const fetchShoppingListData = async (listId) => {
  setLoading(true);
  error.value = null;

  try {
    const hasAccess = await checkListAccess(listId);
    if (!hasAccess) {
      return;
    }

    const listResponse = await shoppingListsStore.getListItems(listId);
    const listNameResponse = await shoppingListsStore.getShoppingListName(listId);

    listItems.value = listResponse;
    shoppingListName.value = listNameResponse.name;
  } catch (error) {
    console.error("Error fetching shopping list data:", error);
    error.value = "Could not load shopping list";
    toast.add({
      severity: "error",
      summary: "Error",
      detail: "Could not load shopping list",
      life: 3000,
    });
  } finally {
    setLoading(false);
  }
};

onBeforeRouteUpdate(async (to) => {
  if (to.params.id) {
    await fetchShoppingListData(to.params.id);
  }
});

onMounted(async () => {
  const hasAccess = await checkListAccess(route.params.id);

  if (!hasAccess) {
    return;
  }

  const listId = route.params.id;
  const currentUserId = authStore.authenticatedUser.id;
  // Create a user identifier string similar to the one used in notifications
  const currentUserIdentifier = `User ID: ${currentUserId}`;

  // If user has access, load the shopping list data
  await fetchShoppingListData(listId);

  // Join the SignalR group for this shopping list
  await signalRService.joinShoppingList(listId);
      
  // Listen for updates
  signalRService.onShoppingListUpdated(async (updatedListId, updatedBy) => {
    if (Number(updatedListId) === Number(listId)) {
      const username = await authStore.getUsernameById(updatedBy);

      console.log(`🚀🚀🚀🚀🚀🚀List updated by ${updatedBy} (${username})`);
      
      // Only show toast if action wasn't performed by current user
      if (updatedBy !== currentUserIdentifier) {
        toast.add({
          severity: "info",
          summary: "List Updated",
          detail: `List updated by ${username}`,
          life: 2000,
        });
      }
      
      fetchShoppingListData(listId);
    }
  });
  
  signalRService.onShoppingItemAdded(async (updatedListId, itemId, addedBy) => {
    if (Number(updatedListId) === Number(listId)) {
      const username = await authStore.getUsernameById(addedBy);

      console.log(`🚀🚀🚀🚀🚀🚀New item added by ${addedBy} (${username})`);
      
      // Only show toast if action wasn't performed by current user
      if (addedBy !== currentUserIdentifier) {
        toast.add({
          severity: "info",
          summary: "New Item Added",
          detail: `New item added by ${username}`,
          life: 2000,
        });
      }
      
      fetchShoppingListData(listId);
    }
  });

  signalRService.onShoppingItemUpdated(async(updatedListId, itemId, updatedBy) => {
    if (Number(updatedListId) === Number(listId)) {
      const username = await authStore.getUsernameById(updatedBy);

      console.log(`🚀🚀🚀🚀🚀🚀Item updated by ${updatedBy} (${username})`);
      
      // Only show toast if action wasn't performed by current user
      if (updatedBy !== currentUserIdentifier) {
        toast.add({
          severity: "info",
          summary: "Item Updated",
          detail: `Item updated by ${username}`,
          life: 2000,
        });
      }
      
      fetchShoppingListData(listId);
    }
  });

  signalRService.onShoppingItemRemoved(async (updatedListId, itemId, removedBy) => {
    if (Number(updatedListId) === Number(listId)) {
      const username = await authStore.getUsernameById(removedBy);

      console.log(`🚀🚀🚀🚀🚀🚀Item removed by ${removedBy} (${username})`);
      
      // Only show toast if action wasn't performed by current user
      if (removedBy !== currentUserIdentifier) {
        toast.add({
          severity: "info",
          summary: "Item Removed",
          detail: `Item removed by ${username}`,
          life: 2000,
        });
      }
      
      fetchShoppingListData(listId);
    }
  });
});

onUnmounted(async () => {
  // Leave the group when component is unmounted
  await signalRService.leaveShoppingList(route.params.id);
});
</script>
