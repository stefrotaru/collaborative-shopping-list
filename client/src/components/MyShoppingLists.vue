<template>
  <div class="shopping-lists">
    <div class="section-header">
      <h3>My Shopping Lists</h3>
    </div>
    
    <ul class="lists-container">
      <li>
        <router-link class="group-item" to="/shoppinglists">
          <i class="pi pi-list-check item-icon"></i>
          <span>Show all lists</span>
        </router-link>
      </li>
      
      <li v-for="shoppingList in shoppingLists" :key="shoppingList.id">
        <router-link
          class="group-item"
          :to="'/shoppinglists/' + shoppingList.id"
        >
          <i class="pi pi-shopping-bag item-icon"></i>
          <span>{{ shoppingList.name }}</span>
        </router-link>
      </li>
      
      <li v-if="shoppingLists.length === 0" class="empty-list">
        <i class="pi pi-info-circle"></i>
        <span>No shopping lists yet</span>
      </li>
    </ul>
  </div>
</template>

<script setup>
import { ref, watch } from "vue";
import { useAuthStore } from "../store/auth";
import { useShoppingListsStore } from "../store/shoppingLists";

const authStore = useAuthStore();
const shoppingListsStore = useShoppingListsStore();
const shoppingLists = ref(shoppingListsStore.userShoppingLists);

watch(
  () => shoppingListsStore.userShoppingLists,
  (newVal) => {
    if (newVal) {
      shoppingLists.value = newVal;
    } else {
      shoppingLists.value = [];
    }
  }
);

const fetchShoppingLists = async () => {
  if (authStore.userGroups?.length > 0) {
    const groupIds = authStore.userGroups.map(group => group.id);
    await shoppingListsStore.fetchAllGroupsShoppingLists(groupIds);
  }
};

fetchShoppingLists();
</script>

<style lang="scss" scoped>
.shopping-lists {
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

.lists-container {
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