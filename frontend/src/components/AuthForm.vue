<template>
  <div>
    <h1>{{ mode === 'login' ? 'Вход' : 'Регистрация' }}</h1>
    <form @submit.prevent="onSubmit">
      <input
        v-model="form.username"
        placeholder="Логин"
        required
      />
      <input
        v-model="form.password"
        type="password"
        placeholder="Пароль"
        required
      />
      <button type="submit">
        {{ mode === 'login' ? 'Войти' : 'Зарегистрироваться' }}
      </button>
    </form>
    <p>
      <a href="#" @click.prevent="toggleMode">
        Перейти к {{ mode === 'login' ? 'регистрации' : 'входу' }}
      </a>
    </p>
    <p v-if="error" style="color: red">{{ error }}</p>
  </div>
</template>

<script setup>
import { reactive, ref } from 'vue'
import { useAuthStore } from '@/store/auth'
import { useRouter } from 'vue-router'

const auth = useAuthStore()
const router = useRouter()

// режим — 'login' или 'register'
const mode = ref('register')
const form = reactive({ username: '', password: '' })
const error = ref('')

function toggleMode() {
  mode.value = mode.value === 'login' ? 'register' : 'login'
}

async function onSubmit() {
  error.value = ''
  try {
    if (mode.value === 'register') {
      await auth.register(form)
    } else {
      await auth.login(form)
    }
    router.push('/profile')
  } catch (err) {
    error.value = err.response?.data ?? 'Не удалось выполнить запрос'
  }
}
</script>
