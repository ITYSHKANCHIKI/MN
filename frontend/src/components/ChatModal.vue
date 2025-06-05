<template>
  <div class="chat-overlay" @click.self="close">
    <div class="chat-modal">
      <header class="chat-header">
        <h3>GPT-Психолог</h3>
        <button class="close-btn" @click="close">✕</button>
      </header>
      <div ref="chatWindow" class="chat-body">
        <div v-for="(msg, idx) in messages" :key="idx" :class="['message', msg.role]">
          <div class="role">
            <span v-if="msg.role === 'system'">Система</span>
            <span v-else-if="msg.role === 'user'">Вы</span>
            <span v-else>Психолог</span>
          </div>
          <div class="content">{{ msg.content }}</div>
        </div>
      </div>
      <footer class="chat-footer">
        <textarea
          v-model="userInput"
          placeholder="Напишите сообщение..."
          @keydown.enter.prevent="sendUserMessage"
        ></textarea>
        <button :disabled="isLoading || !userInput.trim()" @click="sendUserMessage">Отправить</button>
      </footer>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, nextTick } from 'vue';
import axios from 'axios';
import { useAuthStore } from '@/store/auth';

const props = defineProps({
  resultId: {
    type: Number,
    required: true
  }
});
const emit = defineEmits(['close']);

const auth = useAuthStore();

/**
 * Array of chat messages. Each item has the shape
 * { role: 'system' | 'user' | 'assistant', content: string }
 */
const messages = ref([]);
const userInput = ref('');
const isLoading = ref(false);
const chatWindow = ref(null);

onMounted(async () => {
  if (!auth.user?.id) {
    emit('close');
    return;
  }

  messages.value = [];
  try {
    isLoading.value = true;
    const payload = {
      resultId: props.resultId,
      userId: auth.user.id
    };
    const { data } = await axios.post('/api/chat/initiate', payload);
    messages.value.push({ role: 'system', content: 'Чат с психологом открыт.' });
    messages.value.push({ role: 'assistant', content: data.botMessage });
    await scrollToBottom();

    const stored = localStorage.getItem('lastAnswers');
    if (stored) {
      try {
        const obj = JSON.parse(stored);
        const text = Object.entries(obj)
          .map(([q, a]) => `Вопрос ${q}: вариант ${Number(a) + 1}`)
          .join('\n');
        await sendUserMessage(text);
      } catch (err) {
        console.error('Не удалось отправить сохранённые ответы:', err);
      }
    }    
  } catch (e) {
    console.error('Ошибка инициации чата:', e);
    messages.value.push({ role: 'assistant', content: 'Ошибка при инициации чата.' });
    await scrollToBottom();
  } finally {
    isLoading.value = false;
  }
});

async function sendUserMessage(customText) {
  const text = (customText ?? userInput.value).trim();
  if (!text) return;

  messages.value.push({ role: 'user', content: text });
  if (!customText) {
    userInput.value = '';
  }
  await scrollToBottom();

  const payloadMessages = messages.value.map(m => ({
    role: m.role,
    content: m.content
  }));

  try {
    isLoading.value = true;
    const payload = {
      resultId: props.resultId,
      userId: auth.user.id,
      messages: payloadMessages
    };
    const { data } = await axios.post('/api/chat/continue', payload);
    messages.value.push({ role: 'assistant', content: data.botMessage });
    await scrollToBottom();
  } catch (e) {
    console.error('Ошибка при продолжении чата:', e);
    messages.value.push({ role: 'assistant', content: 'Ошибка связи с сервером.' });
    await scrollToBottom();
  } finally {
    isLoading.value = false;
  }
}

function close() {
  emit('close');
}

async function scrollToBottom() {
  await nextTick();
  if (chatWindow.value) {
    chatWindow.value.scrollTop = chatWindow.value.scrollHeight;
  }
}
</script>

<style scoped>
.chat-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.5);
  z-index: 1000;
  display: flex;
  justify-content: center;
  align-items: center;
}

.chat-modal {
  background: white;
  width: 90%;
  max-width: 600px;
  height: 80%;
  display: flex;
  flex-direction: column;
  border-radius: 8px;
  overflow: hidden;
}

.chat-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  background: #2c8d2c;
  color: white;
}

.close-btn {
  background: none;
  border: none;
  color: white;
  font-size: 1.2rem;
  cursor: pointer;
}

.chat-body {
  flex: 1;
  padding: 1rem;
  overflow-y: auto;
  background: #f5f5f5;
}

.message {
  margin-bottom: 1rem;
  max-width: 80%;
  white-space: pre-line;
}

.message.system {
  align-self: center;
  background: #e0e0e0;
  padding: 0.5rem;
  border-radius: 4px;
  font-style: italic;
}

.message.user {
  align-self: flex-end;
  background: #d1e7dd;
  border-radius: 8px 8px 0 8px;
  padding: 0.5rem;
}

.message.assistant {
  align-self: flex-start;
  background: #fff3cd;
  border-radius: 8px 8px 8px 0;
  padding: 0.5rem;
}

.role {
  font-size: 0.75rem;
  opacity: 0.7;
  margin-bottom: 0.25rem;
}

.content {
  white-space: pre-wrap;
}

.chat-footer {
  padding: 0.5rem;
  display: flex;
  gap: 0.5rem;
  background: white;
  border-top: 1px solid #ccc;
}

.chat-footer textarea {
  flex: 1;
  resize: none;
  height: 3rem;
  padding: 0.5rem;
  border: 1px solid #ccc;
  border-radius: 4px;
}

.chat-footer button {
  background: #2c8d2c;
  color: white;
  border: none;
  padding: 0 1rem;
  cursor: pointer;
  border-radius: 4px;
}
.chat-footer button:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}
</style>
