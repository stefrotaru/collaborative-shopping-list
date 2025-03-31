<template>
  <nav class="app-navbar">
    <div class="app-navbar__logo">
      <span @click="$router.push('/home')">
        <img src="../assets/linklist1.svg" alt="Logo" height="60"/>
        Link-List
      </span>
    </div>
    <div class="app-navbar__user-profile">
      <Avatar @click="$router.push('/profile')"/>
    </div>
  </nav>
</template>

<script setup>
import Avatar from "./Avatar.vue";
import { useGlobalStore } from "../store/global.ts";

const globalStore = useGlobalStore();
</script>
