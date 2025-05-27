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
  margin-right: .5rem;
}
</style>
