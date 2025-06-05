// File: frontend/src/router/index.js

import { createRouter, createWebHistory } from 'vue-router'
import AuthPage from '@/views/AuthPage.vue'
import ProfilePage from '@/components/ProfilePage.vue'
import TestPage from '@/components/TestPage.vue'
import { useAuthStore } from '@/store/auth'

const routes = [
  {
    path: '/auth',
    name: 'Auth',
    component: AuthPage
  },
  {
    path: '/profile',
    name: 'Profile',
    component: ProfilePage,
    meta: { requiresAuth: true }
  },
  {
    path: '/test/:id',
    name: 'Test',
    component: TestPage,
    meta: { requiresAuth: true }
  },
  {
    // «Ловим» любые неизвестные пути и кидаем на /auth
    path: '/:pathMatch(.*)*',
    redirect: '/auth'
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

// Навигационный guard
router.beforeEach((to, from, next) => {
  const auth = useAuthStore()
  // Если рут требует авторизации, а пользователь не залогинен → на /auth
  if (to.meta.requiresAuth && !auth.isLoggedIn) {
    return next('/auth')
  }
  // Если рут — /auth, но уже залогинены → на /profile
  if (to.path === '/auth' && auth.isLoggedIn) {
    return next('/profile')
  }
  next()
})

export default router
