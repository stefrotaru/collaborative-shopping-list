<template>
  <div class="user-profile">
    <h2>User Profile</h2>
    <div class="profile-info">
      <div class="avatar-container">
        <Avatar v-if="!isEditMode" size="100px" />

        <Carousel
          v-show="isEditMode"
          :value="avatarItems"
          :page="currentPage"
          :numVisible="1"
          :numScroll="1"
          :circular="true"
          :showIndicators="false"
          v-model:page="currentPage"
        >
          <template #item="slotProps">
            <div class="avatar">
              <img
                :src="`/avatars/${slotProps.data.file}`"
                :alt="`Avatar ${slotProps.data.id}`"
              />
            </div>
          </template>
        </Carousel>
      </div>

      <div class="info-item">
        <label>Username: </label>
        <span v-if="!isEditMode">{{
          authStore.authenticatedUser?.username
        }}</span>
        <Input v-else type="text" v-model="user.username" />
      </div>

      <div class="info-item">
        <label>Email: </label>
        <span v-if="!isEditMode">{{ authStore.authenticatedUser?.email }}</span>
        <Input v-else type="email" v-model="user.email" />
      </div>

      <Button
        v-if="!isEditMode"
        icon="pi pi-pencil"
        label="Edit Profile"
        @click="toggleEditMode"
      />
      <div class="button-group">
        <Button
          v-if="isEditMode"
          icon="pi pi-times"
          label="Cancel"
          @click="toggleEditMode"
        />
        <Button
          v-if="isEditMode"
          icon="pi pi-check"
          label="Save"
          @click="saveProfileChanges"
        />
      </div>

      <p>{{ isEditMode ? "Edit mode on" : "Edit mode off" }}</p>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch, nextTick } from "vue";
import { useAuthStore } from "../store/auth";

import Button from "primevue/button";
import Input from "primevue/inputtext";
import Carousel from "primevue/carousel";

import Avatar from "../components/Avatar.vue";
import EditProfileModal from "../components/EditProfileModal.vue";
import avatarEnum from "../data/avatars";

const authStore = useAuthStore();
const user = ref({...authStore.authenticatedUser});
const isEditMode = ref(false);

const getCurrentAvatarIndex = () => {
  return avatarItems.findIndex((avatar) => avatar.file === user.value?.avatar);
};

const avatarItems = avatarEnum.map((avatar) => ({
  ...avatar,
  itemImageSrc: `/avatars/${avatar.file}`,
}));

const currentPage = ref(getCurrentAvatarIndex());

const toggleEditMode = async () => {  
  isEditMode.value = !isEditMode.value;
};

const saveProfileChanges = async () => {
  //TODO: Save changes to the user profile

  // const updatedUser = {
  //   ...user.value,
  //   avatar: avatarItems[currentPage.value].file,
  // };
  // await authStore.updateUserProfile(updatedUser);

  console.log("🚀 Profile changes (NOT) saved");
  toggleEditMode();
};
</script>
