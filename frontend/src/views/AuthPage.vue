<!-- File: src/views/AuthPage.vue -->
<template>
  <!-- /auth → здесь всегда рендерится форма входа/регистрации -->
  <AuthForm />
</template>

<script setup>
import AuthForm from '@/components/AuthForm.vue'
import { useAuthStore } from '@/store/auth'
import { useRouter } from 'vue-router'
import { watch } from 'vue'

// Получаем Pinia-стор и маршрутизатор
const auth = useAuthStore()
const router = useRouter()

// Если уже вошли, сразу перекидываем на /profile
if (auth.isLoggedIn) {
  router.replace('/profile')
}

// Смотрим за изменением auth.isLoggedIn. Как только станет true → навигируем
watch(
  () => auth.isLoggedIn,
  (newVal) => {
    if (newVal) {
      router.replace('/profile')
    }
  }
)
</script>

<style>
/* Здесь можно ничего не писать, фон страницы задаётся глобально */
</style>
