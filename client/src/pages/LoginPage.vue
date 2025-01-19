<template>
  <div class="login-page">
    <div class="login-page__header">
      <img src="../assets/vue.svg" alt="Vue Shopping List" />
      <h1>Vue Shopping List Login</h1>
    </div>

    <div class="card">
      <!-- <h2 class="card__title">Login</h2> -->
      <form @submit.prevent="handleLogin">
        <div class="card__form-group">
          <label for="email">Email</label>
          <InputText id="email" v-model="email" type="email" required />
        </div>
        <div class="card__form-group">
          <label for="password">Password</label>
          <Password id="password" v-model="password" :feedback="false" required />
        </div>
        <Button type="submit" label="Login" class="card__submit-btn" />
      </form>
    </div>

    <div v-if="isInvalidUser" class="p-error">Invalid email or password 😕</div>

    <div>
      <p>
        Don't have an account?
        <router-link to="/register">Register</router-link>
      </p>
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

const handleLogin = async () => {
  const registerResponse = await authStore.login(email.value, password.value);

  console.log("registerResponse", registerResponse);
  if (!registerResponse) {
    console.log("Login failed", registerResponse);
    isInvalidUser.value = true;

    //TODO: create toast service
    toast.add({
      severity: "error",
      summary: "Login failed",
      detail: "Invalid email or password",
      life: 3000,
    });

    return;
  }

  email.value = "";
  password.value = "";

  authStore.getUserGroups();

  // TODO: Navigate to appropriate page after successful login
  router.push("/home");
};
</script>

<style lang="scss" scoped>
.login-page {
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;

  &__header {
    text-align: center;

    img {
      height: 5em;
    }

    h1 {
      font-size: 1.5rem;
    }
  }
}

.card {
  width: 370px;
  padding: 2rem;
  border-radius: 4px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);

  &__form-group {
    margin-bottom: 1rem;
    display: flex;
    align-items: center;
    justify-content: space-between;
  }

  &__submit-btn {
    width: 100%;
    margin-top: 1rem;
  }
}
</style>
