<!-- File: src/components/AuthForm.vue -->
<template>
  <div class="modal-overlay">
    <div class="modal-container">
      <!-- Вкладки «Вход» / «Регистрация» -->
      <div class="auth-tabs">
        <button
          class="tab-btn"
          :class="{ active: mode === 'login' }"
          @click="mode = 'login'"
        >
          <h2>Вход</h2>
        </button>
        <button
          class="tab-btn"
          :class="{ active: mode === 'register' }"
          @click="mode = 'register'"
        >
          <h2>Регистрация</h2>
        </button>
      </div>

      <!-- Содержимое форм -->
      <div class="auth-content">
        <!-- Форма входа -->
        <form v-if="mode === 'login'" @submit.prevent="onSubmit">
          <div class="form-group">
            <label for="login-username">Логин</label>
            <input
              id="login-username"
              type="text"
              v-model="form.username"
              placeholder="Введите логин"
              autocomplete="username"
              required
            />
          </div>
          <div class="form-group">
            <label for="login-password">Пароль</label>
            <input
              id="login-password"
              type="password"
              v-model="form.password"
              placeholder="Введите пароль"
              autocomplete="current-password"
              required
            />
          </div>
          <button type="submit" class="submit-btn">
            Войти
          </button>
        </form>

        <!-- Форма регистрации -->
        <form v-else @submit.prevent="onSubmit">
          <div class="form-group">
            <label for="reg-username">Логин</label>
            <input
              id="reg-username"
              type="text"
              v-model="form.username"
              placeholder="Введите логин"
              autocomplete="username"
              required
            />
          </div>
          <div class="form-group">
            <label for="reg-password">Пароль</label>
            <input
              id="reg-password"
              type="password"
              v-model="form.password"
              placeholder="Введите пароль"
              autocomplete="new-password"
              required
            />
          </div>
          <div class="form-group">
            <label for="reg-confirm-password">Подтвердите пароль</label>
            <input
              id="reg-confirm-password"
              type="password"
              v-model="form.confirmPassword"
              placeholder="Повторите пароль"
              autocomplete="new-password"
              required
            />
          </div>
          <button type="submit" class="submit-btn">
            Зарегистрироваться
          </button>
        </form>

        <!-- Ссылка переключения вкладок -->
        <p class="toggle-link">
          <a href="#" @click.prevent="toggleMode">
            {{ mode === 'login' ? 'Нет профиля? Перейти к регистрации' : 'Уже зарегистрированы? Войти' }}
          </a>
        </p>

        <!-- Текст ошибки -->
        <p v-if="error" class="error-message">{{ error }}</p>
      </div>
    </div>
  </div>
</template>

<script setup>
import { reactive, ref, watch } from 'vue'
import { useAuthStore } from '@/store/auth'
import { useRouter } from 'vue-router'

// Получаем Pinia-стор и рутер
const auth = useAuthStore()
const router = useRouter()

// Режим: 'login' или 'register'
const mode = ref('login')

// Поля формы
const form = reactive({
  username: '',
  password: '',
  confirmPassword: ''
})

// Поле для сообщения об ошибке
const error = ref('')

// При переключении mode сбрасываем форму и ошибку
watch(mode, () => {
  error.value = ''
  form.username = ''
  form.password = ''
  form.confirmPassword = ''
})

// Обработчик сабмита (и вход, и регистрация)
async function onSubmit() {
  error.value = ''
  try {
    if (mode.value === 'register') {
      // Проверка совпадения паролей
      if (form.password !== form.confirmPassword) {
        error.value = 'Пароли не совпадают!'
        return
      }
      // Вызываем Pinia-действие register
      await auth.register({
        username: form.username,
        password: form.password
      })
    } else {
      // Вызываем Pinia-действие login
      await auth.login({
        username: form.username,
        password: form.password
      })
    }

    // Через Pinia-стор уже установится auth.isLoggedIn = true,
    // маршрутизатор (ниже в AuthPage.vue) автоматически переведёт пользователя на /profile.
    // Здесь мы просто вызываем router.push('/profile') на всякий случай:
    router.push('/profile')
  } catch (e) {
    // Показываем серверную ошибку (или дефолт)
    error.value = e.response?.data ?? 'Не удалось выполнить запрос'
  }
}

// Переключение вкладок вручную (ссылка)
function toggleMode() {
  mode.value = mode.value === 'login' ? 'register' : 'login'
}
</script>

<style scoped>
/* Затемнённый фон */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.6);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}

/* Контейнер формы */
.modal-container {
  background-color: var(--main-bg-color);
  border-radius: var(--borderradius);
  padding: 2rem;
  width: 40vh;
  max-width: 90%;
  position: relative;
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.3);
  color: var(--h1-color);
}

/* Вкладки */
.auth-tabs {
  display: flex;
  margin-bottom: 1.5rem;
  border-bottom: 1px solid #ccc;
}

.tab-btn {
  flex: 1;
  padding: 0.75rem 1rem;
  background: none;
  border: none;
  cursor: pointer;
  font-size: 1.1rem;
  color: var(--color-dark-gray);
  position: relative;
  transition: color 0.2s;
}

.tab-btn h2 {
  margin: 0;
}

.tab-btn.active {
  color: var(--color-green);
  font-weight: bold;
}

.tab-btn.active::after {
  content: '';
  position: absolute;
  bottom: -2px;
  left: 0;
  right: 0;
  height: 3px;
  background-color: var(--color-green);
}

/* Контент форм */
.auth-content {
  font-size: var(--parsize);
}

.form-group {
  margin-bottom: 1.5rem;
}

.form-group label {
  display: block;
  margin-bottom: 0.5rem;
  color: var(--color-dark-gray);
  font-size: var(--parsize);
}

.form-group input {
  width: 100%;
  box-sizing: border-box;
  padding: 0.75rem;
  border: 1px solid var(--color-dark-gray);
  border-radius: var(--borderradius);
  font-size: var(--parsize);
  background-color: var(--color-white);
  color: var(--h1-color);
}

/* Кнопка «Войти» / «Зарегистрироваться» */
.submit-btn {
  width: 100%;
  padding: 0.75rem;
  background-color: var(--color-green);
  color: white;
  border: none;
  border-radius: var(--borderradius);
  cursor: pointer;
  font-size: var(--parsize);
  transition: background-color 0.2s;
}

.submit-btn:hover {
  background-color: var(--color-dark-green);
}

/* Ссылка переключения вкладок */
.toggle-link {
  margin-top: 1rem;
  text-align: center;
  font-size: var(--parsize);
}

.toggle-link a {
  color: var(--color-green);
  text-decoration: none;
  transition: opacity 0.2s;
}

.toggle-link a:hover {
  opacity: 0.8;
}

/* Текст ошибки */
.error-message {
  margin-top: 1rem;
  color: var(--color-red);
  font-size: var(--parsize);
  text-align: center;
}
</style>
