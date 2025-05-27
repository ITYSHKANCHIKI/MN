import { createApp } from 'vue'
import { createPinia } from 'pinia'
import axios from 'axios'
import App from './App.vue'
import router from './router'

import './assets/styles.css'

axios.defaults.baseURL = 'http://localhost:5000'
const pinia = createPinia()
const app = createApp(App)
app.use(pinia)
app.use(router)
app.mount('#app')

// инициализируем токен
import { useAuthStore } from '@/store/auth'
useAuthStore().init()
