<template>
    <div class="max-w-2xl mx-auto p-4">
      <div class="space-y-2">
        <BarcodeScanner @scanned="handleScanned" />

        <div class="bg-white dark:bg-zinc-900 p-4 rounded-xl shadow-sm border border-slate-200 dark:border-zinc-800">
          <label for="isbn" class="block text-sm font-medium text-slate-700 dark:text-zinc-300 mb-2">
            Manual ISBN Input
          </label>
          <div class="flex gap-2">
            <input id="isbn" v-model="manualIsbn" type="text" placeholder="Enter ISBN..." class="min-w-0 flex-1 px-4 py-2 bg-slate-50 dark:bg-zinc-800 border border-slate-200 dark:border-zinc-700 rounded-lg focus:ring-2 focus:ring-indigo-500 outline-none transition-all dark:text-white" @keyup.enter="fetchBook(manualIsbn)"/>
            <button @click="fetchBook(manualIsbn)" class="shrink-0 px-4 py-2 bg-indigo-600 hover:bg-indigo-700 text-white rounded-lg font-medium transition-colors disabled:opacity-50" :disabled="loading">
              Search
            </button>
          </div>
        </div>
      </div>

      <Transition enter-active-class="transition duration-300 ease-out" enter-from-class="transform translate-y-full" enter-to-class="transform translate-y-0" leave-active-class="transition duration-200 ease-in" leave-from-class="transform translate-y-0" leave-to-class="transform translate-y-full">
        <div v-if="book" class="fixed inset-x-0 bottom-0 z-[60] bg-white dark:bg-zinc-900 border-t border-slate-200 dark:border-zinc-800 shadow-2xl p-6 pb-8 sm:pb-6 rounded-t-3xl">
          <div class="max-w-2xl mx-auto">
            <div class="flex justify-between items-start mb-4">
              <h2 class="text-xl font-bold text-slate-900 dark:text-zinc-100">{{ book.title }}</h2>
              <button @click="book = null" class="text-slate-400 hover:text-slate-600 dark:hover:text-zinc-300">
                <span class="material-icons">close</span>
              </button>
            </div>

            <div class="flex gap-6">
              <div class="w-24 h-36 bg-slate-100 dark:bg-zinc-800 rounded-lg flex flex-col items-center justify-center border-2 border-dashed border-slate-200 dark:border-zinc-700 shrink-0">
                <span class="material-icons text-slate-300 dark:text-zinc-600 text-3xl mb-1">book</span>
                <span class="text-[10px] text-slate-400 dark:text-zinc-500 font-medium">NO COVER</span>
              </div>

              <div class="flex-1 min-w-0">
                <p class="text-sm text-slate-600 dark:text-zinc-400 mb-1">
                  {{ book.authors.join(', ') }}
                </p>
                <div class="space-y-1 mt-3">
                  <p class="text-xs text-slate-500 dark:text-zinc-500 flex items-center gap-2">
                    <span class="font-medium text-slate-700 dark:text-zinc-300">Publisher:</span>
                    {{ book.publisher || 'Unknown' }}
                  </p>
                  <p class="text-xs text-slate-500 dark:text-zinc-500 flex items-center gap-2">
                    <span class="font-medium text-slate-700 dark:text-zinc-300">Released:</span>
                    {{ formatDate(book.publishDate) }}
                  </p>
                  <p class="text-xs text-slate-500 dark:text-zinc-500 flex items-center gap-2">
                    <span class="font-medium text-slate-700 dark:text-zinc-300">ISBN:</span>
                    {{ book.isbn }}
                  </p>
                </div>
              </div>
            </div>

            <div class="mt-6 flex gap-3">
              <button @click="addBook" class="flex-1 bg-indigo-600 hover:bg-indigo-700 text-white py-3 rounded-xl font-bold transition-all flex items-center justify-center gap-2 disabled:opacity-50" :disabled="adding">
                <span v-if="adding" class="animate-spin material-icons !text-lg">sync</span>
                <span v-else class="material-icons !text-lg">add_circle</span>
                {{ adding ? 'Adding...' : 'Add to Collection' }}
              </button>
              <button @click="book = null" class="px-6 py-3 border border-slate-200 dark:border-zinc-700 text-slate-600 dark:text-zinc-400 rounded-xl font-medium hover:bg-slate-50 dark:hover:bg-zinc-800 transition-colors">
                Cancel
              </button>
            </div>
          </div>
        </div>
      </Transition>

      <div v-if="loading" class="fixed inset-0 bg-slate-900/20 backdrop-blur-[2px] z-[70] flex items-center justify-center">
        <div class="bg-white dark:bg-zinc-900 p-4 rounded-2xl shadow-xl flex items-center gap-3">
          <span class="animate-spin material-icons text-indigo-600">sync</span>
          <span class="font-medium dark:text-zinc-200">Searching book...</span>
        </div>
      </div>

      <div v-if="error" class="fixed bottom-24 left-1/2 -translate-x-1/2 z-[80] bg-red-600 text-white px-6 py-3 rounded-full shadow-lg flex items-center gap-2">
        <span class="material-icons">error_outline</span>
        {{ error }}
      </div>
      
      <div v-if="success" class="fixed bottom-24 left-1/2 -translate-x-1/2 z-[80] bg-emerald-600 text-white px-6 py-3 rounded-full shadow-lg flex items-center gap-2">
        <span class="material-icons">check_circle</span>
        {{ success }}
      </div>
    </div>
</template>

<script setup>
import { ref } from 'vue';
import MainLayout from '../components/MainLayout.vue';
import BarcodeScanner from '../components/BarcodeScanner.vue';
import { apiClient } from '../services/apiClient';

const manualIsbn = ref('');
const book = ref(null);
const loading = ref(false);
const adding = ref(false);
const error = ref(null);
const success = ref(null);

function handleScanned({ decodedText }) {
  fetchBook(decodedText);
}

async function fetchBook(isbn) {
  if (!isbn) return;
  
  loading.value = true;
  error.value = null;
  book.value = null;
  
  try {
    const data = await apiClient.get(`/book?isbn=${encodeURIComponent(isbn)}`);
    if (data) {
      book.value = data;
    } else {
      showError('Book not found');
    }
  } catch (err) {
    console.error('Fetch book error:', err);
    showError('Failed to fetch book details');
  } finally {
    loading.value = false;
  }
}

async function addBook() {
  if (!book.value) return;
  
  adding.value = true;
  error.value = null;
  
  try {
    await apiClient.post('/book/add', { isbn: book.value.isbn });
    showSuccess('Book added to collection!');
    book.value = null;
    manualIsbn.value = '';
  } catch (err) {
    console.error('Add book error:', err);
    showError('Failed to add book to collection');
  } finally {
    adding.value = false;
  }
}

function formatDate(dateString) {
  if (!dateString) return 'Unknown';
  const date = new Date(dateString);
  return date.toLocaleDateString(undefined, { year: 'numeric', month: 'long', day: 'numeric' });
}

function showError(msg) {
  error.value = msg;
  setTimeout(() => {
    error.value = null;
  }, 3000);
}

function showSuccess(msg) {
  success.value = msg;
  setTimeout(() => {
    success.value = null;
  }, 3000);
}
</script>

<style scoped>
@reference "tailwindcss";

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