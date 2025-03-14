<template>
  <div class="sidebar-wrapper" :class="{ 'sidebar-expanded': globalStore.isSidebarVisible }">
    <SidebarHandle />
    <aside class="app-sidebar" v-show="globalStore.isSidebarVisible">
      <div class="sidebar-content">
        <UserProfile />
        
        <div class="sidebar-divider"></div>
        
        <div class="sidebar-section">
          <MyShoppingLists />
        </div>
        
        <div class="sidebar-section">
          <MyGroupsList />
        </div>
      </div>
    </aside>
  </div>
</template>

<script setup>
import UserProfile from "./UserProfile.vue";
import MyShoppingLists from "./MyShoppingLists.vue";
import MyGroupsList from "./MyGroupsList.vue";
import SidebarHandle from "./AppSidebarHandle.vue";

import { useGlobalStore } from "../store/global.ts";

const globalStore = useGlobalStore();
</script>
