<template>
  <div class="home-container">
    <div class="hero-section">
      <div class="hero-content">
        <h1 class="hero-title">
          <span class="title-accent">Simplify</span> your shopping experience
        </h1>
        <p class="hero-description">
          Create, manage and share shopping lists with friends and family. 
          Never forget an item again with our collaborative shopping platform.
        </p>
        <div class="hero-actions">
          <Button 
            label="Get Started" 
            icon="pi pi-arrow-right" 
            class="p-button-rounded get-started-btn" 
            @click="getStarted" 
          />
          <Button 
            v-if="!authenticatedUser"
            label="Learn More" 
            icon="pi pi-info-circle" 
            class="p-button-rounded p-button-outlined learn-more-btn" 
            @click="scrollToFeatures"
          />
        </div>
        <div class="hero-stats" v-if="authenticatedUser">
          <div class="stat-item">
            <span class="stat-value">{{ userStats.lists || 0 }}</span>
            <span class="stat-label">Shopping Lists</span>
          </div>
          <div class="stat-item">
            <span class="stat-value">{{ userStats.groups || 0 }}</span>
            <span class="stat-label">Groups</span>
          </div>
          <div class="stat-item">
            <span class="stat-value">{{ userStats.items || 0 }}</span>
            <span class="stat-label">Items</span>
          </div>
        </div>
      </div>
      <div class="hero-visual">
        <div class="visual-container">
          <!-- Animated shopping list illustration -->
          <div class="shopping-list-visual">
            <div class="list-paper">
              <div class="list-header">
                <div class="list-title">My Shopping List</div>
                <div class="list-icon"><i class="pi pi-shopping-cart"></i></div>
              </div>
              <div class="list-items">
                <div class="list-item" v-for="(item, index) in visualItems" :key="index">
                  <div class="item-checkbox" :class="{ 'checked': item.checked }">
                    <i v-if="item.checked" class="pi pi-check"></i>
                  </div>
                  <div class="item-name" :class="{ 'checked': item.checked }">{{ item.name }}</div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div id="features" class="features-section">
      <h2 class="section-title">Why Choose Our App?</h2>
      
      <div class="features-grid">
        <div class="feature-card">
          <div class="feature-icon">
            <i class="pi pi-bolt"></i>
          </div>
          <h3 class="feature-title">Easy to Use</h3>
          <p class="feature-description">
            Simple and intuitive interface designed to make shopping list creation quick and effortless.
          </p>
        </div>
        
        <div class="feature-card">
          <div class="feature-icon">
            <i class="pi pi-users"></i>
          </div>
          <h3 class="feature-title">Collaborative</h3>
          <p class="feature-description">
            Share lists with family and friends to collaborate in real-time on shopping plans.
          </p>
        </div>
        
        <div class="feature-card">
          <div class="feature-icon">
            <i class="pi pi-mobile"></i>
          </div>
          <h3 class="feature-title">Mobile Friendly</h3>
          <p class="feature-description">
            Access your shopping lists anywhere, anytime on any device with our responsive design.
          </p>
        </div>
        
        <div class="feature-card">
          <div class="feature-icon">
            <i class="pi pi-check-circle"></i>
          </div>
          <h3 class="feature-title">Stay Organized</h3>
          <p class="feature-description">
            Keep track of what you need with organized lists that sync across all your devices.
          </p>
        </div>
      </div>
    </div>

    <div class="cta-section" v-if="!authenticatedUser">
      <div class="cta-content">
        <h2 class="cta-title">Ready to get started?</h2>
        <p class="cta-description">Create an account now and start organizing your shopping today.</p>
        <Button 
          label="Sign Up Now" 
          icon="pi pi-user-plus" 
          class="p-button-rounded get-started-btn" 
          @click="navigateTo('/register')" 
        />
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, computed } from 'vue';
import { useRouter } from "vue-router";
import Button from "primevue/button";
import { useAuthStore } from "../store/auth";
import { useShoppingListsStore } from "../store/shoppingLists";

const router = useRouter();
const authStore = useAuthStore();
const shoppingListsStore = useShoppingListsStore();

const authenticatedUser = computed(() => authStore.authenticatedUser);

const userStats = reactive({
  lists: 0,
  groups: 0,
  items: 0
});

const visualItems = ref([
  { name: 'Milk', checked: true },
  { name: 'Eggs', checked: true },
  { name: 'Bread', checked: false },
  { name: 'Apples', checked: false },
  { name: 'Coffee', checked: false },
  { name: 'Cheese', checked: false }
]);

// Simulate checking off items for the animation
let currentItemIndex = 2; // Start with "Bread" (index 2)
const animateChecking = () => {
  if (currentItemIndex < visualItems.value.length) {
    visualItems.value[currentItemIndex].checked = true;
    currentItemIndex++;
    
    // Reset animation if we've reached the end
    if (currentItemIndex >= visualItems.value.length) {
      setTimeout(() => {
        visualItems.value.forEach((item, index) => {
          if (index > 1) { // Keep first two items checked
            item.checked = false;
          }
        });
        currentItemIndex = 2;
      }, 3000);
    }
  }
};

// Periodically check items for animation
const startAnimation = () => {
  setInterval(animateChecking, 2000);
};

const getStarted = () => {
  if (authenticatedUser.value) {
    router.push("/shoppinglists");
  } else {
    router.push("/login");
  }
};

const navigateTo = (path) => {
  router.push(path);
};

const scrollToFeatures = () => {
  document.getElementById('features').scrollIntoView({ 
    behavior: 'smooth' 
  });
};

const fetchUserStats = async () => {
  if (authenticatedUser.value) {
    try {
      // Get user groups
      const groups = await authStore.getUserGroups(true);
      userStats.groups = groups.length;
      
      // Get user shopping lists
      const userGroupIds = groups.map(group => group.id);
      const lists = await shoppingListsStore.fetchAllGroupsShoppingLists(userGroupIds);
      userStats.lists = lists.length;
      
      // Count items (this is an example, adjust based on your API)
      let itemCount = 0;
      for (const list of lists) {
        const items = await shoppingListsStore.getListItems(list.id);
        itemCount += items.length;
      }
      userStats.items = itemCount;
    } catch (error) {
      console.error("Error fetching user stats:", error);
    }
  }
};

onMounted(() => {
  startAnimation();
  fetchUserStats();
});
</script>
