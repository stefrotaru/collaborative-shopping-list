<template>
  <div class="registration-page">
    <div class="registration-page__header">
      <img src="../assets/vue.svg" alt="Vue Shopping List" />
      <h1>Vue Shopping List Login</h1>
    </div>

    <div class="card">
      <form @submit.prevent="handleRegistration">
        <div class="card__form-group">
          <label for="username">Username</label>
          <InputText id="username" v-model="username" required />
        </div>
        <div class="card__form-group">
          <label for="email">Email</label>
          <InputText id="email" v-model="email" type="email" required />
        </div>
        <div class="card__form-group">
          <label for="password">Password</label>
          <Password id="password" v-model="password" required />
        </div>
        <div class="card__form-group">
          <label for="confirmPassword">Confirm Password</label>
          <Password id="confirmPassword" v-model="confirmPassword" required />
        </div>
        <Button type="submit" label="Register" class="card__submit-btn" />
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref } from "vue";
import { useRouter } from "vue-router";
import { useAuthStore } from '../store/auth';
import InputText from "primevue/inputtext";
import Password from "primevue/password";
import Button from "primevue/button";

const authStore = useAuthStore();
const username = ref("");
const email = ref("");
const password = ref("");
const confirmPassword = ref("");
const router = useRouter();

const handleRegistration = async () => {
  const registerResponse = await authStore.register(username.value, email.value, password.value);
  
  if (!registerResponse) {
    return;
  }

  username.value = '';
  email.value = '';
  password.value = '';
  confirmPassword.value = '';

  // TODO: Navigate to appropriate page after successful login
  router.push('/home');
};
</script>

<style lang="scss" scoped>
.registration-page {
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

.form-group {
  margin-bottom: 1rem;
}
</style>
