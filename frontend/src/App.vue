<!-- File: src/App.vue -->
<template>
  <div id="app">
    <nav>
      <router-link to="/auth">Вход/Рег</router-link> |
      <router-link v-if="auth.isLoggedIn" to="/profile">Профиль</router-link>
    </nav>
    <router-view />
  </div>
</template>

<script>
import { useAuthStore } from '@/store/auth'

export default {
  setup() {
    const auth = useAuthStore()
    // При загрузке приложения считаем, есть ли в localStorage токен
    auth.init()
    return { auth }
  }
}
</script>

<style>
nav {
  padding: 1rem;
  background: #f5f5f5;
  margin-bottom: 1rem;
}
nav a {
  margin-right: 0.5rem;
  text-decoration: none;
  color: #333;
}
nav a.router-link-active {
  font-weight: bold;
}
</style>
