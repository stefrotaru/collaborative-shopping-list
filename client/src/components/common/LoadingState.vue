<template>
  <div class="loading-state">
    <i class="pi pi-spin pi-spinner loading-icon"></i>
    <p class="loading-message">{{ message }}</p>
  </div>
</template>

<script setup>
defineProps({
  message: {
    type: String,
    default: "Loading...",
  },
});
</script>
