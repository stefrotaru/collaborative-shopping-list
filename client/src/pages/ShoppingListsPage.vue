<template>
  <div class="shopping-lists-page">
    <h2>My Shopping Lists</h2>
    <div class="mb-4">
      <Button
        label="Create List"
        icon="pi pi-plus"
        @click="openCreateListDialog"
      />
    </div>
    <DataTable :value="shoppingLists" responsiveLayout="scroll">
      <Column field="name" header="Name"></Column>
      <Column field="createdAt" header="Created At">
        <template #body="slotProps">
          {{ formatDate(slotProps.data.createdAt) }}
        </template>
      </Column>
      <Column header="Actions">
        <template #body="slotProps">
          <Button
            icon="pi pi-eye"
            class="p-button-rounded p-button-info mr-2"
            @click="viewList(slotProps.data)"
          />
          <Button
            icon="pi pi-trash"
            class="p-button-rounded p-button-danger"
            @click="confirmDeleteList(slotProps.data)"
          />
        </template>
      </Column>
    </DataTable>

    <CreateListDialog
      :visible="showCreateListDialog"
      @update:visible="showCreateListDialog = $event"
      @list-created="onListCreated"
    />
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import { useAuthStore } from "../store/auth";

import DataTable from "primevue/datatable";
import Column from "primevue/column";
import Button from "primevue/button";

import CreateListDialog from "../components/CreateListDialog.vue";

const authStore = useAuthStore();
const shoppingLists = ref([]);

const showCreateListDialog = ref(false);

const openCreateListDialog = () => {
  showCreateListDialog.value = true;
};
const onListCreated = async (listName) => {
  try {
    const response = await fetch(`/api/users/${authStore.user.id}/shopping-lists`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ name: listName }),
    });

    if (!response.ok) {
      throw new Error('Failed to create shopping list');
    }

    const createdList = await response.json();
    shoppingLists.value.push(createdList);
  } catch (error) {
    console.error('Error creating shopping list:', error);
  }

  //TODO: navigate to the created list
};

const formatDate = (date) => {
  return new Date(date).toLocaleDateString();
};

const viewList = (list) => {
  // Navigate to the shopping list page for the selected list
  router.push(`/shopping-lists/${list.id}`);
};

const confirmDeleteList = (list) => {
  // TODO: Implement delete list confirmation and functionality
  console.log("Delete list:", list);
};

onMounted(async () => {
  try {
    // Fetch shopping lists data from the backend API for the logged-in user
    const response = await fetch(
      `/api/users/${authStore.user.id}/shopping-lists`
    );
    if (!response.ok) {
      throw new Error("Failed to fetch shopping lists");
    }
    shoppingLists.value = await response.json();
  } catch (error) {
    console.error("Error fetching shopping lists:", error);
  }
});
</script>

<style scoped>
/* Component-specific styles will go here */
</style>
