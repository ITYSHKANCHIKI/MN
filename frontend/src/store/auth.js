// File: frontend/src/store/auth.js

import { defineStore } from 'pinia'
import axios from 'axios'
import jwt_decode from 'jwt-decode'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    token: null,
    user: null
  }),
  getters: {
    isLoggedIn: (state) => !!state.token
  },
  actions: {
    init() {
      const t = localStorage.getItem('token')
      if (t) {
        this.token = t
        axios.defaults.headers.common['Authorization'] = `Bearer ${t}`
        const decoded = jwt_decode(t)
        this.user = {
          id: parseInt(decoded.id || decoded.nameid || 0),
          username: decoded.unique_name || decoded.name || decoded.sub
        }
      }
    },

    async register(dto) {
      // dto должен содержать { username, password, confirmPassword }
      const response = await axios.post('/api/auth/register', dto)
      // Ожидаем, что бэкенд вернет { access_token: "..." }
      const token = response.data.access_token
      if (token) {
        localStorage.setItem('token', token)
        this.token = token
        axios.defaults.headers.common['Authorization'] = `Bearer ${token}`
        const decoded = jwt_decode(token)
        this.user = {
          id: parseInt(decoded.id || decoded.nameid || 0),
          username: decoded.unique_name || decoded.name || decoded.sub
        }
      } else {
        // Если по какой-то причине токена нет — кидаем ошибку
        throw new Error(response.data.error || 'Не удалось зарегистрироваться')
      }
    },

    async login(dto) {
      // dto должен содержать { username, password }
      const response = await axios.post('/api/auth/login', dto)
      // Ожидаем, что бэкенд вернет { access_token: "..." }
      const token = response.data.access_token
      if (token) {
        localStorage.setItem('token', token)
        this.token = token
        axios.defaults.headers.common['Authorization'] = `Bearer ${token}`
        const decoded = jwt_decode(token)
        this.user = {
          id: parseInt(decoded.id || decoded.nameid || 0),
          username: decoded.unique_name || decoded.name || decoded.sub
        }
      } else {
        throw new Error(response.data.error || 'Неверный логин или пароль')
      }
    },

    logout() {
      this.token = null
      this.user = null
      localStorage.removeItem('token')
      delete axios.defaults.headers.common['Authorization']
    }
  }
})
