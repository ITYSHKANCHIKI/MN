// File: src/store/auth.js
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
          id: decoded.nameid || decoded.sub,
          username: decoded.unique_name || decoded.name
        }
      }
    },
    async register(dto) {
      const { data } = await axios.post('/api/auth/register', dto)
      if (data.success) {
        localStorage.setItem('token', data.token)
        this.init()
      } else {
        throw new Error(data.error)
      }
    },
    async login(dto) {
      const { data } = await axios.post('/api/auth/login', dto)
      if (data.success) {
        localStorage.setItem('token', data.token)
        this.init()
      } else {
        throw new Error(data.error)
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
