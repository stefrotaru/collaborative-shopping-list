<template>
  <div class="shopping-lists-page">
    <h1>My Shopping Lists</h1>

    <div class="user-groups__container">
      <div class="user-groups__list">
        <div class="user-groups__list-header">
          <h3>My Groups</h3>
          <Button
            label="Create Group"
            icon="pi pi-plus"
            @click="openCreateGroupDialog"
          />
        </div>
        <div class="user-groups__list__create-group">
          <InputText
            v-if="showCreateGroupDialog"
            id="group-name"
            v-model="newGroupName"
            @keyup.enter="createGroup"
          />
        </div>
        <ul>
          <li
            v-for="group in userGroups"
            :key="group.id"
          >
            <div class="user-groups__list__item">
              <div class="user-groups__list__item-name">{{ group.name }}</div>
              <div class="user-groups__list__item-actions">
                <router-link :to="'/groups/' + group.id">Manage</router-link>
              </div>
            </div>
          </li>
        </ul>
      </div>
    </div>

    <div class="shopping-lists__container">
      <div class="shopping-lists__list">
        <div class="shopping-lists__list-header">
          <h3>My Shopping Lists</h3>
          <Button
            label="Create List"
            icon="pi pi-plus"
            @click="openCreateListDialog"
          />
        </div>
        <ul>
          <li v-for="list in shoppingLists" :key="list.id">
            <div class="shopping-lists__list__item">
              <div class="shopping-lists__list__item-name">{{ list.name }}</div>
              <div class="shopping-lists__list__item-actions">
                <Button
                  icon="pi pi-eye"
                  class="p-button-rounded p-button-info mr-2"
                  @click="viewList(list)"
                />
                <Button
                  icon="pi pi-trash"
                  class="p-button-rounded p-button-danger"
                  @click="confirmDeleteList(list)"
                />
              </div>
            </div>
          </li>
        </ul>
      </div>
    </div>

    <CreateListDialog
      :visible="showCreateListDialog"
      @update:visible="showCreateListDialog = $event"
      @list-created="onListCreated"
    />
  </div>
</template>

<script setup>
import { ref, onMounted, watch } from "vue";
import { useRouter } from "vue-router";
import { useAuthStore } from "../store/auth";
import Button from "primevue/button";
import CreateListDialog from "../components/CreateListDialog.vue";
import { useShoppingListsStore } from "../store/shoppingLists";

import InputText from "primevue/inputtext";

const router = useRouter();
const authStore = useAuthStore();
const shoppingListsStore = useShoppingListsStore();

const userGroups = ref([]);
const shoppingLists = ref([]);
const showCreateListDialog = ref(false);

const showCreateGroupDialog = ref(false);
const newGroupName = ref("");

const openCreateGroupDialog = () => {
  showCreateGroupDialog.value = true;
};
const createGroup = () => {
  shoppingListsStore.createGroup(newGroupName.value, 'description', authStore.authenticatedUser.id);
  newGroupName.value = "";
  showCreateGroupDialog.value = false;

  // fetch data again to get the updated list of groups
  setTimeout(() => {
    fetchData();
  }, 1000);
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
  } catch (error) {
    // console.error("Error creating shopping list:", error);
  }

  // fetch data again to get the updated list of groups
  setTimeout(() => {
    fetchData();
  }, 1000);

  //TODO: navigate to the created list
};

const formatDate = (date) => {
  return new Date(date).toLocaleDateString();
};

const viewList = (list) => {
  // Navigate to the shopping list page for the selected list
  router.push(`/shoppinglists/${list.id}`);
};

const confirmDeleteList = (list) => {
  // TODO: Implement delete list confirmation and functionality
  console.log("Delete list:", list);
  shoppingListsStore.deleteList(list.id);

  // fetch data again to get the updated list of groups
  setTimeout(() => {
    fetchData();
  }, 1000);
};

const fetchData = async () => {
  //TODO: first I should get all the groups and then fetch the shopping lists for all groups
  if (authStore.authenticatedUser) {
    try {
      const [groupsData, listsData] = await Promise.all([
        authStore.getUserGroups(),
        shoppingListsStore.fetchGroupShoppingLists(authStore.userGroups[0].id), //TODO: fetch shopping lists for all groups
      ]);

      userGroups.value = groupsData;
      shoppingLists.value = listsData;

      console.log("userGroups", userGroups.value);
      console.log("shoppingLists", shoppingLists.value);
    } catch (error) {
      console.error("Error fetching data:", error);
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

<style lang="scss" scoped>
.shopping-lists-page {
  text-align: center;
}

.shopping-lists,
.user-groups {
  &__container {
    max-width: 800px;
    margin: 0 auto;
  }

  &__list {
    margin-top: 1rem;
    border: 1px solid #ccc;
    border-radius: 5px;
    padding: 1rem;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);

    &-header {
      font-size: 1.5rem;
      font-weight: bold;
      margin-bottom: 1rem;

      display: flex;
      justify-content: center;
      gap: 1rem;

      h3 {
        margin: 0;
      }
    }

    &__item {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 0.5rem 1rem;
      border-bottom: 1px solid #ccc;

      &:last-child {
        border-bottom: none;
      }

      &-name {
        font-size: 1.2rem;
      }

      &-actions {
        display: flex;
        gap: 0.5rem;
      }
    }
  }
}
</style>
