<template>
  <div class="min-h-screen bg-gray-100 flex flex-col items-center justify-center p-4">
    <div class="bg-white p-8 rounded-lg shadow-md w-full max-w-md">
      <h1 class="text-3xl font-bold text-primary-600 mb-6 text-center">Bookshelf</h1>
      <div class="text-center">
        <p class="text-gray-600 mb-6">Please log in to access your bookshelf.</p>
        <button @click="login" class="w-full bg-primary-600 hover:bg-primary-700 text-white font-bold py-2 px-4 rounded transition duration-200">
          Login
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import bookshelfApi from '../services/bookshelfApi';

const login = () => {
  window.location.href = bookshelfApi.getLoginUrl(window.location.origin + '/');
};
</script>