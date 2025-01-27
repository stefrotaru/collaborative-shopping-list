<template>
  <div class="shopping-lists-list">
    <div>
      <h3>My Shopping Lists</h3>
    </div>
    <ul>
      <li>
        <router-link class="group-item" to="/shoppinglists">Show all</router-link>
      </li>
      <li v-for="shoppingList in shoppingLists" :key="shoppingLists.id">
        <router-link
          class="group-item"
          :to="'/shoppinglists/' + shoppingList.id"
          >{{ shoppingList.name }}</router-link
        >
      </li>
    </ul>
  </div>
</template>

<script setup>
import { ref, watch } from "vue";
import GroupItem from "./GroupItem.vue";
// import { Button } from "primevue";

import { useAuthStore } from "../store/auth";
import { useShoppingListsStore } from "../store/shoppingLists";

const authStore = useAuthStore();
const shoppingListsStore = useShoppingListsStore();
const shoppingLists = ref(shoppingListsStore.userShoppingLists);

watch(
  () => shoppingListsStore.userShoppingLists,
  (newVal) => {
    if (newVal) {
      shoppingLists.value = newVal;
    } else {
      shoppingLists.value = [];
    }
  },
  // { immediate: true }
);

const fetchShoppingLists = async () => {
  if (authStore.userGroups?.length > 0) {
    const groupIds = authStore.userGroups.map(group => group.id);
    await shoppingListsStore.fetchAllGroupsShoppingLists(groupIds);
  }
};

fetchShoppingLists();
</script>
