<template>
  <div class="bg-white p-8 rounded-lg shadow-md w-full max-w-md">
    <h1 class="text-3xl font-bold text-blue-600 mb-6 text-center">Bookshelf</h1>      
      <button @click="logout" class="w-full bg-red-500 hover:bg-red-600 text-white font-bold py-2 px-4 rounded transition duration-200">
        Logout
      </button>
  </div>
</template>

<script setup>
import { computed } from 'vue';
import { useRouter } from 'vue-router';
import bookshelfApi from '../services/bookshelfApi';

const router = useRouter();

const logout = async () => {
  try {
    await bookshelfApi.logout();
  }
  catch (error) {
    console.error('Logout failed:', error);
  }
  router.push('/login');
}
</script>
