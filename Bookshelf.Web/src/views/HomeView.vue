<template>
  <div class="p-4 sm:p-6 lg:p-8">
    <div v-if="loading" class="flex flex-col items-center justify-center mt-20 text-slate-500 dark:text-zinc-400">
      <span class="material-icons animate-spin text-4xl mb-4">refresh</span>
      <p class="text-lg">Loading your bookshelf...</p>
    </div>
    <div class="book-grid">
      <div v-for="book in books" :key="book.id" class="book-card">
        <div class="book-cover">
          <img :src="bookshelfApi.getThumbnailUrl(book.id)" :alt="book.title" />
        </div>
        <div class="mt-3 text-center">
          <h3 class="text-sm font-semibold text-slate-900 dark:text-zinc-100 line-clamp-2 min-h-[2.5rem]" :title="book.title">
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
import { ref, onMounted } from 'vue';
import bookshelfApi from '../services/bookshelfApi';

const books = ref([]);
const loading = ref(true);

onMounted(async () => {
  try {
    books.value = await bookshelfApi.listBook();
  } catch (error) {
    console.error('Failed to fetch books:', error);
  } finally {
    loading.value = false;
  }
});
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
