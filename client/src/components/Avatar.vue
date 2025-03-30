<template>
  <div class="avatar">
    <img :src="imageString" :style="{ width: `${props.size}px`, height: `${props.size}px` }" alt="User Avatar" />
  </div>
</template>

<script setup>
import { onMounted, ref, watch } from "vue";
import { useAuthStore } from "../store/auth.ts";

//TODO: add withDefaults
const props = defineProps({
  size: {
    type: Number,
    default: 40
  },
  userAvatar: {
    type: String,
    default: null
  }
})

const pathToAvatars = "/avatars/";
let imageString = ref(pathToAvatars + 'user-icon-svgrepo.svg'); // default image

//update imageString when authenticatedUser changes
watch(() => useAuthStore().authenticatedUser, (newVal) => {
  if (newVal && newVal.avatar && newVal.avatar !== "" && props.userAvatar === null) {
    imageString.value = pathToAvatars + newVal.avatar;
  } else {
    imageString.value = pathToAvatars + 'user-icon-svgrepo.svg';
  }
});

onMounted(() => {
  const authenticatedUser = useAuthStore().authenticatedUser;
  if (authenticatedUser && authenticatedUser.avatar && authenticatedUser.avatar !== "" && props.userAvatar === null) {
    imageString.value = pathToAvatars + authenticatedUser.avatar;
  } else if (props.userAvatar && props.userAvatar !== "") {
    imageString.value = pathToAvatars + props.userAvatar;
  } else {
    imageString.value = pathToAvatars + 'user-icon-svgrepo.svg';
  }
});
</script>
