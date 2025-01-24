import { createRouter, createWebHistory } from "vue-router";
import HomePage from "../pages/HomePage.vue";
import LoginPage from "../pages/LoginPage.vue";
import RegisterPage from "../pages/RegistrationPage.vue";
import UserProfilePage from "../pages/UserProfilePage.vue";

import ShoppingLists from "../pages/ShoppingListsPage.vue";
import ShoppingListPage from "../pages/ShoppingListPage.vue";

import { useAuthStore } from "../store/auth.js";
// import { useGlobalStore } from "../store/global.js";
import { useShoppingListsStore } from "../store/shoppingLists.js";

const routes = [
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
  },
  {
    path: "/shoppinglists/:id",
    name: "ShoppingList",
    component: ShoppingListPage,
  },
];

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
});

router.beforeEach(async (to, from, next) => {
  const authStore = useAuthStore();
  const publicRoutes = ['/login', '/register'];
  
  if (!authStore.authenticatedUser && !publicRoutes.includes(to.path)) {
    console.log('🔎🔎🔎check auth needed!')
    await authStore.checkAuth();
  }

  console.log("change route");
  console.log(authStore.authenticatedUser);

  if (
    to.path === "/login" ||
    (to.path === "/register" && authStore.authenticatedUser)
  ) {
    next("/home");
  } else {
    next();
  }
});

export default router;
