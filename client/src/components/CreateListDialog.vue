<template>
  <Dialog :visible="visible" @update:visible="$emit('update:visible', $event)">
    <template #header>
      <h3>Create Shopping List</h3>
    </template>

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

const emit = defineEmits(["update:visible", "list-created"]);

const props = defineProps({
  visible: {
    type: Boolean,
    default: false,
  },
});

const listName = ref("");

const closeDialog = () => {
  emit("update:visible", false);
};

const createList = () => {
  emit("list-created", listName.value);
  listName.value = "";
  closeDialog();
};
</script>
