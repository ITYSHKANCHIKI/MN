import { createRouter, createWebHistory } from 'vue-router'
import AuthForm from '@/components/AuthForm.vue'
import ProfilePage from '@/components/ProfilePage.vue'
import TestPage from '@/components/TestPage.vue'
import { useAuthStore } from '@/store/auth'

const routes = [
  { path: '/auth', component: AuthForm },
  { 
    path: '/profile', 
    component: ProfilePage,
    meta: { requiresAuth: true }
  },
  { 
    path: '/test/:id', 
    component: TestPage,
    meta: { requiresAuth: true }
  },
  { path: '/:pathMatch(.*)*', redirect: '/auth' }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

// Навигационный guard
router.beforeEach((to, from, next) => {
  const auth = useAuthStore()
  if (to.meta.requiresAuth && !auth.isLoggedIn) {
    return next('/auth')
  }
  if (to.path === '/auth' && auth.isLoggedIn) {
    return next('/profile')
  }
  next()
})

export default router
