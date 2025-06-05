<!-- File: frontend/src/components/TestPage.vue -->
<template>
  <div class="test-page">
    <h2 class="test-title">{{ test.title }}</h2>

    <div v-if="loading" class="loading">Загрузка…</div>
    <div v-else-if="errorLoading" class="error">Ошибка при загрузке теста.</div>

    <div v-else class="content">
      <!-- Если есть вопросы -->
      <div v-if="test.questions && test.questions.length" class="question-block">
        <p class="question-text">{{ currentQuestion.text }}</p>

        <div class="polaroid">
          <img v-if="idx === 0" src="/images/Хайнц.png" alt="Heinz dilemma" class="question-image" />
          <img v-else-if="idx === 1" src="/images/самолет.png" alt="Plane dilemma" class="question-image" />
          <img v-else-if="idx === 2" src="/images/аборт.png" alt="Abort dilemma" class="question-image" />
          <img v-else-if="idx === 3" src="/images/толстяк.png" alt="Cave dilemma" class="question-image" />
          <img v-else-if="idx === 4" src="/images/вагонетка.png" alt="Trolley dilemma" class="question-image" />
        </div>

        <div class="answer-buttons">
          <button
            class="left-btn"
            :class="{ selected: answers[currentQuestion.id] === 0 }"
            @click="selectAnswer(0)"
          >
            {{ currentQuestion.options[0] }}
          </button>
          <button
            class="right-btn"
            :class="{ selected: answers[currentQuestion.id] === 1 }"
            @click="selectAnswer(1)"
          >
            {{ currentQuestion.options[1] }}
          </button>
        </div>

        <button
          class="submit-btn"
          :disabled="answers[currentQuestion.id] == null"
          @click="nextOrSubmit"
        >
          {{ isLast ? 'Завершить' : 'Далее' }}
        </button>
      </div>

      <div v-else class="no-questions">
        <p>В этом тесте нет вопросов.</p>
      </div>
    </div>

    <div v-if="errorSubmit" class="error">
      Не удалось отправить ответы. Пожалуйста, попробуйте ещё раз.
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import axios from 'axios'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/store/auth'

const route = useRoute()
const router = useRouter()
const auth = useAuthStore()

// ID теста из URL, приведённый к числу
const testId = Number(route.params.id)

const test = ref({ id: 0, title: '', questions: [] })
const answers = ref({})
const idx = ref(0)
const loading = ref(true)
const errorLoading = ref(false)
const errorSubmit = ref(false)

onMounted(async () => {
  loading.value = true
  errorLoading.value = false
  try {
    // Берём структуру теста (вопросы, варианты)
    const { data } = await axios.get(`http://localhost:5000/api/tests/${testId}`)
    test.value = data
  } catch (e) {
    console.error('Не удалось загрузить тест:', e)
    errorLoading.value = true
  } finally {
    loading.value = false
  }
})

const currentQuestion = computed(() => test.value.questions[idx.value])
const isLast = computed(() => idx.value === test.value.questions.length - 1)

function selectAnswer(optionIndex) {
  answers.value = { ...answers.value, [currentQuestion.value.id]: optionIndex }
}

async function nextOrSubmit() {
  if (!isLast.value) {
    idx.value++
  } else {
    errorSubmit.value = false

    // Если вдруг не авторизован – кидаем на страницу авторизации
    if (!auth.isLoggedIn) {
      router.push('/auth')
      return
    }

    // Собираем payload только с полем answers
    const payload = {
      answers: answers.value
    }

    try {
      // POST /api/tests/{testId} с body = { answers: { [questionId]: 0|1 } }
      const { data } = await axios.post(
        `http://localhost:5000/api/tests/${testId}`,
        payload
      )
      const resultId = data.resultId

      // Запомним в localStorage, чтобы потом показать ChatModal
      localStorage.setItem('lastResultId', resultId.toString())
      localStorage.setItem('lastAnswers', JSON.stringify(answers.value))

      // Перенаправляем на профиль с параметром, чтобы ChatModal открылся сразу
      router.push({ path: '/profile', query: { showChat: '1' } })
    } catch (e) {
      console.error('Ошибка отправки ответов:', e)
      errorSubmit.value = true
    }
  }
}
</script>

<style scoped>
.test-page {
  max-width: 600px;
  margin: 2rem auto;
  background: rgba(0, 0, 0, 0.6);
  padding: 1.5rem;
  border-radius: 8px;
  color: #eee;
}

.loading,
.error,
.no-questions {
  text-align: center;
  padding: 1rem 0;
}

.test-title {
  text-align: center;
  margin-bottom: 1rem;
  font-size: 1.5rem;
}

.question-block {
  display: flex;
  flex-direction: column;
  align-items: center;
}

.question-text {
  font-size: 1.2rem;
  margin-bottom: 0.5rem;
  text-align: center;
}

.polaroid {
  background: #fff;
  padding: 0.5rem 0.5rem 3rem;
  border-radius: 4px;
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.5);
  margin: 1rem auto;
  max-width: 390px;
  width: 100%;
}
.polaroid img {
  display: block;
  width: 100%;
  height: auto;
  border-radius: 2px;
}

.answer-buttons {
  display: flex;
  gap: 1rem;
  margin-bottom: 1rem;
  width: 100%;
}
.answer-buttons button {
  flex: 1;
  padding: 0.75rem;
  background: transparent;
  border: 2px solid #555;
  font-size: 1rem;
  color: #eee;
  border-radius: 4px;
  cursor: pointer;
  transition: all 0.2s;
}
.answer-buttons .left-btn:hover {
  border-color: red;
  background: rgba(255, 0, 0, 0.2);
}
.answer-buttons .right-btn:hover {
  border-color: blue;
  background: rgba(0, 0, 255, 0.2);
}
.answer-buttons button.selected {
  background: rgba(0, 255, 0, 0.2);
  border-color: #0f0;
}

.submit-btn {
  width: 100%;
  padding: 1rem;
  font-size: 1.1rem;
  background: rgba(0, 255, 0, 0.2);
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}
.submit-btn:disabled {
  background: #555;
  cursor: not-allowed;
}

.error {
  margin-top: 1rem;
  color: #ff5555;
}
</style>
