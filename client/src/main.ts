import { createApp } from 'vue'
import { createPinia } from 'pinia'
import './style.scss'
import router from './router'
import App from './App.vue'

import PrimeVue from 'primevue/config'
import Aura from '@primevue/themes/aura';

const pinia = createPinia()

createApp(App)
    .use(pinia)
    .use(router)
    .use(PrimeVue, {
        theme: {
            preset: Aura
        }
    })
    .mount('#app')
