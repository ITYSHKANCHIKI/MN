// File: frontend/src/main.js

import { createApp } from 'vue';
import { createPinia } from 'pinia';
import App from './App.vue';
import router from './router';
import axios from 'axios';

import './assets/styles.css';

// DEV‐флаг от Vite
const isDev = import.meta.env.DEV;

// В режиме DEV — оставляем пустую строку, чтобы Vite проксировал `/api` → http://localhost:5000
// В режиме PROD — явно указываем http://localhost:5000, потому что фронтенд в докере
// слушает 3000 порт, а бэкенд — 5000
axios.defaults.baseURL = isDev
  ? ''
  : 'http://localhost:5000';

axios.defaults.headers.common['Content-Type'] = 'application/json';

const app = createApp(App);
const pinia = createPinia();

app.use(pinia);
app.use(router);

// Если вы хотите внутри компонентов писать `this.$axios`
app.config.globalProperties.$axios = axios;

// ─── Важно: инициализируем авторизацию ДО того, как приложение примонтируется ───
import { useAuthStore } from './store/auth';
const auth = useAuthStore();
auth.init();

// ──────────────────────────────────────────────────────────────────────────────
// Теперь монтируем Vue-приложение
app.mount('#app');
