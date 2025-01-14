<template>
  <SidebarHandle />
  <aside class="app-sidebar" v-if="globalStore.isSidebarVisible">
    <UserProfile />
    <MyShoppingLists />
    <GroupList />

    <!-- <Button
      rounded
      raised
      icon="pi pi-angle-left"
      aria-label="Close Sidebar"
      variant="outlined"
      class="app-sidebar__close-btn"
      @click="globalStore.toggleSidebar()"
    /> -->
  </aside>
</template>

<script setup>
import UserProfile from "./UserProfile.vue";
import MyShoppingLists from "./MyShoppingLists.vue";
import GroupList from "./GroupList.vue";
// import Button from "primevue/button";

import SidebarHandle from "./AppSidebarHandle.vue";

import { useGlobalStore } from "../store/global.js";

const globalStore = useGlobalStore();
</script>

<style lang="scss" scoped>
.app-sidebar {
  width: 250px;
  position: relative;
  /* background-color: #ffffff; */
  background-color: #18181b;
  padding: 1rem;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  color: #e4e4e7;

  // &__close-btn {
  //   position: absolute;
  //   top: 17.5px;
  //   right: -20px;
  // }

  @media screen and (max-width: 768px) {
    width: 150px;
  }
}
</style>
