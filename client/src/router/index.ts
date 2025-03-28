import { createRouter, createWebHistory } from "vue-router";
import type { RouteRecordRaw } from 'vue-router';

import HomePage from "../pages/HomePage.vue";
import LoginPage from "../pages/LoginPage.vue";
import RegisterPage from "../pages/RegistrationPage.vue";
import UserProfilePage from "../pages/UserProfilePage.vue";

import ShoppingLists from "../pages/ShoppingListsPage.vue";
import ShoppingListPage from "../pages/ShoppingListPage.vue";

import GroupPage from "../pages/GroupPage.vue";

import { useAuthStore } from "../store/auth.ts";
import { useAccessibleShoppingListsStore } from "../store/accessibleShoppingLists.ts";
import AccessDeniedPage from "../pages/AccessDeniedPage.vue"; // TODO: create this page

const routes: RouteRecordRaw[] = [
  {
    path: "/",
    redirect: "/home"
  },
  {
    path: "/home",
    name: "Home",
    component: HomePage,
  },
  {
    path: "/login",
    name: "Login",
    component: LoginPage,
  },
  {
    path: "/register",
    name: "Register",
    component: RegisterPage,
  },
  {
    path: "/profile",
    name: "Profile",
    component: UserProfilePage,
  },
  {
    path: "/shoppinglists",
    name: "ShoppingLists",
    component: ShoppingLists,
    meta: { requiresAuth: true }
  },
  {
    path: "/shoppinglists/:id",
    name: "ShoppingList",
    component: ShoppingListPage,
    meta: { requiresAuth: true, checkListAccess: true },
    props: true
  },
  {
    path: "/groups/:id",
    name: "Group",
    component: GroupPage,
    meta: { requiresAuth: true, checkGroupAccess: true },
    props: true
  },
  {
    path: "/access-denied",
    name: "AccessDenied",
    component: AccessDeniedPage
  }
];

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
});

router.beforeEach(async (to, from, next) => {
  const authStore = useAuthStore();
  const publicRoutes = ['/login', '/register', '/access-denied'];

  console.log("⬅️🛣️ Before each route, from:", from, "to:", to);
  
  // Check if user is authenticated for protected routes
  if (!authStore.authenticatedUser && !publicRoutes.includes(to.path)) {
    console.log('🔎🔎🔎check auth needed!')
    await authStore.checkAuth();
  }

  // Redirect to login if not authenticated and route requires auth
  if (to.meta.requiresAuth && !authStore.authenticatedUser) {
    next({ name: 'Login', query: { redirect: to.fullPath } });
    return;
  }

  // Check access for specific shopping list
  if (to.meta.checkListAccess && to.params.id) {
    // Only do this check if the user is authenticated
    if (authStore.authenticatedUser) {
      const accessibleShoppingListsStore = useAccessibleShoppingListsStore();
      const listId = Number(to.params.id);
      
      try {
        const canAccess = await accessibleShoppingListsStore.canAccessShoppingList(listId);
        if (!canAccess) {
          console.log(`Access denied to shopping list ${listId}`);
          next({ name: 'AccessDenied' });
          return;
        }
      } catch (error) {
        console.error('Error checking shopping list access:', error);
        next({ name: 'AccessDenied' });
        return;
      }
    }
  }

  // Check access for specific group
  if (to.meta.checkGroupAccess && to.params.id && authStore.authenticatedUser) {
    const groupId = Number(to.params.id);
    
    // This assumes your useAuthStore has userGroups
    // Or you could load a specific check from your GroupsStore
    const userGroups = await authStore.getUserGroups();
    const hasAccess = userGroups?.some(group => group.id === groupId) ?? false;
    
    // if (!hasAccess) {
    //   console.log(`Access denied to group ${groupId}`);
    //   next({ name: 'AccessDenied' });
    //   return;
    // }
  }

  // Handle redirect to home if already logged in
  if ((to.path === "/login" || to.path === "/register") && authStore.authenticatedUser) {
    next("/home");
  } else {
    next();
  }
});

export default router;