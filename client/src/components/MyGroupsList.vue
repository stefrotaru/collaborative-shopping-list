<template>
  <div class="groups-list">
    <div class="section-header">
      <h3>My Groups</h3>
    </div>
    
    <ul class="groups-container">
      <li v-for="group in groups" :key="group.id">
        <router-link class="group-item" :to="'/groups/' + group.id">
          <i class="pi pi-users item-icon"></i>
          <span>{{ group.name }}</span>
        </router-link>
      </li>
      
      <li v-if="groups.length === 0" class="empty-list">
        <i class="pi pi-info-circle"></i>
        <span>No groups joined yet</span>
      </li>
    </ul>
  </div>
</template>

<script setup>
import { ref, watch } from "vue";
import { useAuthStore } from "../store/auth";

const authStore = useAuthStore();
const groups = ref(authStore.userGroups || []);

watch(authStore, (newValue) => {
  groups.value = newValue.userGroups || [];
});
</script>

<style lang="scss" scoped>
.groups-list {
  display: flex;
  flex-direction: column;
}

.section-header {
  margin-bottom: 0.75rem;
  
  h3 {
    font-size: 0.9375rem;
    text-transform: uppercase;
    letter-spacing: 0.05em;
    font-weight: 600;
    color: #9ca3af;
    margin: 0;
    padding-bottom: 0.5rem;
    border-bottom: 1px solid rgba(255, 255, 255, 0.1);
  }
}

.groups-container {
  list-style-type: none;
  padding: 0;
  margin: 0;
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.group-item {
  display: flex;
  align-items: center;
  padding: 0.6rem 0.75rem;
  border-radius: 6px;
  color: #e4e4e7;
  text-decoration: none;
  transition: all 0.2s ease;
  font-size: 0.9375rem;
  
  &:hover, &.router-link-active {
    background-color: rgba(52, 211, 153, 0.15);
    color: #34d399;
  }
  
  .item-icon {
    margin-right: 0.625rem;
    font-size: 1rem;
    color: #9ca3af;
    transition: color 0.2s ease;
  }
  
  &:hover .item-icon, &.router-link-active .item-icon {
    color: #34d399;
  }
  
  span {
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
  }
}

.empty-list {
  display: flex;
  align-items: center;
  padding: 0.6rem 0.75rem;
  color: #9ca3af;
  font-size: 0.875rem;
  font-style: italic;
  
  i {
    margin-right: 0.625rem;
    font-size: 0.875rem;
  }
}
</style>