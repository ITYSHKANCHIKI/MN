// File: src/main.js
import { createApp } from 'vue';
import { createPinia } from 'pinia';
import App from './App.vue';
import router from './router';
import axios from 'axios';

import './assets/styles.css';

// DEV-флаг от Vite
const isDev = import.meta.env.DEV;

// В Dev: полностью на localhost:5000, в Prod: относительные пути
axios.defaults.baseURL = isDev
  ? 'http://localhost:5000'
  : '';
axios.defaults.headers.common['Content-Type'] = 'application/json';

const app = createApp(App);
const pinia = createPinia();

app.use(pinia);
app.use(router);

// Если вы хотите, чтобы в компонентах можно было делать this.$axios
app.config.globalProperties.$axios = axios;

// ↓↓↓ Инициализируем авторизацию ДО того, как приложение будет примонтировано ↓↓↓
import { useAuthStore } from './store/auth';
const auth = useAuthStore();
auth.init();

// ↑↑↑ Только теперь монтируем Vue-приложение ↑↑↑
app.mount('#app');
