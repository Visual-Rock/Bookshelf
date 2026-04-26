<template>
  <div :class="{ dark: isDark }">
    <div class="min-h-screen flex flex-col transition-colors duration-300 bg-slate-50 text-zinc-900 dark:bg-zinc-950 dark:text-zinc-100">

      <header class="sticky top-0 z-50 border-b transition-colors duration-300 bg-white/80 border-slate-200 backdrop-blur-md dark:bg-zinc-900/80 dark:border-zinc-800">
        <div class="flex items-center gap-3 px-4 h-14">
          <div class="flex items-center gap-1.5 shrink-0">
            <button @click="library" class="topbar-btn text-slate-500 hover:bg-slate-100 hover:text-slate-900 dark:text-zinc-400 dark:hover:bg-zinc-800 dark:hover:text-zinc-100" title="Check if book is in collection using camera">
              <span class="material-icons !text-[1.25rem]">
                book
              </span>
            </button>
            <button @click="scanBook" class="topbar-btn text-slate-500 hover:bg-slate-100 hover:text-slate-900 dark:text-zinc-400 dark:hover:bg-zinc-800 dark:hover:text-zinc-100" title="Check if book is in collection using camera">
              <span class="material-icons !text-[1.25rem]">
                document_scanner
              </span>
            </button>
          </div>

          <div class="w-px h-6 shrink-0 transition-colors duration-300 bg-slate-200 dark:bg-zinc-700"></div>

          <div class="hidden sm:flex flex-1 min-w-0">
            <SearchBar v-model="searchQuery" @search="onSearch" />
          </div>

          <div class="flex items-center gap-2 shrink-0 sm:ml-0 ml-auto">
            <button @click="isDark = !isDark" class="topbar-btn relative w-8 h-8 rounded-lg transition-colors duration-200 text-slate-500 hover:bg-slate-100 hover:text-slate-900 dark:text-zinc-400 dark:hover:bg-zinc-800 dark:hover:text-zinc-100" :title="isDark ? 'Switch to light mode' : 'Switch to dark mode'">
              <span class="material-icons !text-[1rem] m-auto">
                {{ isDark ? 'light_mode' : 'dark_mode' }}
              </span>
            </button>

            <div class="flex items-center gap-2 px-2.5 py-1 rounded-full text-sm font-medium border transition-colors duration-200 cursor-default select-none bg-primary-50 border-primary-200 text-primary-700 dark:bg-primary-950/60 dark:border-primary-800 dark:text-primary-300">
              <span class="flex items-center justify-center w-5 h-5 rounded-full text-xs font-bold bg-primary-500 text-white dark:bg-primary-600">
                {{ userInitials }}
              </span>
              <span class="hidden sm:inline max-w-[120px] truncate">{{ username }}</span>
            </div>

            <button @click="onLogout" class="flex items-center gap-1.5 px-3 py-1.5 rounded-lg text-sm font-medium border transition-all duration-200 border-slate-200 text-slate-500 hover:bg-red-50 hover:border-red-200 hover:text-red-600 dark:border-zinc-700 dark:text-zinc-400 dark:hover:bg-red-950/50 dark:hover:border-red-800 dark:hover:text-red-400" title="Log out">
              <span class="material-icons !text-[1rem] shrink-0">
                logout
              </span>
              <span class="hidden sm:inline">Logout</span>
            </button>
          </div>
        </div>
      </header>

      <main class="flex-1 overflow-auto">
        <slot />
      </main>

      <div class="sm:hidden sticky bottom-0 z-50 border-t px-3 py-2 transition-colors duration-300 bg-white/90 border-slate-200 backdrop-blur-md dark:bg-zinc-900/90 dark:border-zinc-800">
        <SearchBar v-model="searchQuery" @search="onSearch" />
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue';
import { useRouter } from 'vue-router';
import bookshelfApi from '../services/bookshelfApi';
import SearchBar from './SearchBar.vue';

const props = defineProps({
  defaultDark: {
    type: Boolean,
    default: false,
  },
})

const router = useRouter();
const username = computed(() => bookshelfApi.user?.username ?? bookshelfApi.user?.Username ?? '')
const isDark = ref(props.defaultDark)
const searchQuery = ref('')

function onSearch() {
  console.log('Searching for:', searchQuery.value);
}

function library() {
  router.push('/');
}

function scanBook() {
  router.push('/scan');
}

function onLogout() {
  window.location.href = bookshelfApi.getLogoutUrl(window.location.origin + '/');
}

const userInitials = computed(() => {
  const name = username.value || ''
  return name.split(' ').filter(Boolean).map(w => w[0]).join('').toUpperCase().slice(0, 2)
})
</script>

<style scoped>
.topbar-btn {
    @reference "tailwindcss";

    @apply flex items-center justify-center w-8 h-8 rounded-lg transition-colors duration-200;
}
</style>