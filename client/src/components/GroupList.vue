<template>
  <div class="group-list">
    <h3>My Groups</h3>
    <ul>
      <li v-for="group in groups" :key="group.id">
        <GroupItem :group="group" />
      </li>
    </ul>
  </div>
</template>

<script setup>
import { ref } from "vue";
import GroupItem from "./GroupItem.vue";

const groups = ref([
  { id: 1, name: "Family" },
  { id: 2, name: "Friends" },
  { id: 3, name: "Work" },
]);
</script>

<style scoped>
.group-list {
  margin-top: 1rem;
}

ul {
  list-style: none;
  padding: 0;
}
</style>
