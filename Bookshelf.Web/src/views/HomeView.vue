<template>
  <div class="container mx-auto p-4">
    <h1 class="text-2xl font-bold mb-4">Barcode Scanner Demo</h1>
    <p class="mb-4" v-if="lastScan">Last scan: <span class="font-mono bg-gray-100 px-2 py-1 rounded">{{ lastScan }}</span></p>
    <BarcodeScanner @scanned="onScanned" />
  </div>
</template>

<script setup>
import { ref } from 'vue';
import BarcodeScanner from '../components/BarcodeScanner.vue';

const lastScan = ref('');

const onScanned = (data) => {
  console.log('Scanned:', data.decodedText);
  lastScan.value = data.decodedText;
};
</script>
