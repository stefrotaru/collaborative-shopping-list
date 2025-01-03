import { createApp } from 'vue'
import { createPinia } from 'pinia'
import './style.css'
import App from './App.vue'

import PrimeVue from 'primevue/config'
import Aura from '@primevue/themes/aura';

const pinia = createPinia()

createApp(App)
    .use(pinia)
    .use(PrimeVue, {
        theme: {
            preset: Aura
        }
    })
    .mount('#app')
