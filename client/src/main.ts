import { createApp } from 'vue'
import { createPinia } from 'pinia'
import { indexedDBService } from './services/indexedDbService'
import './scss/style.scss'
import router from './router'
import App from './App.vue'

import PrimeVue from 'primevue/config'
import Aura from '@primevue/themes/aura'
import 'primeicons/primeicons.css'

import ToastService from 'primevue/toastservice';

// Initialize indexedDB if supported by browser
if (indexedDBService.isSupported()) {
  await indexedDBService.init();
  console.log('IndexedDB ready!');
} else {
  console.error('IndexedDB not supported');
}

const pinia = createPinia()

const app = createApp(App);

app
  .use(pinia)
  .use(router)
  .use(PrimeVue, {
      theme: {
          preset: Aura,
          options: {
            prefix: 'p',
            darkModeSelector: 'system',
            cssLayer: {
                name: 'primevue',
                order: 'tailwind-base, primevue, tailwind-utilities'
            }
        }
    }
  })
  .use(ToastService)
  .mount('#app')
