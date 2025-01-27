<template>
  <div class="avatar">
    <img :src="imageString" :style="{ width: props.size, height: props.size }" alt="User Avatar" />
  </div>
</template>

<script setup>
import { onMounted, ref, watch } from "vue";
import { useAuthStore } from "../store/auth.ts";

//TODO: add withDefaults
const props = defineProps({
  size: {
    type: String,
    default: '40px'
  }
})

const pathToAvatars = "/avatars/";
let imageString = ref(pathToAvatars + 'user-icon-svgrepo.svg'); // default image

//update imageString when authenticatedUser changes
watch(() => useAuthStore().authenticatedUser, (newVal) => {
  if (newVal && newVal.avatar && newVal.avatar !== "") {
    imageString.value = pathToAvatars + newVal.avatar;
  } else {
    imageString.value = pathToAvatars + 'user-icon-svgrepo.svg';
  }
});

onMounted(() => {
  const authenticatedUser = useAuthStore().authenticatedUser;
  if (authenticatedUser && authenticatedUser.avatar && authenticatedUser.avatar !== "") {
    imageString.value = pathToAvatars + authenticatedUser.avatar;
  }
});
</script>
