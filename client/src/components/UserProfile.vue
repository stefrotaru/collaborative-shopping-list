<template>
  <div class="user-profile">
    <Avatar />
    <div v-if="authStore.authenticatedUser" class="user-info">
      <div class="user-name">{{authStore.authenticatedUser.username}}</div>
      <div class="user-email">{{authStore.authenticatedUser.email}}</div>
    </div>
    <div v-else class="user-info">
      <div class="user-name">Guest</div>
    </div>

    <div v-if="authStore.authenticatedUser" class="user-profile__btns">
      <Button
        label="Profile Page"
        @click="goToProfilePage"
        icon="pi pi-user"
      />
      <Button
        label="Logout"
        severity="danger"
        @click="handleLogout"
        icon="pi pi-sign-out"
      />
    </div>
    <div v-else class="user-profile__btns">
      <Button
        label="Login"
        @click="$router.push('/login')"
      />
    </div>
  </div>
</template>

<script setup>
import Avatar from "./Avatar.vue";
import Button from "primevue/button";

import { useRouter } from "vue-router";
import { useAuthStore } from "../store/auth.ts";
import { useShoppingListsStore } from "../store/shoppingLists.ts";

const router = useRouter();
const authStore = useAuthStore();
const shoppingListsStore = useShoppingListsStore();


const goToProfilePage = () => {
  router.push("/profile");
};
const handleLogout = () => {
  authStore.logout();
  
  //clear store state
  authStore.resetStore();
  shoppingListsStore.resetStore();
  
  router.push("/login");
};
</script>
