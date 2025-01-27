<template>
  <nav class="app-navbar">
    <div class="app-navbar__logo">
      <img src="../assets/vue.svg" alt="Logo" @click="$router.push('/home')"/>
    </div>
    <div class="app-navbar__user-profile">
      <Avatar @click="globalStore.toggleSidebar()"/>
    </div>
  </nav>
</template>

<script setup>
import Avatar from "./Avatar.vue";
import { useGlobalStore } from "../store/global.ts";

const globalStore = useGlobalStore();
</script>
