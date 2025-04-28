<template>
  <div class="group-page">
    <LoadingState v-if="delayedLoading" message="Loading group information..." />
    <ErrorState
      v-else-if="error"
      :message="error"
      @back="$router.push('/shoppinglists')"
      @retry="fetchGroupData(route.params.id)"
    />

    <div class="page-header">
      <div class="page-title-wrapper">
        <h1 class="page-title">{{ groupInfo.name || "Group" }}</h1>
        <Button
          v-if="canManageGroup"
          icon="pi pi-pencil"
          class="p-button-rounded p-button-text p-button-sm title-edit-btn"
          @click="openEditGroupDialog"
          v-tooltip.bottom="'Edit Group'"
        />
      </div>
      <div class="page-actions">
        <Button
          v-if="canManageGroup"
          label="Add Member"
          icon="pi pi-user-plus"
          class="p-button-rounded add-member-btn"
          @click="openAddMemberDialog"
        />
        <Button
          v-if="canManageGroup"
          icon="pi pi-trash"
          class="p-button-rounded p-button-outlined p-button-danger delete-group-btn"
          @click="confirmDeleteGroup"
          v-tooltip.bottom="'Delete Group'"
        />
      </div>
    </div>

    <div class="group-content">
      <!-- Members Card -->
      <div class="card">
        <div class="card-header">
          <h2 class="card-title">Members</h2>
          <span class="member-count">
            {{ groupMembers.length }}
            {{ groupMembers.length === 1 ? "member" : "members" }}
          </span>
        </div>

        <!-- Members List -->
        <TransitionGroup
          v-if="groupMembers.length"
          name="list"
          tag="ul"
          class="members-list"
        >
          <li
            v-for="member in groupMembers"
            :key="member.userId"
            class="member-item"
          >
            <div class="member-avatar">
              <!--!!! use Avatar component !!!-->
              <Avatar
                :userAvatar="member.avatar"
                :size="40"
                :isOwner="isOwner(member)"
              ></Avatar>
              <div
                v-if="isOwner(member)"
                class="owner-badge"
                v-tooltip.right="'Group Owner'"
              >
                <i class="pi pi-star-fill"></i>
              </div>
            </div>
            <div class="member-info">
              <span class="member-name">{{ member.username }}</span>
              <span class="member-email">{{ member.email }}</span>
            </div>
            <div class="member-actions">
              <Button
                v-if="canManageGroup && !isCurrentUser(member)"
                icon="pi pi-trash"
                class="p-button-rounded p-button-outlined p-button-sm p-button-danger"
                @click="confirmRemoveMember(member)"
                v-tooltip.left="'Remove Member'"
              />
            </div>
          </li>
        </TransitionGroup>

        <!-- Empty State -->
        <div v-else class="empty-state">
          <i class="pi pi-users empty-icon"></i>
          <p>No members in this group yet</p>
          <p class="empty-subtext">
            Add members to collaborate on shopping lists
          </p>
        </div>
      </div>

      <!-- Shopping Lists Card -->
      <div class="card">
        <div class="card-header">
          <h2 class="card-title">Shopping Lists</h2>
          <Button
            v-if="canManageGroup"
            label="Create List"
            icon="pi pi-plus"
            class="p-button-rounded create-list-btn"
            @click="openCreateListDialog"
          />
        </div>

        <!-- Lists -->
        <TransitionGroup
          v-if="groupLists.length > 0"
          name="list"
          tag="ul"
          class="lists-list"
        >
          <li v-for="list in groupLists" :key="list.id" class="list-item">
            <div class="list-content" @click="navigateToList(list)">
              <i class="pi pi-shopping-cart list-icon"></i>
              <div class="list-info">
                <span class="list-name">{{ list.name }}</span>
                <span class="list-meta">{{ list.itemCount || 0 }} items</span>
              </div>
            </div>
            <div class="list-actions">
              <Button
                icon="pi pi-eye"
                class="p-button-rounded p-button-outlined p-button-sm p-button-info"
                @click="navigateToList(list)"
                v-tooltip.left="'View List'"
              />
              <Button
                v-if="canManageGroup"
                icon="pi pi-trash"
                class="p-button-rounded p-button-outlined p-button-sm p-button-danger"
                @click="openDeleteListDialog(list)"
                v-tooltip.left="'Delete List'"
              />
            </div>
          </li>
        </TransitionGroup>

        <!-- Empty State -->
        <div v-else class="empty-state">
          <i class="pi pi-shopping-bag empty-icon"></i>
          <p>No shopping lists in this group yet</p>
          <p class="empty-subtext">Create a list to get started</p>
        </div>
      </div>
    </div>

    <!-- Edit Group Dialog -->
    <Dialog
      v-model:visible="editGroupDialog"
      header="Edit Group"
      :style="{ width: '90vw', maxWidth: '450px' }"
      :modal="true"
    >
      <div class="dialog-content">
        <span class="p-float-label mt-4">
          <InputText
            id="groupName"
            v-model="editedGroupName"
            class="w-full p-inputtext-lg"
            size="lg"
          />
          <label for="groupName">Group Name</label>
        </span>
      </div>
      <template #footer>
        <Button
          label="Cancel"
          icon="pi pi-times"
          class="p-button-text"
          @click="editGroupDialog = false"
        />
        <Button
          label="Save"
          icon="pi pi-check"
          class="p-button-primary"
          @click="saveGroupChanges"
        />
      </template>
    </Dialog>

    <!-- Add Member Dialog -->
    <Dialog
      v-model:visible="addMemberDialog"
      header="Add Member"
      :style="{ width: '90vw', maxWidth: '450px' }"
      :modal="true"
    >
      <div class="dialog-content">
        <span class="p-float-label mt-4">
          <InputText
            id="memberEmail"
            v-model="newMemberEmail"
            class="w-full p-inputtext-lg"
            size="lg"
          />
          <label for="memberEmail">Email Address</label>
        </span>
      </div>
      <template #footer>
        <Button
          label="Cancel"
          icon="pi pi-times"
          class="p-button-text"
          @click="addMemberDialog = false"
        />
        <Button
          label="Add"
          icon="pi pi-plus"
          class="p-button-primary"
          @click="addMember"
        />
      </template>
    </Dialog>

    <!-- Delete Group Confirmation Dialog -->
    <Dialog
      v-model:visible="deleteGroupDialog"
      header="Delete Group"
      :style="{ width: '90vw', maxWidth: '400px' }"
      :modal="true"
    >
      <div class="dialog-content">
        <p>
          Are you sure you want to delete <strong>{{ groupInfo.name }}</strong
          >?
        </p>
        <p class="text-danger">
          This action cannot be undone and will remove all shared shopping
          lists.
        </p>
      </div>
      <template #footer>
        <Button
          label="Cancel"
          icon="pi pi-times"
          class="p-button-text"
          @click="deleteGroupDialog = false"
        />
        <Button
          label="Delete"
          icon="pi pi-trash"
          class="p-button-danger"
          @click="deleteGroup"
        />
      </template>
    </Dialog>

    <!-- Remove Member Confirmation Dialog -->
    <Dialog
      v-model:visible="removeMemberDialog"
      header="Remove Member"
      :style="{ width: '90vw', maxWidth: '400px' }"
      :modal="true"
    >
      <div class="dialog-content">
        <p>
          Are you sure you want to remove
          <strong>{{ memberToRemove?.username }}</strong> from this group?
        </p>
      </div>
      <template #footer>
        <Button
          label="Cancel"
          icon="pi pi-times"
          class="p-button-text"
          @click="removeMemberDialog = false"
        />
        <Button
          label="Remove"
          icon="pi pi-user-minus"
          class="p-button-danger"
          @click="removeMember"
        />
      </template>
    </Dialog>

    <!-- Delete List Confirmation Dialog -->
    <Dialog
      v-model:visible="deleteListDialog"
      header="Delete List"
      :style="{ width: '90vw', maxWidth: '400px' }"
      :modal="true"
    >
      <div class="dialog-content">
        <p>
          Are you sure you want to delete
          <strong>{{ listToDelete?.name }}</strong
          >?
        </p>
      </div>
      <template #footer>
        <Button
          label="Cancel"
          icon="pi pi-times"
          class="p-button-text"
          @click="deleteListDialog = false"
        />
        <Button
          label="Delete"
          icon="pi pi-trash"
          class="p-button-danger"
          @click="deleteList"
        />
      </template>
    </Dialog>

    <!-- Create List Dialog -->
    <Dialog
      v-model:visible="createListDialog"
      header="Create Shopping List"
      :style="{ width: '90vw', maxWidth: '450px' }"
      :modal="true"
    >
      <div class="dialog-content">
        <span class="p-float-label mt-4">
          <InputText
            id="listName"
            v-model="newListName"
            class="w-full p-inputtext-lg"
          />
          <label for="listName">List Name</label>
        </span>
      </div>
      <template #footer>
        <Button
          label="Cancel"
          icon="pi pi-times"
          class="p-button-text"
          @click="createListDialog = false"
        />
        <Button
          label="Create"
          icon="pi pi-plus"
          class="p-button-primary"
          @click="createList"
        />
      </template>
    </Dialog>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from "vue";
import { useRoute, useRouter, onBeforeRouteUpdate } from "vue-router";
import { useAuthStore } from "../store/auth";
import { useGroupsStore } from "../store/groups";
import { useShoppingListsStore } from "../store/shoppingLists";
import { useToast } from "primevue/usetoast";
import { useDelayedLoading } from "../components/composables/useDelayedLoading";

import LoadingState from "../components/common/LoadingState.vue";
import ErrorState from "../components/common/ErrorState.vue";

import Button from "primevue/button";
import InputText from "primevue/inputtext";
import Dialog from "primevue/dialog";
import Tooltip from "primevue/tooltip";
import Avatar from "../components/Avatar.vue";

const route = useRoute();
const router = useRouter();
const authStore = useAuthStore();
const groupStore = useGroupsStore();
const shoppingListsStore = useShoppingListsStore();
const toast = useToast();

const { loading, delayedLoading, setLoading } = useDelayedLoading(500);
const error = ref(null);

// User info
const userId = authStore.authenticatedUser.id;

// Group data
const groupInfo = ref({});
const groupMembers = ref([]);
const groupLists = ref([]);

// Dialog states
const editGroupDialog = ref(false);
const addMemberDialog = ref(false);
const deleteGroupDialog = ref(false);
const removeMemberDialog = ref(false);
const createListDialog = ref(false);
const deleteListDialog = ref(false);

// Form data
const editedGroupName = ref("");
const newMemberEmail = ref("");
const memberToRemove = ref(null);
const newListName = ref("");
const listToDelete = ref(null);

// Computed properties
const canManageGroup = computed(() => {
  // Check if current user is owner or has admin rights
  return groupInfo.value?.createdById === userId || groupInfo.value?.isAdmin;
});

// Utility functions
const isOwner = (member) => {
  console.log(member, groupInfo.value);
  // return member.id === groupInfo.value?.ownerId;
  return member.role === "Admin";
};

const isCurrentUser = (member) => {
  return member.userId === userId;
};

// Dialog handlers
const openEditGroupDialog = () => {
  editedGroupName.value = groupInfo.value.name;
  editGroupDialog.value = true;
};

const openAddMemberDialog = () => {
  newMemberEmail.value = "";
  addMemberDialog.value = true;
};

const confirmDeleteGroup = () => {
  deleteGroupDialog.value = true;
};

const confirmRemoveMember = (member) => {
  memberToRemove.value = member;
  removeMemberDialog.value = true;
};

const openCreateListDialog = () => {
  newListName.value = "";
  createListDialog.value = true;
};

const openDeleteListDialog = (list) => {
  listToDelete.value = list;
  deleteListDialog.value = true;
};

// Action handlers
const saveGroupChanges = async () => {
  if (!editedGroupName.value.trim()) {
    toast.add({
      severity: "warn",
      summary: "Empty Name",
      detail: "Group name cannot be empty",
      life: 3000,
    });
    return;
  }

  try {
    await groupStore.updateGroup(groupInfo.value.id, {
      name: editedGroupName.value,
    });

    // Update local data
    groupInfo.value.name = editedGroupName.value;

    toast.add({
      severity: "success",
      summary: "Group Updated",
      detail: "Group name has been updated",
      life: 2000,
    });

    editGroupDialog.value = false;
  } catch (error) {
    console.error("Error updating group:", error);
    toast.add({
      severity: "error",
      summary: "Update Failed",
      detail: "Could not update group",
      life: 3000,
    });
  }
};

const addMember = async () => {
  if (!newMemberEmail.value.trim()) {
    toast.add({
      severity: "warn",
      summary: "Empty Email",
      detail: "Please enter an email address",
      life: 3000,
    });
    return;
  }

  try {
    // Look up the user by email, which now returns a UserDto
    const response = await fetch(
      `/CollaborativeShoppingListAPI/Users/getByEmail?email=${encodeURIComponent(
        newMemberEmail.value.trim()
      )}`,
      {
        method: "GET",
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      }
    );

    if (!response.ok) {
      // Handle user not found
      if (response.status === 404) {
        toast.add({
          severity: "error",
          summary: "User Not Found",
          detail: "No user exists with this email address",
          life: 3000,
        });
      } else {
        throw new Error("Failed to find user");
      }
      return;
    }

    // Get the user data which includes the ID
    const userData = await response.json();

    // Now add the user to the group using their ID
    console.log("Adding user to group:", groupInfo);
    await groupStore.addMemberToGroup(groupInfo.value.id, userData.id);

    // Success message
    toast.add({
      severity: "success",
      summary: "Member Added",
      detail: `${userData.username} has been added to the group`,
      life: 3000,
    });

    // Clear the input field
    newMemberEmail.value = "";

    // Refresh the member list
    await groupStore.fetchGroupMembers(groupInfo.value.id);
  } catch (error) {
    // Error handling
    console.error("Error adding member:", error);
    toast.add({
      severity: "error",
      summary: "Failed to Add",
      detail: "Could not add member to group",
      life: 3000,
    });
  }
};

const deleteGroup = async () => {
  try {
    await groupStore.deleteGroup(groupInfo.value.id);

    toast.add({
      severity: "info",
      summary: "Group Deleted",
      detail: `${groupInfo.value.name} has been deleted`,
      life: 2000,
    });

    // Navigate back to groups list
    router.push("/groups");
  } catch (error) {
    console.error("Error deleting group:", error);
    toast.add({
      severity: "error",
      summary: "Delete Failed",
      detail: "Could not delete group",
      life: 3000,
    });
    deleteGroupDialog.value = false;
  }
};

const removeMember = async () => {
  if (!memberToRemove.value) return;

  try {
    await groupStore.removeUserFromGroup(
      groupInfo.value.id,
      memberToRemove.value.userId
    );

    // Refresh group data
    await fetchGroupData(groupInfo.value.guid);

    toast.add({
      severity: "info",
      summary: "Member Removed",
      detail: `${memberToRemove.value.username} removed from group`,
      life: 2000,
    });

    removeMemberDialog.value = false;
    memberToRemove.value = null;
  } catch (error) {
    console.error("Error removing member:", error);
    toast.add({
      severity: "error",
      summary: "Remove Failed",
      detail: "Could not remove member from group",
      life: 3000,
    });
  }
};

const createList = async () => {
  if (!newListName.value.trim()) {
    toast.add({
      severity: "warn",
      summary: "Empty Name",
      detail: "List name cannot be empty",
      life: 3000,
    });
    return;
  }

  try {
    const response = await shoppingListsStore.createNewList(
      newListName.value,
      groupInfo.value.id,
      userId
    );

    // Add new list to local data
    groupLists.value.push(response);

    toast.add({
      severity: "success",
      summary: "List Created",
      detail: `${newListName.value} has been created`,
      life: 2000,
    });

    createListDialog.value = false;
    newListName.value = "";
  } catch (error) {
    console.error("Error creating list:", error);
    toast.add({
      severity: "error",
      summary: "Create Failed",
      detail: "Could not create shopping list",
      life: 3000,
    });
  }
};

const deleteList = async () => {
  if (!listToDelete.value) {
    return;
  }

  try {
    await shoppingListsStore.deleteList(listToDelete.value.id);

    // Remove list from local data
    const index = groupLists.value.findIndex(
      (l) => l.id === listToDelete.value.id
    );
    if (index !== -1) {
      groupLists.value.splice(index, 1);
    }

    toast.add({
      severity: "info",
      summary: "List Deleted",
      detail: `${listToDelete.value.name} has been deleted`,
      life: 2000,
    });

    deleteListDialog.value = false;
    listToDelete.value = null;
  } catch (error) {
    console.error("Error deleting list:", error);
    toast.add({
      severity: "error",
      summary: "Delete Failed",
      detail: "Could not delete shopping list",
      life: 3000,
    });
  }
};

const navigateToList = (list) => {
  router.push(`/shoppinglists/${list.guid}`);
};

// Data fetching
const fetchGroupData = async (groupGuid) => {
  setLoading(true);
  error.value = null;

  try {
    // Fetch group info
    const groupInfoResponse = await groupStore.fetchGroupInfoByGuid(groupGuid);
    groupInfo.value = groupInfoResponse;

    // Fetch group members
    const groupMembersResponse = await groupStore.fetchGroupMembers(groupInfoResponse.id);
    groupMembers.value = groupMembersResponse;

    // Fetch group's shopping lists
    const listsResponse = await shoppingListsStore.fetchGroupShoppingLists(
      groupInfoResponse.id
    );
    groupLists.value = listsResponse || [];

    console.log("Group data loaded successfully");
  } catch (error) {
    console.error("Error fetching group data:", error);
    error.value = error.message || "Failed to load group data";
    toast.add({
      severity: "error",
      summary: "Error",
      detail: error.message || "Could not load group data",
      life: 5000,
    });
  } finally {
    setLoading(false);
  }
};

onBeforeRouteUpdate(async (to) => {
  if (to.params.id) {
    await fetchGroupData(to.params.id);
  }
});

onMounted(async () => {
  const groupGuid = route.params.id;
  await fetchGroupData(groupGuid);
});
</script>

<style lang="scss" scoped>
:deep(.p-dialog) {
  background-color: #262626 !important;
  color: #e5e7eb !important;
}

:deep(.p-dialog-header) {
  background-color: #262626 !important;
  color: #e5e7eb !important;
  border-bottom-color: #4b5563 !important;
}

:deep(.p-dialog-footer) {
  background-color: #262626 !important;
  border-top-color: #4b5563 !important;
}

:deep(.p-inputtext) {
  background-color: #1e1e1e !important;
  border-color: #4b5563 !important;
  color: #e5e7eb !important;

  &:focus {
    border-color: #34d399 !important;
    box-shadow: 0 0 0 2px rgba(52, 211, 153, 0.2) !important;
  }
}
</style>
