<template>
  <div id="app">
    <AppNavbar />
    <div class="main-container">
      <AppSidebar />
      <main class="page-content">
        <router-view></router-view>
      </main>
    </div>
    <AppFooter />
  </div>
  <Toast />
</template>

<script setup>
import AppNavbar from "./components/AppNavbar.vue";
import AppSidebar from "./components/AppSidebar.vue";
import Toast from "primevue/toast";

import { useAuthStore } from "./store/auth";
import { useShoppingListsStore } from "./store/shoppingLists";
import AppFooter from "./components/AppFooter.vue";

const authStore = useAuthStore();
const shoppingListsStore = useShoppingListsStore();

const init = async () => {
  console.log("✅ App.vue mounted and initialised stores");
  await authStore.checkAuth();

  if (authStore.authenticatedUser) {
    const groups = await authStore.getUserGroups();

    if (groups?.length) {
      const groupIds = groups.map((group) => group.id);
      await shoppingListsStore.fetchAllGroupsShoppingLists(groupIds);
    }
  }
};

init();
</script>
