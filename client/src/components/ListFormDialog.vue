<template>
  <Dialog 
    :visible="visible" 
    @update:visible="$emit('update:visible', $event)"
    :header="list ? 'Edit Shopping List' : 'Create Shopping List'"
  >
    <!-- Group selection -->
    <div class="p-fluid" v-if="showGroupField">
      <div class="field">
        <label for="group">Group</label>
        <Dropdown 
          id="group"
          class="w-full"
          v-model="selectedGroup" 
          :options="groups" 
          optionLabel="name" 
          placeholder="Select a group"
          :disabled="!!groupId"
        />
      </div>
    </div>
    
    <!-- List name -->
    <div class="p-fluid">
      <div class="field">
        <label for="name">Name</label>
        <InputText id="name" class="w-full" v-model="name" />
      </div>
    </div>

    <template #footer>
      <Button
        label="Cancel"
        icon="pi pi-times"
        @click="$emit('update:visible', false)"
        class="p-button-text"
      />
      <Button 
        :label="list ? 'Save' : 'Create'"
        :icon="list ? 'pi pi-check' : 'pi pi-plus'"
        @click="submitForm" 
        autofocus
      />
    </template>
  </Dialog>
</template>

<script setup>
import { ref, computed, watch } from "vue";
import Dialog from "primevue/dialog";
import InputText from "primevue/inputtext";
import Dropdown from "primevue/dropdown";
import Button from "primevue/button";

const emit = defineEmits(["update:visible", "submit"]);

const props = defineProps({
  visible: {
    type: Boolean,
    default: false
  },
  // Pass existing list for edit mode, omit for create mode
  list: {
    type: Object,
    default: null
  },
  // Optional preselected group ID
  groupId: {
    type: Number,
    default: null
  },
  // Available groups to select from
  groups: {
    type: Array,
    default: () => []
  },
  // Whether to show the group field
  showGroupField: {
    type: Boolean,
    default: true
  }
});

// Form state
const name = ref("");
const selectedGroup = ref(null);

// Initialize form data when dialog opens or props change
watch(() => [props.visible, props.list, props.groupId], () => {
  if (props.visible) {
    // Set name from existing list or empty
    name.value = props.list?.name || "";
    
    // Set group based on props
    if (props.groupId) {
      selectedGroup.value = props.groups.find(g => g.id === props.groupId) || null;
    } else if (props.list?.groupId) {
      selectedGroup.value = props.groups.find(g => g.id === props.list.groupId) || null;
    } else {
      selectedGroup.value = null;
    }
  }
}, { immediate: true });

const submitForm = () => {
  if (!name.value.trim()) {
    // Simple validation
    return;
  }
  
  emit("submit", {
    id: props.list?.id,
    name: name.value,
    groupId: selectedGroup.value?.id || props.groupId
  });

  emit("update:visible", false);
};
</script>
