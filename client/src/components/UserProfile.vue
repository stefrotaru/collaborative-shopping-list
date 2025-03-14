<template>
  <div class="user-profile">
    <div class="profile-header">
      <div class="avatar-container">
        <Avatar :size="55" />
        <div
          v-if="authStore.authenticatedUser"
          class="status-indicator online"
        ></div>
      </div>

      <div class="user-info">
        <div v-if="authStore.authenticatedUser" class="user-name">
          {{ authStore.authenticatedUser.username }}
        </div>
        <div v-else class="user-name">Guest</div>

        <div v-if="authStore.authenticatedUser" class="user-email">
          {{ authStore.authenticatedUser.email }}
        </div>
      </div>
    </div>

    <div v-if="authStore.authenticatedUser" class="profile-actions">
      <Button class="profile-btn" @click="goToProfilePage">
        <i class="pi pi-user"></i>
        <span>My Profile</span>
      </Button>

      <Button class="logout-btn" @click="handleLogout">
        <i class="pi pi-sign-out"></i>
        <span>Logout</span>
      </Button>
    </div>

    <div v-else class="profile-actions">
      <Button class="login-btn" @click="$router.push('/login')">
        <i class="pi pi-sign-in"></i>
        <span>Login</span>
      </Button>
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

  // Clear store state
  authStore.resetStore();
  shoppingListsStore.resetStore();

  router.push("/login");
};
</script>

<style lang="scss" scoped>
.user-profile {
  .profile-actions {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;

    .profile-btn,
    .logout-btn,
    .login-btn {
      text-align: left;
      background-color: transparent;
      border: none;
      color: #e4e4e7;
      display: flex;
      align-items: center;
      gap: 0.625rem;
      border-radius: 6px;
      transition: all 0.2s ease;
      font-size: 0.875rem;
      width: 100%;

      i {
        font-size: 1rem;
      }

      &:hover {
        background-color: rgba(255, 255, 255, 0.05);
      }
    }

    .profile-btn {
      color: #34d399;

      i {
        color: #34d399;
      }

      &:hover {
        background-color: rgba(52, 211, 153, 0.1);
      }
    }

    .logout-btn {
      color: #f87171;

      i {
        color: #f87171;
      }

      &:hover {
        background-color: rgba(248, 113, 113, 0.1);
      }
    }

    .login-btn {
      color: #34d399;
      justify-content: center;
      font-weight: 500;
      background-color: rgba(52, 211, 153, 0.1);
      border: 1px solid rgba(52, 211, 153, 0.3);

      &:hover {
        background-color: rgba(52, 211, 153, 0.2);
      }
    }
  }
}
</style>
