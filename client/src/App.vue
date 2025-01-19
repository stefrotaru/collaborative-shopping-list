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
import { onMounted } from "vue";
import { useAuthStore } from "./store/auth";
import { useShoppingListsStore } from "./store/shoppingLists";

const authStore = useAuthStore();
const shoppingListsStore = useShoppingListsStore();

onMounted(() => {
  console.log("App.vue mounted");
  authStore.checkAuth();
  authStore.populateStore();
  shoppingListsStore.populateStore();
});
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
