<template>
  <button 
    class="sidebar-handle"
    :class="[globalStore.isSidebarVisible ? 'active' : '']"
    @click="globalStore.toggleSidebar"
    aria-label="Toggle sidebar"
  >
    <div class="handle-arrow">
      <i :class="globalStore.isSidebarVisible ? 'pi pi-chevron-left' : 'pi pi-chevron-right'"></i>
    </div>
  </button>
</template>

<script setup>
import { useGlobalStore } from "../store/global.ts";
const globalStore = useGlobalStore();
</script>
