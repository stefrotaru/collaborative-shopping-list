<template>
  <Dialog :visible="visible" @update:visible="$emit('update:visible', $event)">
    <template #header>
      <h3>Create Shopping List</h3>
    </template>
    
    <div class="p-fluid">
      <div class="field">
        <label for="group">group</label>
        <InputText id="group" v-model="listGroup" />
      </div>
    </div>
    <div class="p-fluid">
      <div class="field">
        <label for="name">Name</label>
        <InputText id="name" v-model="listName" />
      </div>
    </div>

    <template #footer>
      <Button
        label="Cancel"
        icon="pi pi-times"
        @click="closeDialog"
        class="p-button-text"
      />
      <Button label="Create" icon="pi pi-check" @click="createList" autofocus />
    </template>
  </Dialog>
</template>

<script setup>
import { ref } from "vue";
import Dialog from "primevue/dialog";
import InputText from "primevue/inputtext";
import Button from "primevue/button";

import { useShoppingListsStore } from "../store/shoppingLists";
import { useAuthStore } from "../store/auth";
const shoppingListsStore = useShoppingListsStore();
const authStore = useAuthStore();

const emit = defineEmits(["update:visible", "list-created"]);

const props = defineProps({
  visible: {
    type: Boolean,
    default: false,
  },
});

const listGroup = ref("");
const listName = ref("");

const closeDialog = () => {
  emit("update:visible", false);
};

const createList = async () => {
  let group = authStore.userGroups.find((group) => group.name === listGroup.value);
  let userId = authStore.authenticatedUser.id;
  // if there is no group, create group first
  if (!group) {
    return alert("Group not found");
  }

  console.log(group.id);

  if (!userId) {
    return;
  }
  
  try {
    await shoppingListsStore.createNewList(listName.value, group.id, userId);
  } catch (error) {
    console.error("Error creating list", error);
  }
  
  listName.value = "";
  emit("list-created", listName.value);
  closeDialog();
};
</script>
