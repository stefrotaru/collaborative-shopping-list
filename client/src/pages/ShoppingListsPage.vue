<template>
  <div class="shopping-lists-page">
    <h1 class="page-title">My Shopping Lists</h1>

    <div class="content-container">
      <!-- Groups Section -->
      <div class="card-container">
        <!-- <div class="card"> -->
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
                class="create-input"
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
                <div class="item-content" @click="$router.push('/groups/' + group.id)">
                  <i class="pi pi-users item-icon"></i>
                  <div class="item-name">{{ group.name }}</div>
                </div>
                <div class="item-actions">
                  <Button
                    icon="pi pi-cog"
                    class="p-button-rounded p-button-outlined p-button-sm p-button-info action-button"
                    @click="$router.push('/groups/' + group.id)"
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
        <!-- </div> -->
      </div>

      <!-- Shopping Lists Section -->
      <div class="card-container">
        <!-- <div class="card"> -->
          <div class="card-header">
            <h3 class="card-title">All Shopping Lists</h3>
            <Button
              label="Create List"
              icon="pi pi-plus"
              class="p-button-rounded create-button"
              @click="openCreateListDialog"
            />
          </div>
          
          <!-- Shopping Lists -->
          <TransitionGroup name="list" tag="ul" class="item-list" v-if="shoppingLists.length > 0">
            <li v-for="list in shoppingLists" :key="list.id" class="list-item-container">
              <div class="list-item">
                <div class="item-content" @click="viewList(list)">
                  <i class="pi pi-shopping-cart item-icon"></i>
                  <div class="item-details">
                    <div class="item-name">{{ list.name }}</div>
                    <div class="item-meta">Group: {{ getGroupName(list.groupId) }}</div>
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

          <!-- Empty State for Lists -->
          <div v-else class="empty-list">
            <i class="pi pi-shopping-bag empty-icon"></i>
            <p>You don't have any shopping lists yet</p>
            <p class="empty-subtext">Create a shopping list to get started</p>
          </div>
        <!-- </div> -->
      </div>
    </div>

    <!-- Delete Confirmation Dialog -->
    <Dialog v-model:visible="deleteDialog" header="Confirm Delete" :style="{width: '90vw', maxWidth: '400px'}" :modal="true">
      <p>Are you sure you want to delete <strong>{{ listToDelete?.name }}</strong>?</p>
      <template #footer>
        <Button label="Cancel" icon="pi pi-times" class="p-button-text" @click="deleteDialog = false" />
        <Button label="Delete" icon="pi pi-trash" class="p-button-danger" @click="confirmDelete" />
      </template>
    </Dialog>

    <!-- Create List Dialog -->
    <CreateListDialog
      :visible="showCreateListDialog"
      @update:visible="showCreateListDialog = $event"
      @list-created="onListCreated"
    />
  </div>
</template>

<script setup>
import { ref, onMounted, watch, computed } from "vue";
import { useRouter } from "vue-router";
import { useAuthStore } from "../store/auth";
import { useShoppingListsStore } from "../store/shoppingLists";
import { useToast } from "primevue/usetoast";
import Button from "primevue/button";
import InputText from "primevue/inputtext";
import Dialog from "primevue/dialog";
import CreateListDialog from "../components/CreateListDialog.vue";
// import './ImprovedShoppingListsPage.scss';

const router = useRouter();
const authStore = useAuthStore();
const shoppingListsStore = useShoppingListsStore();
const toast = useToast();

const userGroups = ref([]);
const shoppingLists = ref([]);
const showCreateListDialog = ref(false);
const showCreateGroupDialog = ref(false);
const newGroupName = ref("");
const deleteDialog = ref(false);
const listToDelete = ref(null);

// Get group name by ID
const getGroupName = (groupId) => {
  const group = userGroups.value.find(g => g.id === groupId);
  return group ? group.name : 'Personal';
};

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

const onListCreated = async (listName) => {
  try {
    const response = await fetch(
      `/api/users/${authStore.user.id}/shopping-lists`,
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ name: listName }),
      }
    );

    if (!response.ok) {
      throw new Error("Failed to create shopping list");
    }

    const createdList = await response.json();
    shoppingLists.value.push(createdList);
    
    toast.add({
      severity: 'success',
      summary: 'List Created',
      detail: `List "${listName}" has been created`,
      life: 2000
    });
    
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
  router.push(`/shoppinglists/${list.id}`);
};

const confirmDeleteList = (list) => {
  listToDelete.value = list;
  deleteDialog.value = true;
};

const confirmDelete = async () => {
  if (!listToDelete.value) return;
  
  try {
    await shoppingListsStore.deleteList(listToDelete.value.id);
    
    toast.add({
      severity: 'info',
      summary: 'List Deleted',
      detail: `${listToDelete.value.name} has been deleted`,
      life: 2000
    });
    
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
      const groupsData = await authStore.getUserGroups();
      const userGroupArr = groupsData.map(group => group.id);
      const listsData = await shoppingListsStore.fetchAllGroupsShoppingLists(userGroupArr);

      userGroups.value = groupsData;
      shoppingLists.value = listsData;
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

// Watch if the user's shopping lists have changed
watch(
  () => shoppingListsStore.userShoppingLists,
  (newVal) => {
    shoppingLists.value = newVal || [];
  },
  { immediate: true }
);
</script>
