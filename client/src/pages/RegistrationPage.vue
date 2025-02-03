<template>
  <div class="registration-page">
    <div class="registration-page__header">
      <img src="../assets/vue.svg" alt="Vue Shopping List" />
      <h1>Vue Shopping List Login</h1>
    </div>

    <div class="card">
      <form @submit.prevent="handleRegistration">
        <div class="card__form-group">
          <Carousel
            :value="avatarItems"
            :numVisible="1"
            :numScroll="1"
            :circular="true"
            :showIndicators="false"
            v-model:page="currentAvatarPage"
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
import { ref, computed } from "vue";
import { useRouter } from "vue-router";
import { useAuthStore } from "../store/auth";

import InputText from "primevue/inputtext";
import Password from "primevue/password";
import Button from "primevue/button";
import Carousel from "primevue/carousel";

import avatarEnum from "../data/avatars";
const avatarItems = avatarEnum.map((avatar) => ({
  ...avatar,
  itemImageSrc: `/avatars/${avatar.file}`,
}));
const currentAvatarPage = ref(0)
const selectedAvatar = computed(() => avatarItems[currentAvatarPage.value].file);


console.log("avatarEnum", avatarEnum);

const authStore = useAuthStore();
const username = ref("");
const email = ref("");
const password = ref("");
const confirmPassword = ref("");
const router = useRouter();

const handleRegistration = async () => {
  const registerResponse = await authStore.register({
    username: username.value,
    email: email.value,
    password: password.value,
    avatar: selectedAvatar.value
  });

  if (!registerResponse) {
    return;
  }

  username.value = "";
  email.value = "";
  password.value = "";
  confirmPassword.value = "";

  // TODO: Navigate to appropriate page after successful login
  router.push("/home");
};
</script>
