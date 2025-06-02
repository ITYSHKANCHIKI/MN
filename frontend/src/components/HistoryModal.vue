<template>
  <div class="modal-backdrop" @click.self="$emit('close')">
    <div class="modal">
      <h3>Ваша история</h3>
      <ul>
        <li v-for="h in history" :key="h.testId + h.takenAt">
          Тест #{{ h.testId }} – {{ new Date(h.takenAt).toLocaleString() }} – Баллы: {{ h.score }}
        </li>
      </ul>
      <button @click="$emit('close')">Закрыть</button>
    </div>
  </div>
</template>

<script>
import { ref, onMounted } from 'vue'
import axios from 'axios'

export default {
  setup() {
    const history = ref([])
    onMounted(async () => {
      const { data } = await axios.get('/api/history')
      history.value = data
    })
    return { history }
  }
}
</script>

<style>
.modal-backdrop {
  position: fixed; inset: 0; background: rgba(0,0,0,0.5);
  display: flex; justify-content: center; align-items: center;
}
.modal {
  background: rgb(7, 19, 3); padding: 1.5rem; border-radius: 8px; width: 300px;
}
</style>
