<template>
  <div class="error-state">
    <i class="pi pi-exclamation-triangle error-icon"></i>
    <p class="error-message">{{ message }}</p>
    <div class="error-actions">
      <Button
        v-if="showBackButton"
        :label="backButtonLabel"
        icon="pi pi-arrow-left"
        @click="onBack"
        class="p-button-outlined"
      />
      <Button
        v-if="showRetryButton"
        :label="retryButtonLabel"
        icon="pi pi-refresh"
        @click="onRetry"
        class="p-button-primary"
      />
    </div>
  </div>
</template>

<script setup>
import Button from "primevue/button";

const props = defineProps({
  message: {
    type: String,
    required: true,
  },
  showBackButton: {
    type: Boolean,
    default: true,
  },
  backButtonLabel: {
    type: String,
    default: "Go Back",
  },
  showRetryButton: {
    type: Boolean,
    default: true,
  },
  retryButtonLabel: {
    type: String,
    default: "Try Again",
  },
});

const emit = defineEmits(["back", "retry"]);

const onBack = () => {
  emit("back");
};

const onRetry = () => {
  emit("retry");
};
</script>
