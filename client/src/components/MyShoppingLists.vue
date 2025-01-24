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

import { useShoppingListsStore } from "../store/shoppingLists";
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
</script>

<style lang="scss" scoped>
// duplicate from GroupItem.vue
.group-item {
  padding: 0.8rem 0.5rem;
  cursor: pointer;
  display: block;
}

.group-item:hover {
  background-color: rgba(52, 211, 153, 0.5);
  border-radius: 0.2rem;

  a {
    color: #fff;
  }
}
//---

ul {
  list-style: none;
  padding: 0;
}
</style>
