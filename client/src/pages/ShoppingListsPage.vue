<template>
  <div class="shopping-lists-page">
    <h1>My Shopping Lists</h1>
    <!-- <DataTable :value="shoppingLists" responsiveLayout="scroll">
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
    </DataTable> -->

    <div class="user-groups__container">
      <div class="user-groups-header">
        <h3>My Groups</h3>
      </div>
      <div class="user-groups__list">
        <div
          class="user-groups__list-item"
          v-for="group in userGroups"
          :key="group.id"
        >
          <div class="user-groups__list-item-name">{{ group.name }}</div>
          <div class="user-groups__list-item-actions">
            <router-link :to="'/groups/' + group.id">Manage</router-link>
          </div>
        </div>
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
import { useRouter } from 'vue-router';
import { useAuthStore } from "../store/auth";
import Button from "primevue/button";
import CreateListDialog from "../components/CreateListDialog.vue";
import { useShoppingListsStore } from "../store/shoppingLists";

const router = useRouter();
const authStore = useAuthStore();
const shoppingListsStore = useShoppingListsStore();

const userGroups = ref([]);
const shoppingLists = ref([]);
const showCreateListDialog = ref(false);

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
    console.error("Error creating shopping list:", error);
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

const fetchData = async () => {
  if (authStore.authenticatedUser) {
    try {
      const [groupsData, listsData] = await Promise.all([
        authStore.getUserGroups(),
        shoppingListsStore.fetchGroupShoppingLists(3)
      ]);
      
      userGroups.value = groupsData;
      shoppingLists.value = listsData;
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

.user-groups {
  &__container {
    max-width: 800px;
    margin: 0 auto;
  }

  &-header {
    font-size: 1.5rem;
    font-weight: bold;
    margin-bottom: 1rem;
  }

  &__list {
    margin-top: 1rem;
    border: 1px solid #ccc;
    border-radius: 5px;
    padding: 1rem;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);

    display: grid;
    grid-gap: 10px;
    grid-template-columns: repeat(auto-fill, 32%);

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

.shopping-lists {
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
