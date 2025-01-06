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
import { useGlobalStore } from "../store/global.js";

const globalStore = useGlobalStore();
</script>

<style lang="scss" scoped>
.app-navbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  /* background-color: #f8f9fa; */
  background-color: #18181b;

  &__logo {
    cursor: pointer;
    
    img {
      height: 40px;
    }
  }

  &__user-profile {
    cursor: pointer;
  }
}
</style>
