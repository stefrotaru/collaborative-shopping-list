<template>
  <div class="shopping-lists-list">
    <div>
      <h3>My Shopping Lists</h3>
      <!-- <Button
        icon="pi pi-chevron-down"
        severity="contrast"
        variant="text"
        rounded
        aria-label="Star"
      /> -->
    </div>
    <ul>
      <div class="group-item">
        <router-link to="/shoppinglists">Show all</router-link>
      </div>
      <li v-for="group in groups" :key="group.id">
        <GroupItem :group="group" />
      </li>
    </ul>
  </div>
</template>

<script setup>
import { ref } from "vue";
import GroupItem from "./GroupItem.vue";
// import { Button } from "primevue";

const groups = ref([
  { id: 0, name: "Today's shopping" },
  { id: 1, name: "Tomorrow's shoppings" },
  { id: 2, name: "Presents for Christmas" },
  { id: 3, name: "Party shopping list" },
]);
</script>

<style lang="scss" scoped>
/* duplicate from GroupItem.vue */
.group-list {
  margin-top: 1rem;
}
.group-item:hover {
  background-color: rgba(52, 211, 153, .5);
  border-radius: .2rem;

  a {
    color: #fff;
  }
}
/* duplicate from GroupItem.vue */

.group-item {
  padding: 0.5rem;
  cursor: pointer;
}

ul {
  list-style: none;
  padding: 0;
}
</style>
