import { createRouter, createWebHistory } from 'vue-router';
import HomePage from '../pages/HomePage.vue';
import LoginPage from '../pages/LoginPage.vue';
import RegisterPage from '../pages/RegistrationPage.vue';
import UserProfilePage from '../pages/UserProfilePage.vue';

import ShoppingLists from '../pages/ShoppingListsPage.vue';
import ShoppingListPage from '../pages/ShoppingListPage.vue';

const routes = [
  {
    path: '/home',
    name: 'Home',
    component: HomePage
  },
  {
    path: '/login',
    name: 'Login',
    component: LoginPage
  },
  {
    path: '/register',
    name: 'Register',
    component: RegisterPage
  },
  {
    path: '/profile',
    name: 'Profile',
    component: UserProfilePage,
  },
  {
    path: '/shoppinglists',
    name: 'ShoppingLists',
    component: ShoppingLists,
  },
  {
    path: '/shoppinglists/:id',
    name: 'ShoppingList',
    component: ShoppingListPage,
  },
];

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
});

export default router;