<template>
  <Transition enter-active-class="transition duration-300 ease-out" enter-from-class="transform translate-y-full"
              enter-to-class="transform translate-y-0" leave-active-class="transition duration-200 ease-in"
              leave-from-class="transform translate-y-0" leave-to-class="transform translate-y-full">
    <div v-if="book"
         class="fixed inset-x-0 bottom-0 z-50 bg-white dark:bg-zinc-900 border-t border-slate-200 dark:border-zinc-800 shadow-2xl p-6 pb-8 sm:pb-6 rounded-t-3xl">
      <div class="max-w-2xl mx-auto">
        <div class="flex justify-between items-start mb-4">
          <h2 class="flex items-center text-xl font-bold text-slate-900 dark:text-zinc-100">
            {{ book.title }}
            <span class="material-icons p-1"
                  :class="book.amount >= 1 ? 'text-green-500' : 'text-red-500'">
                {{ book.amount >= 1 ? 'check_circle' : 'cancel' }}
              </span>
          </h2>
          <button @click="$emit('close')"
                  class="text-slate-400 hover:text-slate-600 dark:hover:text-zinc-300">
            <span class="material-icons">close</span>
          </button>
        </div>

        <div class="flex flex-col sm:flex-row gap-6">
          <div
            class="w-32 h-48 bg-slate-100 dark:bg-zinc-800 rounded-lg flex flex-col items-center justify-center shrink-0 overflow-hidden mx-auto sm:mx-0">
            <img :src="bookshelfApi.getThumbnailUrl(book.id)" alt="book cover" class="w-full h-full object-cover">
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
              <p class="text-xs text-slate-500 dark:text-zinc-500 flex items-center gap-2">
                <span class="font-medium text-slate-700 dark:text-zinc-300">Language:</span>
                {{ book.language }}
              </p>
              <p class="text-xs text-slate-500 dark:text-zinc-500 flex items-center gap-2">
                <span class="font-medium text-slate-700 dark:text-zinc-300">Pages:</span>
                {{ book.pages }}
              </p>
            </div>

            <div v-if="book.publicUserAmount && Object.keys(book.publicUserAmount).length > 0" class="mt-4">
              <p class="text-xs font-medium text-slate-700 dark:text-zinc-300 mb-2 flex items-center gap-1.5">
                <span class="material-icons text-[14px]">people</span>
                In other libraries
              </p>
              <ul class="space-y-1">
                <li v-for="(amount, username) in book.publicUserAmount" :key="username"
                    class="flex items-center justify-between text-xs text-slate-500 dark:text-zinc-500 px-2 py-1 rounded-md hover:bg-slate-100 dark:hover:bg-zinc-800 transition-colors">
                  <span class="flex items-center gap-1.5">
                    <span class="material-icons text-[13px] text-slate-400 dark:text-zinc-500">person</span>
                    <span class="text-slate-600 dark:text-zinc-400">{{ username }}</span>
                  </span>
                  <span class="font-medium text-slate-700 dark:text-zinc-300 tabular-nums">
                    {{ amount }} {{ amount === 1 ? 'copy' : 'copies' }}
                  </span>
                </li>
              </ul>
            </div>
          </div>
        </div>

        <div class="mt-6 grid grid-cols-[repeat(auto-fit,minmax(280px,1fr))] gap-3">
          <button @click="$emit('add')"
                  class="bg-primary-600 hover:bg-primary-700 text-white py-3 rounded-xl font-bold transition-all flex items-center justify-center gap-2 disabled:opacity-50"
                  :disabled="adding">
            <span v-if="adding" class="animate-spin material-icons text-lg!">sync</span>
            <span v-else class="material-icons text-lg!">add_circle</span>
            {{ adding ? 'Adding...' : 'Add to Collection' }}
          </button>
          <button v-if="book.amount >= 1" @click="$emit('remove')"
                  class="bg-red-600 hover:bg-red-700 text-white py-3 rounded-xl font-bold transition-all flex items-center justify-center gap-2 disabled:opacity-50"
                  :disabled="removing">
            <span v-if="removing" class="animate-spin material-icons text-lg!">sync</span>
            <span v-else class="material-icons text-lg!">remove_circle</span>
            {{ removing ? 'Removing...' : 'Remove from Collection' }}
          </button>
        </div>
      </div>
    </div>
  </Transition>
</template>

<script setup>
import bookshelfApi from '../services/bookshelfApi.js';

defineProps({
  book: {
    type: Object,
    default: null
  },
  adding: {
    type: Boolean,
    default: false
  },
  removing: {
    type: Boolean,
    default: false
  }
});

defineEmits(['close', 'add', 'remove']);

function formatDate(dateString) {
  if (!dateString) return 'Unknown';
  const date = new Date(dateString);
  return date.toLocaleDateString(undefined, {year: 'numeric', month: 'long', day: 'numeric'});
}
</script>

<style scoped>
@reference "tailwindcss";
</style>
