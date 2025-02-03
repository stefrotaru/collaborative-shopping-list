<template>
  <Dialog :visible="visible" @update:visible="$emit('update:visible', $event)">
    <template #header>
      <h2>Edit Profile</h2>
    </template>

    <div class="form-group">
      <label for="username">Username</label>
      <InputText id="username" v-model="editedUser.username" />
    </div>
    <div class="form-group">
      <label for="email">Email</label>
      <InputText id="email" v-model="editedUser.email" />
    </div>
    <!-- Add more form fields as needed -->

    <template #footer>
      <Button label="Cancel" @click="$emit('update:visible', false)" />
      <Button label="Save" @click="saveProfile" />
    </template>
  </Dialog>
</template>

<script setup>
import { ref } from "vue";
import Dialog from "primevue/dialog";
import InputText from "primevue/inputtext";
import Button from "primevue/button";

//TODO: not used anymore. Remove it or make it a general component

const props = defineProps({
  user: {
    type: Object,
    required: true,
  },
  visible: {
    type: Boolean,
    default: false,
  },
});

const emit = defineEmits(["update:visible", "update-user"]);

const editedUser = ref({ ...props.user });

const saveProfile = () => {
  // TODO: Call API to update user data
  emit("update-user", editedUser.value);
  emit("update:visible", false);
};
</script>

<style lang="scss" scoped>
.form-group {
  margin-bottom: 1rem;

  display: flex;
  align-items: center;
  justify-content: space-between;
}
</style>
