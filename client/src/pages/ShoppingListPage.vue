<template>
  <div class="shopping-list-page">
    <h2>Shopping List</h2>
    <div class="mb-4">
      <Button label="Add Item" icon="pi pi-plus" @click="openAddItemDialog" />
    </div>
    <DataTable :value="listItems" responsiveLayout="scroll">
      <Column field="name" header="Item"></Column>
      <Column field="quantity" header="Quantity"></Column>
      <Column field="isChecked" header="Checked">
        <template #body="slotProps">
          <Checkbox v-model="slotProps.data.isChecked" :binary="true" />
        </template>
      </Column>
      <Column header="Actions">
        <template #body="slotProps">
          <Button
            icon="pi pi-pencil"
            class="p-button-rounded p-button-success mr-2"
            @click="editItem(slotProps.data)"
          />
          <Button
            icon="pi pi-trash"
            class="p-button-rounded p-button-danger"
            @click="confirmDeleteItem(slotProps.data)"
          />
        </template>
      </Column>
    </DataTable>

    <AddItemDialog
      :visible="showAddItemDialog"
      @update:visible="showAddItemDialog = $event"
      @item-added="onItemAdded"
    />
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import { useRoute } from "vue-router";

import DataTable from "primevue/datatable";
import Column from "primevue/column";
import Checkbox from "primevue/checkbox";
import Button from "primevue/button";

import AddItemDialog from "../components/AddItemDialog.vue";

const route = useRoute();
const shoppingList = ref({});
const listItems = ref([]);

const showAddItemDialog = ref(false);

const openAddItemDialog = () => {
  showAddItemDialog.value = true;
};
const onItemAdded = async (newItem) => {
  try {
    const response = await fetch(`/api/shopping-lists/${shoppingList.value.id}/items`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(newItem),
    });

    if (!response.ok) {
      throw new Error('Failed to add item');
    }

    const addedItem = await response.json();
    listItems.value.push(addedItem);
  } catch (error) {
    console.error('Error adding item:', error);
  }
};

const editItem = (item) => {
  // TODO: Implement edit item functionality
  console.log("Edit item:", item);
};

const confirmDeleteItem = (item) => {
  // TODO: Implement delete item confirmation and functionality
  console.log("Delete item:", item);
};

onMounted(async () => {
  const listId = route.params.id;

  try {
    // Fetch shopping list data from the backend API
    const listResponse = await fetch(`/api/shopping-lists/${listId}`);
    if (!listResponse.ok) {
      throw new Error("Failed to fetch shopping list data");
    }
    shoppingList.value = await listResponse.json();

    // Fetch shopping list items from the backend API
    const itemsResponse = await fetch(`/api/shopping-lists/${listId}/items`);
    if (!itemsResponse.ok) {
      throw new Error("Failed to fetch shopping list items");
    }
    listItems.value = await itemsResponse.json();
  } catch (error) {
    console.error("Error fetching shopping list data:", error);
  }
});
</script>

<style scoped>
/* Component-specific styles will go here */
</style>
