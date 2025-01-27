<template>
  <div class="home-page">
    <h1>Welcome to the Shopping List App</h1>
    <p>Manage your shopping lists and collaborate with others easily.</p>
    <Button label="Get Started" @click="navigateToGroups" />
  </div>
</template>

<script setup>
import { useRouter } from "vue-router";
import Button from "primevue/button";

const router = useRouter();

const navigateToGroups = () => {
  router.push("/groups");
};
</script>
