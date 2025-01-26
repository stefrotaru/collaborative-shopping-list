<template>
  <Button
    :icon="
      globalStore.isSidebarVisible ? 'pi pi-angle-left' : 'pi pi-angle-right'
    "
    class="sidebar-handle"
    :class="[globalStore.isSidebarVisible ? 'active' : '']"
    @click="globalStore.toggleSidebar"
  ></Button>
</template>

<script setup>
import Button from "primevue/button";

import { useGlobalStore } from "../store/global.ts";
const globalStore = useGlobalStore();
</script>

<style lang="scss" scoped>
.sidebar-handle {
  height: 75px;
  width: 25px;
  padding-left: 5px;

  position: absolute;
  top: calc(50% - 76px); //76px is the height of the navbar
  left: -5px;

  &.active {
    left: 245px;

    @media screen and (max-width: 768px) {
      left: 145px;
    }
  }

}
</style>
