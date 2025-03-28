<template>
  <div class="access-denied-page">
    <div class="access-denied-container">
      <div class="icon-container">
        <i class="pi pi-lock access-denied-icon"></i>
      </div>
      <h1 class="page-title">Access Denied</h1>
      <p class="message">You don't have permission to view this resource.</p>
      <p class="sub-message">
        It might belong to another user who hasn't shared it with you via a
        group.
      </p>

      <div class="actions">
        <Button
          label="Go to My Lists"
          icon="pi pi-shopping-cart"
          @click="$router.push('/shoppinglists')"
          class="p-button-primary action-btn"
        />
        <Button
          label="Go Home"
          icon="pi pi-home"
          @click="$router.push('/home')"
          class="p-button-outlined action-btn"
        />
      </div>
    </div>
  </div>
</template>

<script setup>
import Button from "primevue/button";
</script>

<style scoped>
.access-denied-page {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 80vh;
}

.access-denied-container {
  max-width: 600px;
  padding: 2rem;
  text-align: center;
  background-color: #262626;
  border-radius: 8px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.3);
}

.icon-container {
  margin-bottom: 1.5rem;
}

.access-denied-icon {
  font-size: 5rem;
  color: #ef4444;
}

.page-title {
  font-size: 2rem;
  margin-bottom: 1rem;
  color: #ef4444;
}

.message {
  font-size: 1.2rem;
  margin-bottom: 0.5rem;
  color: #e5e7eb;
}

.sub-message {
  font-size: 1rem;
  margin-bottom: 2rem;
  color: #9ca3af;
}

.actions {
  display: flex;
  justify-content: center;
  gap: 1rem;
  margin-top: 2rem;
}

.action-btn {
  min-width: 150px;
}

@media (max-width: 640px) {
  .actions {
    flex-direction: column;
  }

  .action-btn {
    margin-bottom: 0.5rem;
  }
}
</style>
