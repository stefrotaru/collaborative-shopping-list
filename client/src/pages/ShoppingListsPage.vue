<template>
  <div class="shopping-lists-page">
    <h1 class="page-title">My Shopping Lists</h1>

    <div class="content-container">
      <!-- Groups Section -->
      <div class="card-container">
        <div class="card-header">
          <h3 class="card-title">All Groups</h3>
          <Button
            label="Create Group"
            icon="pi pi-plus"
            class="p-button-rounded create-button"
            @click="openCreateGroupDialog"
          />
        </div>
        
        <!-- Create Group Input -->
        <div v-if="showCreateGroupDialog" class="create-input-container">
          <div class="create-input-wrapper">
            <i class="pi pi-users input-icon"></i>
            <InputText
              id="group-name"
              v-model="newGroupName"
              placeholder="Enter group name"
              class="create-input no-highlight"
              @keyup.enter="createGroup"
            />
            <div class="create-input-actions">
              <Button
                icon="pi pi-check"
                class="p-button-rounded p-button-sm p-button-success action-btn"
                @click="createGroup"
              />
              <Button
                icon="pi pi-times"
                class="p-button-rounded p-button-sm p-button-danger action-btn"
                @click="showCreateGroupDialog = false"
              />
            </div>
          </div>
        </div>
        
        <!-- Groups List -->
        <TransitionGroup name="list" tag="ul" class="item-list" v-if="userGroups.length > 0">
          <li v-for="group in userGroups" :key="group.id" class="list-item-container">
            <div class="list-item">
              <div class="item-content" @click="$router.push('/groups/' + group.guid)">
                <i class="pi pi-users item-icon"></i>
                <div class="item-name">{{ group.name }}</div>
              </div>
              <div class="item-actions">
                <Button
                  icon="pi pi-cog"
                  class="p-button-rounded p-button-outlined p-button-sm p-button-info action-button"
                  @click="$router.push('/groups/' + group.guid)"
                />
              </div>
            </div>
          </li>
        </TransitionGroup>

        <!-- Empty State for Groups -->
        <div v-else class="empty-list">
          <i class="pi pi-users empty-icon"></i>
          <p>You don't have any groups yet</p>
          <p class="empty-subtext">Create a group to share shopping lists with others</p>
        </div>
      </div>

      <!-- Shopping Lists Section -->
      <div class="card-container">
        <div class="card-header">
          <h3 class="card-title">My Shopping Lists</h3>
          <Button
            label="Create List"
            icon="pi pi-plus"
            class="p-button-rounded create-button"
            @click="openCreateListDialog"
          />
        </div>

        <LoadingState v-if="accessibleListsStore.isLoading" message="Loading your shopping lists..." />
        <ErrorState v-else-if="accessibleListsStore.error" :message="accessibleListsStore.error" :showBackButton="false" @retry="fetchData" />

        <!-- Shopping Lists -->
        <template v-else>
          <!-- Owned Lists -->
          <div v-if="ownedLists.length > 0" class="list-section">
            <h4 class="section-title">Lists I Own</h4>
            <TransitionGroup name="list" tag="ul" class="item-list">
              <li v-for="list in ownedLists" :key="list.shoppingListId" class="list-item-container">
                <div class="list-item">
                  <div class="item-content" @click="viewList(list)">
                    <i class="pi pi-shopping-cart item-icon"></i>
                    <div class="item-details">
                      <div class="item-name">{{ list.shoppingListName }}</div>
                      <div class="item-meta">
                        Group: {{ list.groupName || 'Personal' }}
                        <span class="badge owner-badge">Owner</span>
                      </div>
                    </div>
                  </div>
                  <div class="item-actions">
                    <Button
                      icon="pi pi-eye"
                      class="p-button-rounded p-button-outlined p-button-sm p-button-info action-button"
                      @click="viewList(list)"
                    />
                    <Button
                      icon="pi pi-trash"
                      class="p-button-rounded p-button-outlined p-button-sm p-button-danger action-button"
                      @click="confirmDeleteList(list)"
                    />
                  </div>
                </div>
              </li>
            </TransitionGroup>
          </div>

          <!-- Shared Lists -->
          <div v-if="sharedLists.length > 0" class="list-section">
            <h4 class="section-title">Shared With Me</h4>
            <TransitionGroup name="list" tag="ul" class="item-list">
              <li v-for="list in sharedLists" :key="list.shoppingListId" class="list-item-container">
                <div class="list-item">
                  <div class="item-content" @click="viewList(list)">
                    <i class="pi pi-shopping-cart item-icon"></i>
                    <div class="item-details">
                      <div class="item-name">{{ list.shoppingListName }}</div>
                      <div class="item-meta">
                        {{ list.accessReason }}
                        <span class="badge shared-badge">Shared</span>
                      </div>
                    </div>
                  </div>
                  <div class="item-actions">
                    <Button
                      icon="pi pi-eye"
                      class="p-button-rounded p-button-outlined p-button-sm p-button-info action-button"
                      @click="viewList(list)"
                    />
                  </div>
                </div>
              </li>
            </TransitionGroup>
          </div>

          <!-- Empty State for Lists -->
          <div v-if="!ownedLists.length && !sharedLists.length" class="empty-list">
            <i class="pi pi-shopping-bag empty-icon"></i>
            <p>You don't have any shopping lists yet</p>
            <p class="empty-subtext">Create a shopping list to get started</p>
          </div>
        </template>
      </div>
    </div>

    <!-- Delete Confirmation Dialog -->
    <Dialog v-model:visible="deleteDialog" header="Confirm Delete" :style="{width: '90vw', maxWidth: '400px'}" :modal="true">
      <p>Are you sure you want to delete <strong>{{ listToDelete?.shoppingListName }}</strong>?</p>
      <template #footer>
        <Button label="Cancel" icon="pi pi-times" class="p-button-text" @click="deleteDialog = false" />
        <Button label="Delete" icon="pi pi-trash" class="p-button-danger" @click="confirmDelete" />
      </template>
    </Dialog>

    <!-- Create List Dialog -->
    <ListFormDialog
      :visible="showCreateListDialog"
      @update:visible="showCreateListDialog = $event"
      :groups="userGroups"
      @submit="onListCreated"
    />
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from "vue";
import { useRouter } from "vue-router";
import { useAuthStore } from "../store/auth";
import { useShoppingListsStore } from "../store/shoppingLists";
import { useAccessibleShoppingListsStore } from "../store/accessibleShoppingLists";
import { useToast } from "primevue/usetoast";
import Button from "primevue/button";
import InputText from "primevue/inputtext";
import Dialog from "primevue/dialog";
import ListFormDialog from "../components/ListFormDialog.vue";

const router = useRouter();
const authStore = useAuthStore();
const shoppingListsStore = useShoppingListsStore();
const accessibleListsStore = useAccessibleShoppingListsStore();
const toast = useToast();

import LoadingState from "../components/common/LoadingState.vue";
import ErrorState from "../components/common/ErrorState.vue";

const userGroups = ref([]);
const showCreateListDialog = ref(false);
const showCreateGroupDialog = ref(false);
const newGroupName = ref("");
const deleteDialog = ref(false);
const listToDelete = ref(null);

// Computed properties from the accessible lists store
const ownedLists = computed(() => accessibleListsStore.getOwnedLists());
const sharedLists = computed(() => accessibleListsStore.getSharedLists());

const openCreateGroupDialog = () => {
  showCreateGroupDialog.value = true;
};

const createGroup = async () => {
  if (!newGroupName.value.trim()) {
    toast.add({
      severity: 'warn',
      summary: 'Empty Name',
      detail: 'Please enter a group name',
      life: 3000
    });
    return;
  }

  try {
    await shoppingListsStore.createGroup(
      newGroupName.value, 
      'description', 
      authStore.authenticatedUser.id
    );
    
    toast.add({
      severity: 'success',
      summary: 'Group Created',
      detail: `Group "${newGroupName.value}" has been created`,
      life: 2000
    });
    
    newGroupName.value = "";
    showCreateGroupDialog.value = false;
    fetchData();
  } catch (error) {
    toast.add({
      severity: 'error',
      summary: 'Error',
      detail: 'Could not create group',
      life: 3000
    });
    console.error("Error creating group:", error);
  }
};

const openCreateListDialog = () => {
  showCreateListDialog.value = true;
};

const onListCreated = async (list) => {
  try {
    // Create list with your existing method
    const response = await shoppingListsStore.createNewList(
      list.name,
      list.groupId, // GroupId 0 for personal lists, or let user select a group
      authStore.authenticatedUser.id
    );
    
    toast.add({
      severity: 'success',
      summary: 'List Created',
      detail: `List "${list.name}" has been created`,
      life: 2000
    });
    
    // Refresh data to show new list
    fetchData();
  } catch (error) {
    toast.add({
      severity: 'error',
      summary: 'Error',
      detail: 'Could not create shopping list',
      life: 3000
    });
    console.error("Error creating shopping list:", error);
  }
};

const viewList = (list) => {
  router.push(`/shoppinglists/${list.shoppingListGuid}`);
};

const confirmDeleteList = (list) => {
  listToDelete.value = list;
  deleteDialog.value = true;
};

const confirmDelete = async () => {
  if (!listToDelete.value) return;
  
  try {
    await shoppingListsStore.deleteList(listToDelete.value.shoppingListId);
    
    toast.add({
      severity: 'info',
      summary: 'List Deleted',
      detail: `${listToDelete.value.shoppingListName} has been deleted`,
      life: 2000
    });
    
    // Refresh lists after deletion
    fetchData();
  } catch (error) {
    toast.add({
      severity: 'error',
      summary: 'Error',
      detail: 'Could not delete shopping list',
      life: 3000
    });
    console.error("Error deleting list:", error);
  } finally {
    deleteDialog.value = false;
    listToDelete.value = null;
  }
};

const fetchData = async () => {
  if (authStore.authenticatedUser) {
    try {
      // Fetch groups data
      const groupsData = await authStore.getUserGroups(true);
      userGroups.value = groupsData;
      
      // Fetch accessible shopping lists using the new store
      await accessibleListsStore.fetchAccessibleLists();
    } catch (error) {
      console.error("Error fetching data:", error);
      toast.add({
        severity: 'error',
        summary: 'Error',
        detail: 'Could not load your data',
        life: 3000
      });
    }
  }
};

onMounted(async () => {
  fetchData();
});
</script>

<style scoped>
/* Your existing styles */
.section-title {
  font-size: 1.2rem;
  margin-bottom: 0.75rem;
  color: #d1d5db;
  border-bottom: 1px solid #4b5563;
  padding-bottom: 0.5rem;
}

.list-section {
  margin-bottom: 2rem;
}

.badge {
  font-size: 0.7rem;
  padding: 0.1rem 0.4rem;
  border-radius: 4px;
  margin-left: 0.5rem;
}

.owner-badge {
  background-color: #10b981;
  color: white;
}

.shared-badge {
  background-color: #3b82f6;
  color: white;
}

.loading-state, .error-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 2rem;
  color: #9ca3af;
  text-align: center;
}
</style>