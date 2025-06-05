<!-- File: frontend/src/components/HistoryModal.vue -->
<template>
  <div class="modal-overlay">
    <div class="modal-container history-container">
      <!-- Крестик “Закрыть” -->
      <button class="close-btn" @click="$emit('close')">✕</button>
      <h2>История ваших тестов</h2>

      <div v-if="loading" class="loading">Загрузка истории…</div>
      <div v-else-if="error" class="error">
        Не удалось загрузить историю. Попробуйте позже.
      </div>
      <div v-else>
        <ul v-if="history.length" class="history-list">
          <li v-for="item in history" :key="item.resultId" class="history-item">
            <div class="hist-main">
              <strong>Тест:</strong> {{ item.testTitle }}
            </div>
            <div class="hist-meta">
              <span><strong>Дата:</strong> {{ formatDate(item.takenAt) }}</span>
              <span><strong>Result ID:</strong> {{ item.resultId }}</span>
            </div>
            <div class="hist-answers">
              <strong>Ваши ответы:</strong>
              <ul class="answers-list">
                <li v-for="(ans, qid) in item.answers" :key="qid">
                  Вопрос {{ qid }} → {{ ans === 0 ? 'Вариант 1' : 'Вариант 2' }}
                </li>
              </ul>
            </div>
            <hr />
          </li>
        </ul>
        <div v-else class="no-history">
          У вас ещё нет завершённых тестов.
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'

const loading = ref(true)
const error = ref(false)
const history = ref([])

onMounted(async () => {
  loading.value = true
  error.value = false

  try {
    // GET /api/history – бэкенд считывает identity из JWT
    const { data } = await axios.get(`http://localhost:5000/api/history`)
    // Ожидаем массив примерно вида:
    // [ { resultId, testTitle, takenAt: ISO, answers: { [questionId]: 0|1 } }, … ]
    history.value = data
  } catch (e) {
    console.error('Не удалось загрузить историю:', e)
    error.value = true
  } finally {
    loading.value = false
  }
})

function formatDate(isoString) {
  const d = new Date(isoString)
  return d.toLocaleString('ru-RU', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.7);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}

.modal-container {
  background-color: var(--main-bg-color);
  border-radius: var(--borderradius);
  padding: 2rem;
  width: 90%;
  max-width: 600px;
  position: relative;
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.30);
  color: var(--h1-color);
  max-height: 80vh;
  overflow-y: auto;
}

.close-btn {
  position: absolute;
  top: 1rem;
  right: 1rem;
  background: transparent;
  border: none;
  font-size: 1.2rem;
  cursor: pointer;
  color: var(--h1-color);
}

.loading,
.error,
.no-history {
  text-align: center;
  margin-top: 1rem;
  color: #ff5555;
}

.history-list {
  list-style: none;
  padding: 0;
  margin: 0;
}

.history-item {
  margin-bottom: 1rem;
}

.hist-main {
  font-size: 1.1rem;
  margin-bottom: 0.5rem;
}

.hist-meta {
  font-size: 0.9rem;
  color: #ccc;
  margin-bottom: 0.5rem;
}

.hist-meta span {
  display: inline-block;
  margin-right: 1rem;
}

.hist-answers {
  margin-bottom: 0.5rem;
}

.answers-list {
  list-style: disc;
  margin-left: 1.5rem;
}
.answers-list li {
  margin-bottom: 0.25rem;
}
</style>
