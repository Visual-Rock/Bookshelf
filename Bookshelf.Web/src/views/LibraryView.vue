<template>
  <div class="p-4 sm:p-6 lg:p-8">
    <div v-if="loading" class="flex flex-col items-center justify-center mt-20 text-slate-500 dark:text-zinc-400">
      <span class="material-icons animate-spin text-4xl mb-4">refresh</span>
      <p class="text-lg">Loading bookshelf...</p>
    </div>

    <div v-else-if="error" class="flex flex-col items-center justify-center mt-20 text-center">
      <span class="material-icons text-4xl mb-4 text-slate-300 dark:text-zinc-600">wifi_off</span>
      <p class="text-base font-medium text-slate-600 dark:text-zinc-400 mb-1">Failed to load bookshelf</p>
      <p class="text-sm text-slate-400 dark:text-zinc-600 mb-4">{{ error }}</p>
      <button @click="fetchBooks" class="text-sm px-4 py-2 rounded-lg border border-slate-200 dark:border-zinc-700 text-slate-600 dark:text-zinc-400 hover:bg-slate-100 dark:hover:bg-zinc-800 transition-colors">
        Try again
      </button>
    </div>

    <div v-else-if="books.length === 0" class="flex flex-col items-center justify-center mt-20 text-slate-500 dark:text-zinc-400">
      <span class="material-icons text-4xl mb-4 text-slate-300 dark:text-zinc-600">auto_stories</span>
      <p class="text-base">No books in this library yet</p>
    </div>

    <div v-else class="book-grid">
      <div v-for="book in books" :key="book.id" class="book-card">
        <div class="book-cover">
          <img :src="bookshelfApi.getThumbnailUrl(book.id)" :alt="book.title" />
          <div v-if="book.amount > 1" class="absolute top-2 right-2 flex items-center justify-center min-w-6 h-6 px-1.5 rounded-full bg-primary-600 text-white text-xs font-bold">
            {{ book.amount }}
          </div>
        </div>
        <div class="mt-3 text-center">
          <h3 class="text-sm font-semibold text-slate-900 dark:text-zinc-100 line-clamp-2 min-h-10" :title="book.title">
            {{ book.title }}
          </h3>
          <p class="mt-1 text-xs text-slate-500 dark:text-zinc-400 truncate" :title="book.authors.join(', ')">
            {{ book.authors.join(', ') }}
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import bookshelfApi from '../services/bookshelfApi'

const route = useRoute()
const userId = computed(() => route.params.userId ?? null)

const books = ref([])
const loading = ref(true)
const error = ref(null)

async function fetchBooks() {
  loading.value = true
  error.value = null
  try {
    books.value = await bookshelfApi.listBooks(userId.value)
  } catch (e) {
    error.value = e?.message ?? 'An unexpected error occurred'
  } finally {
    loading.value = false
  }
}

watch(userId, fetchBooks)
onMounted(fetchBooks)
</script>

<style scoped>
.book-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, 150px);
  gap: 2rem;
  justify-content: center;
}

@media (min-width: 1400px) {
  .book-grid {
    grid-template-columns: repeat(min(8, auto-fill), 150px);
  }
}

.book-card {
  width: 150px;
  cursor: default;
}

.book-cover {
  position: relative;
  width: 150px;
  height: 225px;
  border-radius: 0.5rem;
  overflow: hidden;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1);
  background: #f1f5f9;
  display: flex;
  align-items: center;
  justify-content: center;
}

.dark .book-cover {
  background: #18181b;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.3);
}

.book-cover img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}
</style>