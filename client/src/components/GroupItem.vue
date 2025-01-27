<template>
  <router-link class="group-item" :to="'/groups/' + group.id">
    {{ group.name }}
  </router-link>
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
