<template>
  <footer class="app-footer">
    <div class="footer-content">
      <div class="footer-logo">Link-List</div>
      <!-- <div class="footer-links">
        <a href="#" @click.prevent="navigateTo('/about')">About</a>
        <a href="#" @click.prevent="navigateTo('/privacy')">Privacy</a>
        <a href="#" @click.prevent="navigateTo('/terms')">Terms</a>
        <a href="#" @click.prevent="navigateTo('/contact')">Contact</a>
      </div> -->
      <div class="footer-copyright">
        &copy; {{ new Date().getFullYear() }} Link-List App by Stefan Rotaru. All rights reserved.
      </div>
    </div>
  </footer>
</template>

<script setup>
import { useRouter } from "vue-router";

const router = useRouter();

const navigateTo = (path) => {
  router.push(path);
};
</script>
