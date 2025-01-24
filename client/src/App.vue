<template>
  <div id="app">
    <AppNavbar />
    <div class="main-content">
      <AppSidebar />
      <div class="page-content">
        <router-view></router-view>
      </div>
    </div>
  </div>
  <Toast />
</template>

<script setup lang="ts">
import AppNavbar from "./components/AppNavbar.vue";
import AppSidebar from "./components/AppSidebar.vue";
import Toast from "primevue/toast";

import { useAuthStore } from "./store/auth";
import { useShoppingListsStore } from "./store/shoppingLists";

const authStore = useAuthStore();
const shoppingListsStore = useShoppingListsStore();

const init = async () => {
  console.log("✅ App.vue mounted and initialised stores");
  await authStore.populateStore();
  await authStore.checkAuth();

  //Todo GetByGroups: use GetByGroups which will accept an array of group id's 
  let groupId = 4;
  await shoppingListsStore.populateStore(groupId);
};

init();
</script>

<style lang="scss" scoped>
// .logo {
//   height: 6em;
//   padding: 1.5em;
//   will-change: filter;
//   transition: filter 300ms;
// }
// .logo {
//   &:hover {
//     filter: drop-shadow(0 0 2em #646cffaa);
//   }
//   &.vue:hover {
//     filter: drop-shadow(0 0 2em #42b883aa);
//   }
// }
</style>
