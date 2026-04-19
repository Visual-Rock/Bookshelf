<template>
  <component :is="layout">
    <router-view />
  </component>
</template>

<script setup>
import { computed } from 'vue';
import { useRoute } from 'vue-router';
import MainLayout from './components/MainLayout.vue';

const route = useRoute();
const layout = computed(() => {
  return route.name === 'login' ? 'div' : MainLayout;
});
</script>
