<template>
  <Dialog :visible="visible" @update:visible="$emit('update:visible', $event)">
    <template #header>
      <h3>Add Item</h3>
    </template>

    <div class="p-fluid">
      <div class="field">
        <label for="name">Name</label>
        <InputText id="name" v-model="newItem.name" />
      </div>
      <div class="field">
        <label for="quantity">Quantity</label>
        <InputNumber id="quantity" v-model="newItem.quantity" />
      </div>
    </div>

    <template #footer>
      <Button
        label="Cancel"
        icon="pi pi-times"
        @click="closeDialog"
        class="p-button-text"
      />
      <Button label="Add" icon="pi pi-check" @click="addItem" autofocus />
    </template>
  </Dialog>
</template>

<script setup>
import { ref } from "vue";
import Dialog from "primevue/dialog";
import InputText from "primevue/inputtext";
import InputNumber from "primevue/inputnumber";
import Button from "primevue/button";

const emit = defineEmits(["update:visible", "item-added"]);

const props = defineProps({
  visible: {
    type: Boolean,
    default: false,
  },
});

const newItem = ref({
  name: "",
  quantity: 1,
});

const closeDialog = () => {
  emit("update:visible", false);
};

const addItem = () => {
  emit("item-added", newItem.value);
  newItem.value = {
    name: "",
    quantity: 1,
  };
  closeDialog();
};
</script>
