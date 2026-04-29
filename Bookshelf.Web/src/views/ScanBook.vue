<template>
  <div class="max-w-2xl mx-auto p-4">
    <div class="space-y-2">
      <BarcodeScanner @scanned="handleScanned"/>

      <div
        class="bg-white dark:bg-zinc-900 p-4 rounded-xl shadow-sm border border-slate-200 dark:border-zinc-800">
        <label for="isbn" class="block text-sm font-medium text-slate-700 dark:text-zinc-300 mb-2">
          Manual ISBN Input
        </label>
        <div class="flex gap-2">
          <input id="isbn" v-model="manualIsbn" type="text" placeholder="Enter ISBN..."
                 class="min-w-0 flex-1 px-4 py-2 bg-slate-50 dark:bg-zinc-800 border border-slate-200 dark:border-zinc-700 rounded-lg focus:ring-2 focus:ring-primary-500 outline-none transition-all dark:text-white"
                 @keyup.enter="fetchBook(manualIsbn)"/>
          <button @click="fetchBook(manualIsbn)"
                  class="shrink-0 px-4 py-2 bg-primary-600 hover:bg-primary-700 text-white rounded-lg font-medium transition-colors disabled:opacity-50"
                  :disabled="loading">
            Search
          </button>
        </div>
      </div>
    </div>

    <BookDetailsFlyout :book="book" :adding="adding" :removing="removing" @close="book = null" @add="addBook" @remove="removeBook"/>

    <div v-if="loading"
         class="fixed inset-0 bg-slate-900/20 backdrop-blur-[2px] z-70 flex items-center justify-center">
      <div class="bg-white dark:bg-zinc-900 p-4 rounded-2xl shadow-xl flex items-center gap-3">
        <span class="animate-spin material-icons text-primary-600">sync</span>
        <span class="font-medium dark:text-zinc-200">Searching book...</span>
      </div>
    </div>

    <div v-if="error"
         class="fixed bottom-24 left-1/2 -translate-x-1/2 z-80 bg-red-600 text-white px-6 py-3 rounded-full shadow-lg flex items-center gap-2">
      <span class="material-icons">error_outline</span>
      {{ error }}
    </div>

    <div v-if="success"
         class="fixed bottom-24 left-1/2 -translate-x-1/2 z-80 bg-primary-600 text-white px-6 py-3 rounded-full shadow-lg flex items-center gap-2">
      <span class="material-icons">check_circle</span>
      {{ success }}
    </div>
  </div>
</template>

<script setup>
import {ref} from 'vue';
import MainLayout from '../components/MainLayout.vue';
import BarcodeScanner from '../components/BarcodeScanner.vue';
import BookDetailsFlyout from '../components/BookDetailsFlyout.vue';
import bookshelfApi from '../services/bookshelfApi.js';

const manualIsbn = ref('');
const book = ref(null);
const loading = ref(false);
const adding = ref(false);
const removing = ref(false);
const error = ref(null);
const success = ref(null);

function handleScanned({decodedText}) {
  fetchBook(decodedText);
}

async function fetchBook(isbn) {
  if (!isbn) return;

  loading.value = true;
  error.value = null;
  book.value = null;

  try {
    const data = await bookshelfApi.getBookByIsbn(isbn);
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
    await bookshelfApi.addBook(book.value.isbn);
    showSuccess('Book added to collection!');
    book.value = null;
  } catch (err) {
    console.error('Add book error:', err);
    showError('Failed to add book to collection');
  } finally {
    adding.value = false;
  }
}

async function removeBook() {
  if (!book.value) return;

  adding.value = true;
  error.value = null;

  try {
    await bookshelfApi.removeBook(book.value.id, 1);
    showSuccess('Book removed from collection!');
    book.value = null;
  } catch (err) {
    console.error('remove book error:', err);
    showError('Failed to remove book from collection');
  } finally {
    adding.value = false;
  }
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
</style>