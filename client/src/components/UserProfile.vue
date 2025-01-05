<template>
  <div class="user-profile">
    <Avatar />
    <div class="user-info">
      <div class="user-name">John Doe</div>
      <div class="user-email">johndoe@example.com</div>
    </div>
    <Button label="Edit Profile" icon="pi pi-pencil" />
  </div>
</template>

<script setup>
import Avatar from "./Avatar.vue";
import Button from "primevue/button";
</script>

<style lang="scss" scoped>
.user-profile {
  display: flex;
  flex-direction: column;
  align-items: center;
}

.user-info {
  text-align: center;
  margin: 0.5rem 0 1rem 0;
  
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.user-name {
  font-weight: bold;
}
</style>
