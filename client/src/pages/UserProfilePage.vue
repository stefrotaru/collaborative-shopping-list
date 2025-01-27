<template>
  <div class="user-profile">
    <h2>User Profile</h2>
    <div class="profile-info">
      <Avatar size="100px" />
      <div class="info-item">
        <label>Username: </label>
        <span>{{ authStore.authenticatedUser?.username }}</span>
      </div>
      <div class="info-item">
        <label>Email: </label>
        <span>{{ authStore.authenticatedUser?.email }}</span>
      </div>
      <Button icon="pi pi-pencil" label="Edit Profile" @click="openEditModal" />
      <EditProfileModal
        :user="user"
        :visible="showEditModal"
        @update:visible="showEditModal = $event"
        @update-user="updateUser"
      />
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import { useAuthStore } from "../store/auth";
import Button from "primevue/button";

import Avatar from "../components/Avatar.vue";
import EditProfileModal from '../components/EditProfileModal.vue';


const authStore = useAuthStore();
const user = ref(authStore.authenticatedUser);
const showEditModal = ref(false);

// onMounted(async () => {
// });

const openEditModal = () => {
  showEditModal.value = true;
};
const updateUser = (updatedUser) => {
  user.value = updatedUser;
};
</script>
