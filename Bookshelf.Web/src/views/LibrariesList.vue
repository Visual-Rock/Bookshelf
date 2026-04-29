<template>
  <div class="min-h-screen bg-slate-50 dark:bg-zinc-950 px-4 py-8">
    <div class="max-w-2xl mx-auto">
      <div v-if="loading" class="space-y-2">
        <div v-for="n in 6" :key="n" class="h-16 rounded-xl bg-slate-200 dark:bg-zinc-800 animate-pulse" :style="{ opacity: 1 - n * 0.12 }"/>
      </div>

      <div v-else-if="error" class="flex flex-col items-center justify-center py-16 text-center">
        <span class="material-icons text-4xl text-slate-300 dark:text-zinc-600 mb-3">
          wifi_off
        </span>
        <p class="text-sm font-medium text-slate-600 dark:text-zinc-400 mb-1">
          Failed to load libraries
        </p>
        <p class="text-xs text-slate-400 dark:text-zinc-600 mb-4">{{ error }}</p>
        <button @click="fetchLibraries" class="text-xs px-3 py-1.5 rounded-lg border border-slate-200 dark:border-zinc-700 text-slate-600 dark:text-zinc-400 hover:bg-slate-100 dark:hover:bg-zinc-800 transition-colors">
          Try again
        </button>
      </div>

      <div v-else-if="libraries.length === 0" class="flex flex-col items-center justify-center py-16 text-center">
        <span class="material-icons text-4xl text-slate-300 dark:text-zinc-600 mb-3">
          auto_stories
        </span>
        <p class="text-sm font-medium text-slate-600 dark:text-zinc-400">
          No public libraries yet
        </p>
        <p class="text-xs text-slate-400 dark:text-zinc-600 mt-1">
          Be the first to make your library public
        </p>
      </div>

      <ul v-else class="space-y-2">
        <li v-for="(library, index) in libraries" :key="library.userId" class="group flex items-center gap-4 px-4 py-3.5 rounded-xl cursor-pointer bg-white dark:bg-zinc-900 border border-slate-100 dark:border-zinc-800 hover:border-slate-300 dark:hover:border-zinc-600 hover:shadow-sm" @click="navigateTo(library.userId)">
          <div class="w-9 h-9 rounded-full flex items-center justify-center shrink-0 bg-slate-100 dark:bg-zinc-800 group-hover:bg-blue-50 dark:group-hover:bg-blue-950 transition-colors">
            <span class="text-sm font-semibold text-slate-500 dark:text-zinc-400 group-hover:text-blue-600 dark:group-hover:text-blue-400 select-none uppercase">
              {{ library.username.charAt(0) }}
            </span>
          </div>

          <div class="flex-1 min-w-0">
            <p class="text-sm font-medium text-slate-800 dark:text-zinc-100 truncate">
              {{ library.username }}
            </p>
            <p class="text-xs text-slate-400 dark:text-zinc-500 mt-0.5">
              {{ library.books }} {{ library.books === 1 ? 'book' : 'books' }}
            </p>
          </div>

          <span class="material-icons text-[18px] text-slate-300 dark:text-zinc-600 group-hover:text-slate-500 dark:group-hover:text-zinc-400">
            chevron_right
          </span>
        </li>
      </ul>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import bookshelfApi from '../services/bookshelfApi'

const router = useRouter()

const libraries = ref([])
const loading = ref(true)
const error = ref(null)

async function fetchLibraries() {
  loading.value = true
  error.value = null
  try {
    libraries.value = await bookshelfApi.getPublicLibraries()
  } catch (e) {
    error.value = e?.message ?? 'An unexpected error occurred'
  } finally {
    loading.value = false
  }
}

function navigateTo(userId) {
  router.push(`/library/${userId}`)
}

onMounted(fetchLibraries)
</script>