<template>
  <div class="registration-container">
    <div class="registration-card">
      <div class="brand-header">
        <div class="logo-container">
          <img
            src="../assets/vue.svg"
            alt="Vue Shopping List"
            class="logo-image"
          />
        </div>
        <h1 class="brand-title">Shopping List</h1>
        <p class="brand-subtitle">Create an account to get started</p>
      </div>

      <form @submit.prevent="handleRegistration" class="registration-form">
        <div class="avatar-selection">
          <h3 class="section-title">Choose your avatar</h3>
          <div class="avatar-carousel-container">
            <Button
              icon="pi pi-chevron-left"
              class="p-button-rounded p-button-text carousel-nav-btn"
              @click="previousAvatar"
            />

            <Carousel
              :value="avatarItems"
              :numVisible="1"
              :numScroll="1"
              :circular="true"
              :showIndicators="false"
              :showNavigators="false"
              v-model:page="currentAvatarPage"
              class="avatar-carousel"
            >
              <template #item="slotProps">
                <div class="avatar-item">
                  <img
                    :src="`/avatars/${slotProps.data.file}`"
                    :alt="`Avatar ${slotProps.data.id}`"
                    class="avatar-image"
                  />
                </div>
              </template>
            </Carousel>

            <Button
              icon="pi pi-chevron-right"
              class="p-button-rounded p-button-text carousel-nav-btn"
              @click="nextAvatar"
            />
          </div>
          <div class="avatar-pages">
            <span class="current-page">{{ currentAvatarPage + 1 }}</span>
            <span class="page-divider">/</span>
            <span class="total-pages">{{ avatarItems.length }}</span>
          </div>
        </div>

        <div class="form-fields">
          <div class="form-group">
            <label for="username">Username</label>
            <div class="input-wrapper">
              <i class="pi pi-user input-icon"></i>
              <InputText
                id="username"
                v-model="formData.username"
                required
                class="w-full"
                placeholder="Enter your username"
                :class="{ 'p-invalid': v$.username.$error }"
                @blur="v$.username.$touch()"
              />
            </div>
            <small v-if="v$.username.$error" class="p-error">{{
              v$.username.$errors[0].$message
            }}</small>
          </div>

          <div class="form-group">
            <label for="email">Email</label>
            <div class="input-wrapper">
              <i class="pi pi-envelope input-icon"></i>
              <InputText
                id="email"
                v-model="formData.email"
                type="email"
                required
                class="w-full"
                placeholder="Enter your email address"
                :class="{ 'p-invalid': v$.email.$error }"
                @blur="v$.email.$touch()"
              />
            </div>
            <small v-if="v$.email.$error" class="p-error">{{
              v$.email.$errors[0].$message
            }}</small>
          </div>

          <div class="form-group">
            <label for="password">Password</label>
            <Password
              id="password"
              v-model="formData.password"
              required
              class="w-full"
              inputClass="w-full p-password-input"
              :class="{ 'p-invalid': v$.password.$error }"
              :feedback="true"
              :toggleMask="true"
              placeholder="Create a password"
              @blur="v$.password.$touch()"
            >
              <template #header>
                <h6>Choose a secure password</h6>
              </template>
              <template #footer>
                <div class="password-requirements">
                  <p>Password must contain:</p>
                  <ul>
                    <li :class="{ 'requirement-met': passwordLength }">
                      At least 8 characters
                    </li>
                    <li :class="{ 'requirement-met': passwordUppercase }">
                      At least one uppercase letter
                    </li>
                    <li :class="{ 'requirement-met': passwordLowercase }">
                      At least one lowercase letter
                    </li>
                    <li :class="{ 'requirement-met': passwordNumber }">
                      At least one number
                    </li>
                  </ul>
                </div>
              </template>
            </Password>
            <small v-if="v$.password.$error" class="p-error">{{
              v$.password.$errors[0].$message
            }}</small>
          </div>

          <div class="form-group">
            <label for="confirmPassword">Confirm Password</label>
            <div class="input-wrapper">
              <i class="pi pi-lock input-icon"></i>
              <InputText
                id="confirmPassword"
                v-model="formData.confirmPassword"
                type="password"
                required
                class="w-full"
                placeholder="Confirm your password"
                :class="{ 'p-invalid': v$.confirmPassword.$error }"
                @blur="v$.confirmPassword.$touch()"
              />
            </div>
            <small v-if="v$.confirmPassword.$error" class="p-error">{{
              v$.confirmPassword.$errors[0].$message
            }}</small>
          </div>
        </div>

        <div class="form-actions">
          <Button
            type="submit"
            label="Create Account"
            icon="pi pi-user-plus"
            class="register-btn p-button-lg"
            :loading="loading"
            :disabled="loading || v$.$invalid"
          />

          <div class="form-separator">
            <div class="separator-line"></div>
            <span class="separator-text">or</span>
            <div class="separator-line"></div>
          </div>

          <div class="form-alternative">
            <span>Already have an account?</span>
            <router-link to="/login" class="login-link">Sign In</router-link>
          </div>
        </div>
      </form>
    </div>

    <div class="registration-info">
      <div class="info-card">
        <div class="info-header">
          <i class="pi pi-check-circle info-icon"></i>
          <h3 class="info-title">Benefits of Joining</h3>
        </div>
        <ul class="benefits-list">
          <li>
            <i class="pi pi-list"></i>
            <span>Create and manage multiple shopping lists</span>
          </li>
          <li>
            <i class="pi pi-users"></i>
            <span>Share lists with family and friends</span>
          </li>
          <li>
            <i class="pi pi-mobile"></i>
            <span>Access your lists on any device</span>
          </li>
          <li>
            <i class="pi pi-sync"></i>
            <span>Real-time updates across all devices</span>
          </li>
          <li>
            <i class="pi pi-shield"></i>
            <span>Secure and private data storage</span>
          </li>
        </ul>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, reactive } from "vue";
import { useRouter } from "vue-router";
import { useAuthStore } from "../store/auth";
import { useToast } from "primevue/usetoast";
import { useVuelidate } from "@vuelidate/core";
import {
  required,
  email,
  minLength,
  helpers,
} from "@vuelidate/validators";

import InputText from "primevue/inputtext";
import Password from "primevue/password";
import Button from "primevue/button";
import Carousel from "primevue/carousel";

import avatarEnum from "../data/avatars";

const authStore = useAuthStore();
const router = useRouter();
const toast = useToast();
const loading = ref(false);

// Avatar selection
const avatarItems = avatarEnum.map((avatar) => ({
  ...avatar,
  itemImageSrc: `/avatars/${avatar.file}`,
}));
const currentAvatarPage = ref(0);
const selectedAvatar = computed(
  () => avatarItems[currentAvatarPage.value].file
);

// Form data and validation
const formData = reactive({
  username: "",
  email: "",
  password: "",
  confirmPassword: "",
});

// Custom validators
const containsUppercase = (value) => /[A-Z]/.test(value);
const containsLowercase = (value) => /[a-z]/.test(value);
const containsNumber = (value) => /[0-9]/.test(value);

const rules = {
  username: {
    required: helpers.withMessage("Username is required", required),
    minLength: helpers.withMessage(
      "Username must be at least 3 characters",
      minLength(3)
    ),
  },
  email: {
    required: helpers.withMessage("Email is required", required),
    email: helpers.withMessage("Please enter a valid email address", email),
  },
  password: {
    required: helpers.withMessage("Password is required", required),
    minLength: helpers.withMessage(
      "Password must be at least 8 characters",
      minLength(8)
    ),
    containsUppercase: helpers.withMessage(
      "Password must contain at least one uppercase letter",
      containsUppercase
    ),
    containsLowercase: helpers.withMessage(
      "Password must contain at least one lowercase letter",
      containsLowercase
    ),
    containsNumber: helpers.withMessage(
      "Password must contain at least one number",
      containsNumber
    ),
  },
  confirmPassword: {
    required: helpers.withMessage("Please confirm your password", required),
    sameAsPassword: helpers.withMessage(
      "Passwords do not match",
      function (value) {
        return value === formData.password;
      }
    ),
  },
};

const v$ = useVuelidate(rules, formData);

// Password strength indicators
const passwordLength = computed(() => formData.password.length >= 8);
const passwordUppercase = computed(() => containsUppercase(formData.password));
const passwordLowercase = computed(() => containsLowercase(formData.password));
const passwordNumber = computed(() => containsNumber(formData.password));

// Avatar navigation
const previousAvatar = () => {
  if (currentAvatarPage.value > 0) {
    currentAvatarPage.value--;
  } else {
    currentAvatarPage.value = avatarItems.length - 1;
  }
};

const nextAvatar = () => {
  if (currentAvatarPage.value < avatarItems.length - 1) {
    currentAvatarPage.value++;
  } else {
    currentAvatarPage.value = 0;
  }
};

// Form submission
const handleRegistration = async () => {
  // Validate all form fields
  const isFormValid = await v$.value.$validate();
  if (!isFormValid) return;

  loading.value = true;

  try {
    const registerResponse = await authStore.registerUser({
      username: formData.username,
      email: formData.email,
      password: formData.password,
      avatar: selectedAvatar.value,
    });

    if (!registerResponse) {
      toast.add({
        severity: "error",
        summary: "Registration Failed",
        detail: "There was an error creating your account. Please try again.",
        life: 5000,
      });
      loading.value = false;
      return;
    }

    toast.add({
      severity: "success",
      summary: "Registration Successful",
      detail: "Your account has been created successfully!",
      life: 3000,
    });

    // Reset form
    formData.username = "";
    formData.email = "";
    formData.password = "";
    formData.confirmPassword = "";
    v$.value.$reset();

    router.push("/home");
  } catch (error) {
    console.error("Registration error:", error);
    toast.add({
      severity: "error",
      summary: "Registration Failed",
      detail: error.message || "There was an error creating your account.",
      life: 5000,
    });
  } finally {
    loading.value = false;
  }
};
</script>

<style lang="scss" scoped>
.registration-container {
  .avatar-carousel {
    width: 60%;
    margin: 0 0.5rem;

    :deep(.p-carousel-container) {
      padding: 0;
    }

    :deep(.p-carousel-items-container) {
      overflow: visible;
    }
  }

  .form-group {
    .p-password {
      :deep(.p-password-panel) {
        background-color: #1e1e1e;
        color: #e5e7eb;
        border: 1px solid #4b5563;

        h6 {
          color: #34d399;
          margin-top: 0;
        }
      }

      :deep(.p-password-meter) {
        background-color: #4b5563;
      }

      :deep(.p-password-input) {
        padding-right: 2.5rem !important;
      }

      :deep(.pi-eye),
      :deep(.pi-eye-slash) {
        color: #9ca3af;

        &:hover {
          color: #34d399;
        }
      }
    }
  }

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
    }
  }
}
</style>
