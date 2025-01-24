<template>
    <router-link class="group-item" :to="'/groups/' + group.id">{{ group.name }}</router-link>
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
  padding: .8rem .5rem;
  cursor: pointer;
  display: block;
}

.group-item:hover {
  background-color: rgba(52, 211, 153, .5);
  border-radius: .2rem;

  a {
    color: #fff;
  }
}
</style>
