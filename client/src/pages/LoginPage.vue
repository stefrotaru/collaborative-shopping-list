<template>
  <div class="login-container">
    <div class="login-card">
      <div class="brand-header">
        <div class="logo-container">
          <img
            src="../assets/vue.svg"
            alt="Vue Shopping List"
            class="logo-image"
          />
        </div>
        <h1 class="brand-title">Shopping List</h1>
        <p class="brand-subtitle">Sign in to your account</p>
      </div>

      <form @submit.prevent="handleLogin" class="login-form">
        <div class="form-group">
          <label for="email">Email</label>
          <div class="input-wrapper">
            <i class="pi pi-envelope input-icon"></i>
            <InputText
              id="email"
              v-model="email"
              type="email"
              class="w-full"
              :class="{ 'p-invalid': isInvalidUser }"
              placeholder="Enter your email address"
              required
            />
          </div>
        </div>

        <div class="form-group">
          <div class="label-row">
            <label for="password">Password</label>
            <router-link to="/forgot-password" class="forgot-link"
              >Forgot password?</router-link
            >
          </div>
          <div class="input-wrapper">
            <i class="pi pi-lock input-icon"></i>
            <Password
              id="password"
              v-model="password"
              :feedback="false"
              :toggleMask="true"
              class="w-full"
              :class="{ 'p-invalid': isInvalidUser }"
              placeholder="Enter your password"
              required
            />
          </div>
        </div>

        <small v-if="isInvalidUser" class="p-error error-message">
          <i class="pi pi-exclamation-circle"></i> Invalid email or password
        </small>

        <Button
          type="submit"
          label="Sign In"
          icon="pi pi-sign-in"
          class="login-btn p-button-lg"
          :loading="loading"
        />

        <div class="form-separator">
          <div class="separator-line"></div>
          <span class="separator-text">or</span>
          <div class="separator-line"></div>
        </div>

        <div class="register-option">
          <span>Don't have an account?</span>
          <router-link to="/register" class="register-link"
            >Create Account</router-link
          >
        </div>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref } from "vue";
import { useRouter } from "vue-router";
import { useAuthStore } from "../store/auth";
import { useToast } from "primevue/usetoast";

import InputText from "primevue/inputtext";
import Password from "primevue/password";
import Button from "primevue/button";

const router = useRouter();
const authStore = useAuthStore();
const toast = useToast();

const email = ref("");
const password = ref("");
const isInvalidUser = ref(false);
const loading = ref(false);

const handleLogin = async () => {
  loading.value = true;
  isInvalidUser.value = false;

  try {
    const loginResponse = await authStore.login({
      email: email.value,
      password: password.value,
    });

    if (!loginResponse) {
      isInvalidUser.value = true;

      toast.add({
        severity: "error",
        summary: "Login Failed",
        detail: "Invalid email or password",
        life: 3000,
      });

      loading.value = false;
      return;
    }

    // Success
    toast.add({
      severity: "success",
      summary: "Login Successful",
      detail: "Welcome back!",
      life: 2000,
    });

    // Clear form
    email.value = "";
    password.value = "";

    // Get user data and navigate
    await authStore.getUserGroups(true);
    router.push("/home");
  } catch (error) {
    console.error("Login error:", error);
    toast.add({
      severity: "error",
      summary: "Login Error",
      detail: "An unexpected error occurred",
      life: 3000,
    });
  } finally {
    loading.value = false;
  }
};
</script>

<style lang="scss" scoped>
.input-wrapper {
  :deep(.p-inputtext) {
    padding-left: 2.5rem !important;
    background-color: #1e1e1e !important;
    border-color: #4b5563 !important;
    color: #e5e7eb !important;
    width: 100%;

    &:focus {
      border-color: #34d399 !important;
      box-shadow: 0 0 0 2px rgba(52, 211, 153, 0.2) !important;
    }

    &.p-invalid {
      border-color: #f87171 !important;
    }
  }

  :deep(.p-password-input) {
    width: 100%;
    padding-left: 2.5rem !important;
  }

  :deep(.p-password) {
    width: 100%;

    .p-icon {
      color: #9ca3af;

      &:hover {
        color: #e5e7eb;
      }
    }
  }
}
</style>
