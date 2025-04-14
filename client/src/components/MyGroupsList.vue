<template>
  <div class="groups-list">
    <div class="section-header">
      <h3>My Groups</h3>
    </div>

    <LoadingState v-if="delayedLoading" message="Loading groups..." />

    <ul v-else class="groups-container">
      <li v-for="group in groups" :key="group.id">
        <router-link
          class="group-item"
          :to="'/groups/' + group.id"
          :class="
            group.createdById === authStore.authenticatedUser?.id
              ? 'created-by-me'
              : 'created-by-others'
          "
        >
          <i class="pi pi-users item-icon"></i>
          <span>{{ group.name }}</span>
          <span v-if="isCreatedByUser(group)" class="owner-badge">Owner</span>
          <span v-else class="member-badge">Member</span>
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
import { ref, onMounted, computed, watch } from "vue";
import { useAuthStore } from "../store/auth";

import LoadingState from "./common/LoadingState.vue";
import { useDelayedLoading } from "./composables/useDelayedLoading";

const authStore = useAuthStore();
const groups = ref([]);
const { loading, delayedLoading, setLoading } = useDelayedLoading(500);

// Fetch all groups the user has access to
const fetchGroups = async () => {
  setLoading(true);
  try {
    await authStore.checkAuth();
    const fetchedGroups = await authStore.getUserGroups(true);
    groups.value = fetchedGroups || [];
  } catch (error) {
    console.error("Error fetching groups:", error);
  } finally {
    setLoading(false);
  }
};

// Function to check if the group was created by the current user
const isCreatedByUser = (group) => {
  if (!authStore.authenticatedUser) {
    return false;
  }

  // If you have createdById in your GroupDto, you can use it directly
  // Otherwise, you may need to modify this check based on your data structure
  console.log(group);
  console.log(group.createdById, authStore.authenticatedUser.id);
  return group.createdById === authStore.authenticatedUser.id;
};

watch(
  () => authStore.userGroups,
  (newVal) => {
    if (newVal) {
      groups.value = newVal;
    } else {
      groups.value = [];
    }
  }
);

onMounted(fetchGroups);
</script>

<style lang="scss" scoped>
@use "../scss/base/variables" as *;

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

  &:hover,
  &.router-link-active {
    background-color: $green-400-opacity-15;
    color: $primary-color;

    &.created-by-others {
      background-color: $blue-400-opacity-15;
      color: $info-color;
    }
  }


  .item-icon {
    margin-right: 0.625rem;
    font-size: 1rem;
    transition: color 0.2s ease;
  }
  &.created-by-me .item-icon {
    color: $primary-color;
  }
  &.created-by-others .item-icon {
    color: $info-color;
  }

  &:hover .item-icon,
  &.router-link-active .item-icon {
    color: $primary-color;
  }
  &.created-by-others:hover .item-icon,
  &.created-by-others.router-link-active .item-icon {
    color: $info-color;
  }

  span {
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
  }
}

.owner-badge,
.member-badge {
  font-size: 0.7rem;
  padding: 0.1rem 0.4rem;
  border-radius: 4px;
  margin-left: auto;
}

.owner-badge {
  background-color: #10b981;
  color: white;
}

.member-badge {
  background-color: #3b82f6;
  color: white;
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

.loading-state {
  display: flex;
  align-items: center;
  padding: 0.6rem 0.75rem;
  color: #9ca3af;
  font-size: 0.875rem;

  .loading-icon {
    margin-right: 0.625rem;
    font-size: 1rem;
  }
}
</style>
