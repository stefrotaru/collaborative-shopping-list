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

<script setup lang="ts">
import { onMounted, onUnmounted } from 'vue';
import { useToast } from 'primevue/usetoast';
import AppNavbar from "./components/AppNavbar.vue";
import AppSidebar from "./components/AppSidebar.vue";
import Toast from "primevue/toast";
import AppFooter from "./components/AppFooter.vue";

import { useAuthStore } from "./store/auth";
import { useShoppingListsStore } from "./store/shoppingLists";
import { useGroupsStore } from "./store/groups";
import signalRService from './services/signalRService.ts';
import { initializeNotificationService, shutdownNotificationService } from './services/notificationService.ts';

const toast = useToast();
const authStore = useAuthStore();
const shoppingListsStore = useShoppingListsStore();
const groupsStore = useGroupsStore();

const init = async () => {
  console.log("✅ App.vue mounted and initializing stores");
  await authStore.checkAuth();

  if (authStore.authenticatedUser) {
    // Start SignalR connection
    await signalRService.start(authStore.authenticatedUser.token);
    
    // Initialize the notification service (sets up all the listeners)
    await initializeNotificationService(toast, authStore, shoppingListsStore, groupsStore);
    
    // Fetch groups and lists for the app
    const groups = await authStore.getUserGroups(true);
    if (groups?.length) {
      const groupIds = groups.map((group) => group.id);
      await shoppingListsStore.fetchAllGroupsShoppingLists(groupIds);
    }
    
    console.log("✅ App initialization complete");
  }
};

onMounted(() => {
  init();
});

onUnmounted(async () => {
  // Shutdown the notification service when the app is unmounted
  await shutdownNotificationService();
});
</script>