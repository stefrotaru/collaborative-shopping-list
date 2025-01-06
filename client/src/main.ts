import { createApp } from 'vue'
import { createPinia } from 'pinia'
import './style.scss'
import router from './router'
import App from './App.vue'

import PrimeVue from 'primevue/config'
import Aura from '@primevue/themes/aura';
import ToastService from 'primevue/toastservice';

const pinia = createPinia()

const app = createApp(App);

app
  .use(pinia)
  .use(router)
  .use(PrimeVue, {
      theme: {
          preset: Aura
      }
  })
  .use(ToastService)
  .mount('#app')
