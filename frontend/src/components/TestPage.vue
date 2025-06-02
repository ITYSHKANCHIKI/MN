<template>
  <div class="test-page">
    <h2 class="test-title">{{ test.title }}</h2>

    <div v-if="test.questions.length" class="question-block">
      <p class="question-text">{{ currentQuestion.text }}</p>

      <div class="polaroid">
        <img
          v-if="currentQuestion.id === 1"
          src="/images/Хайнц.png"
          alt="Heinz dilemma"
          class="question-image"
        />
        <img
          v-else-if="currentQuestion.id === 2"
          src="/images/самолет.png"
          alt="Plane dilemma"
          class="question-image"
        />
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
const testId = route.params.id

const test = ref({ title: '', questions: [] })
const answers = ref({})       // { [questionId]: selectedIndex }
const idx = ref(0)            // индекс текущего вопроса

onMounted(async () => {
  try {
    const { data } = await axios.get(`/api/tests/${testId}`)
    test.value = data
  } catch (e) {
    console.error('Не удалось загрузить тест:', e)
  }
})

const currentQuestion = computed(() => test.value.questions[idx.value])
const isLast = computed(() => idx.value === test.value.questions.length - 1)

function selectAnswer(optionIndex) {
  answers.value = {
    ...answers.value,
    [currentQuestion.value.id]: optionIndex
  }
}

async function nextOrSubmit() {
  if (!isLast.value) {
    idx.value++
  } else {
    // собираем и шлём на бэкенд
    try {
      await axios.post(`/api/tests/${testId}`, {
        userId: auth.user.id,
        answers: answers.value
      })
      router.push('/profile')
    } catch (e) {
      console.error('Ошибка отправки ответов:', e)
      alert('Не удалось отправить ответы.')
    }
  }
}
</script>

<style scoped>
/* Блок страницы */
.test-page {
  max-width: 600px;
  margin: 2rem auto;
  background: rgba(0, 0, 0, 0.6);
  padding: 1.5rem;
  border-radius: 8px;
  color: #eee;
}
/* Заголовок */
.test-title {
  text-align: center;
  margin-bottom: 1rem;
  font-size: 1.5rem;
}
/* Текст вопроса */
.question-text {
  font-size: 1.2rem;
  margin-bottom: 0.5rem;
}
/* Полароид */
.polaroid {
  display: inline-block;
  background: #fff;
  padding: 0.5rem 0.5rem 2.5rem;
  border-radius: 4px;
  box-shadow: 0 8px 20px rgba(0,0,0,0.5);
  transform: rotate(0deg);
  margin: 1rem 0;
}
.polaroid img {
  display: block;
  max-width: 100%;
  border-radius: 2px;
}
/* Кнопки-ответы */
.answer-buttons {
  display: flex;
  gap: 1rem;
  margin-bottom: 1rem;
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
/* Hover */
.answer-buttons .left-btn:hover {
  border-color: red;
  background: rgba(255,0,0,0.2);
}
.answer-buttons .right-btn:hover {
  border-color: blue;
  background: rgba(0,0,255,0.2);
}
/* Выбранный ответ */
.answer-buttons button.selected {
  background: rgba(0,255,0,0.2);
  border-color: #0f0;
}
/* Кнопка Далее/Завершить */
.submit-btn {
  width: 100%;
  padding: 1rem;
  font-size: 1.1rem;
  background: rgba(0,255,0,0.2);
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}
.submit-btn:disabled {
  background: #555;
  cursor: not-allowed;
}
</style>
