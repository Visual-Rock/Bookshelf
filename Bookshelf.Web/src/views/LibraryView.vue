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
      <div v-for="book in books" :key="book.id" class="book-card" @click="selectBook(book)">
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

    <BookDetailsFlyout :book="selectedBook" :adding="adding" :removing="removing" @close="selectedBook = null" @add="addBook" @remove="removeBook"/>

    <div v-if="actionError" class="fixed bottom-24 left-1/2 -translate-x-1/2 z-60 bg-red-600 text-white px-6 py-3 rounded-full shadow-lg flex items-center gap-2">
      <span class="material-icons">error_outline</span>
      {{ actionError }}
    </div>

    <div v-if="success" class="fixed bottom-24 left-1/2 -translate-x-1/2 z-60 bg-primary-600 text-white px-6 py-3 rounded-full shadow-lg flex items-center gap-2">
      <span class="material-icons">check_circle</span>
      {{ success }}
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import bookshelfApi from '../services/bookshelfApi'
import BookDetailsFlyout from '../components/BookDetailsFlyout.vue'

const route = useRoute()
const userId = computed(() => route.params.userId ?? null)

const books = ref([])
const loading = ref(true)
const error = ref(null)

const selectedBook = ref(null)
const adding = ref(false)
const removing = ref(false)
const actionError = ref(null)
const success = ref(null)

function selectBook(book) {
  selectedBook.value = book
}

async function addBook() {
  if (!selectedBook.value) return

  adding.value = true
  actionError.value = null

  try {
    await bookshelfApi.addBook(selectedBook.value.isbn)
    showSuccess('Book added to collection!')
    await fetchBooks()
    selectedBook.value = books.value.find(b => b.id === selectedBook.value.id) || null
  } catch (err) {
    console.error('Add book error:', err)
    showActionError('Failed to add book to collection')
  } finally {
    adding.value = false
  }
}

async function removeBook() {
  if (!selectedBook.value) return

  removing.value = true
  actionError.value = null

  try {
    await bookshelfApi.removeBook(selectedBook.value.id, 1)
    showSuccess('Book removed from collection!')
    await fetchBooks()
    selectedBook.value = books.value.find(b => b.id === selectedBook.value.id) || null
  } catch (err) {
    console.error('Remove book error:', err)
    showActionError('Failed to remove book from collection')
  } finally {
    removing.value = false
  }
}

function showActionError(msg) {
  actionError.value = msg
  setTimeout(() => {
    actionError.value = null
  }, 3000)
}

function showSuccess(msg) {
  success.value = msg
  setTimeout(() => {
    success.value = null
  }, 3000)
}

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
  cursor: pointer;
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

.material-icons {
  font-family: 'Material Icons';
  font-weight: normal;
  font-style: normal;
  line-height: 1;
  letter-spacing: normal;
  text-transform: none;
  display: inline-block;
  white-space: nowrap;
  word-wrap: normal;
  direction: ltr;
  -webkit-font-smoothing: antialiased;
}
</style>