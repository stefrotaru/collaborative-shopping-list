<template>
  <div class="group-item">
    <router-link :to="'/groups/' + group.id">{{ group.name }}</router-link>
  </div>
</template>

<script setup>
import { defineProps } from "vue";

const props = defineProps({
  group: {
    type: Object,
    required: true,
  },
});
</script>

<style lang="scss" scoped>
.group-item {
  padding: 0.5rem;
  cursor: pointer;
}

.group-item:hover {
  background-color: rgba(52, 211, 153, .5);
  border-radius: .2rem;

  a {
    color: #fff;
  }
}
</style>
