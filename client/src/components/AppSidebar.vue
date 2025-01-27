<template>
  <SidebarHandle />
  <aside class="app-sidebar" v-show="globalStore.isSidebarVisible" >
    <UserProfile />
    <MyShoppingLists />
    <GroupList />
  </aside>
</template>

<script setup>
import UserProfile from "./UserProfile.vue";
import MyShoppingLists from "./MyShoppingLists.vue";
import GroupList from "./MyGroupsList.vue";

import SidebarHandle from "./AppSidebarHandle.vue";

import { useGlobalStore } from "../store/global.ts";

const globalStore = useGlobalStore();
</script>
