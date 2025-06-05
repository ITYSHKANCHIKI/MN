<!-- File: frontend/src/components/ProfilePage.vue -->
<template>
  <div v-if="auth.user" class="profile">
    <!-- Кнопка “Выйти” -->
    <button @click="logout" class="btn-logout">Выйти</button>

    <!-- Статистика по Кольбергу -->
    <div v-if="kohlbergStats" class="kohlberg-stats">
      <h2>Статистика по Кольбергу</h2>
      <p>{{ kohlbergStats }}</p>
    </div>

    <!-- Список доступных тестов -->
    <h2>Доступные тесты</h2>
    <ul class="tests-list">
      <li v-for="t in tests" :key="t.id" class="test-item">
        <span>{{ t.title }}</span>
        <button @click="goToTest(t.id)" class="btn-start">Начать тестирование</button>
      </li>
    </ul>

    <!-- Кнопка “История тестов” -->
    <button @click="showHistory = true" class="btn-history">История тестов</button>

    <!-- ChatModal: открывается, если в query есть showChat=1 -->
    <ChatModal
      v-if="showChat && lastResultId"
      :result-id="lastResultId"
      @close="() => { showChat = false; localStorage.removeItem('lastResultId'); localStorage.removeItem('lastAnswers'); }"
    />

    <!-- HistoryModal: открывается по кнопке “История” -->
    <HistoryModal
      v-if="showHistory"
      @close="showHistory = false"
    />
  </div>

  <div v-else class="loading">
    Загрузка профиля…
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '@/store/auth'
import HistoryModal from '@/components/HistoryModal.vue'
import ChatModal from '@/components/ChatModal.vue'

const auth = useAuthStore()
const router = useRouter()
const route = useRoute()

const tests = ref([])
const showHistory = ref(false)
const kohlbergStats = ref(null)

// Для управления ChatModal
const showChat = ref(false)
const lastResultId = ref(null)

function goToTest(id) {
  router.push(`/test/${id}`)
}

function logout() {
  auth.logout()
  router.push('/auth')
}

function loadTests() {
  // Фейковые данные тестов
  tests.value = [
    { id: 1, title: "Моральные дилеммы" },
    { id: 2, title: "Проверка устойчивости к стрессу" },
    { id: 3, title: "Изучение уровня самосознания" },
    { id: 4, title: "Оценка навыков общения" },
    { id: 5, title: "Анализ творческого потенциала" },
    { id: 6, title: "Определение лидерских способностей" },
  ]
}

function loadKohlbergStats() {
  // Фейковая статистика по Кольбергу
  kohlbergStats.value = "Ваш уровень нравственного развития: 4 из 6 (Конвенциональный уровень)"
}

onMounted(() => {
  if (!auth.isLoggedIn) {
    router.push('/auth')
    return
  }

  loadTests()
  loadKohlbergStats()

  // Проверяем lastResultId в localStorage
  const storedId = localStorage.getItem('lastResultId')
  if (storedId) {
    lastResultId.value = Number(storedId)
    // Если в query есть showChat=1 – открываем сразу ChatModal
    if (route.query.showChat === '1') {
      showChat.value = true
    }
  }
})
</script>

<style scoped>
.profile {
  max-width: 600px;
  margin: 2rem auto;
  padding: 1rem;
  color: #eee;
}
.loading {
  text-align: center;
  padding: 2rem;
  font-size: 1.2rem;
  color: #eee;
}

.btn-logout {
  margin-bottom: 2rem;
  padding: 0.5rem 1rem;
  font-size: 1rem;
  background: transparent;
  border: 2px solid #eee;
  border-radius: 4px;
  color: #eee;
  cursor: pointer;
  transition: background 0.2s, color 0.2s;
}
.btn-logout:hover {
  background: rgba(255, 255, 255, 0.1);
}

.kohlberg-stats {
  margin-bottom: 2rem;
  padding: 1rem;
  background: rgba(0, 0, 0, 0.3);
  border-radius: 8px;
}
.kohlberg-stats h2 {
  margin-bottom: 0.5rem;
  font-size: 1.2rem;
}
.kohlberg-stats p {
  font-size: 1rem;
}

.tests-list {
  list-style: none;
  padding: 0;
  margin-bottom: 2rem;
}
.test-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.75rem;
}
.btn-start {
  padding: 0.4rem 0.8rem;
  background: transparent;
  border: 2px solid #eee;
  border-radius: 4px;
  color: #eee;
  cursor: pointer;
  transition: background 0.2s, color 0.2s;
}
.btn-start:hover {
  background: rgba(255, 255, 255, 0.1);
}

.btn-history {
  padding: 0.75rem 1.5rem;
  background: #2c8d2c;
  border: none;
  border-radius: 4px;
  color: white;
  font-size: 1rem;
  cursor: pointer;
  transition: background 0.2s;
}
.btn-history:hover {
  background: #247c24;
}
</style>