<template>
  <div class="group-list" v-if="groups.length > 0">
    <h3>My Groups</h3>
    <ul>
      <li v-for="group in groups" :key="group.id">
        <GroupItem :group="group" />
      </li>
    </ul>
  </div>
</template>

<script setup>
import { ref, watch } from "vue";
import GroupItem from "./GroupItem.vue";

import { useAuthStore } from "../store/auth";
const authStore = useAuthStore();

const groups = ref(authStore.userGroups);

// onMounted(() => {
//   authStore.fetchUserGroups();
// });
watch(authStore, (newValue) => {
  groups.value = newValue.userGroups;
});
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
