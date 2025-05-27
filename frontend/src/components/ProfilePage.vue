<template>
  <div v-if="auth.user" class="profile">
    <h1>Привет, {{ auth.user.username }}!</h1>
    <button @click="logout">Выйти</button>

    <h2>Доступные тесты</h2>
    <ul>
      <li v-for="t in tests" :key="t.id">
        {{ t.title }}
        <button @click="goToTest(t.id)">Начать</button>
      </li>
    </ul>

    <button @click="showHistory = true">История тестов</button>
    <HistoryModal v-if="showHistory" @close="showHistory = false" />
  </div>

  <div v-else class="loading">
    Загрузка профиля…
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useAuthStore } from '@/store/auth'
import axios from 'axios'
import { useRouter } from 'vue-router'
import HistoryModal from './HistoryModal.vue'

const auth = useAuthStore()
const router = useRouter()

const tests = ref([])
const showHistory = ref(false)

function goToTest(id) {
  router.push(`/test/${id}`)
}

function logout() {
  auth.logout()
  router.push('/auth')
}

async function loadTests() {
  try {
    const { data } = await axios.get('/api/tests')
    tests.value = data
  } catch (err) {
    console.error('Не удалось загрузить тесты:', err)
  }
}

onMounted(() => {
  if (!auth.isLoggedIn) {
    router.push('/auth')
    return
  }
  // подгружаем список тестов
  loadTests()
})
</script>

<style scoped>
.profile {
  max-width: 600px;
  margin: auto;
  padding: 1rem;
}
.loading {
  text-align: center;
  padding: 2rem;
  font-size: 1.2rem;
}
button {
  margin: 0.5rem 0;
}
ul {
  list-style: none;
  padding: 0;
}
li {
  margin-bottom: 0.5rem;
}
</style>
