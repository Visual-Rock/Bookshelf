<template>
  <div class="flex flex-col gap-4 p-4 bg-white dark:bg-zinc-900 rounded-xl shadow-lg border border-slate-200 dark:border-zinc-800 max-w-md mx-auto">
    <div class="flex p-1 bg-slate-100 dark:bg-zinc-800 rounded-lg">
      <button @click="mode = 'camera'" type="button" :class="['flex-1 flex items-center justify-center gap-2 py-2 rounded-md transition-all duration-200', mode === 'camera' ? 'bg-white dark:bg-zinc-700 shadow-sm text-primary-600 dark:text-primary-300 font-medium' : 'text-slate-500 dark:text-zinc-400 hover:text-slate-700 dark:hover:text-zinc-300']">
        <span class="material-icons !text-lg">videocam</span>
        Camera
      </button>
      <button @click="mode = 'file'" type="button" :class="['flex-1 flex items-center justify-center gap-2 py-2 rounded-md transition-all duration-200', mode === 'file' ? 'bg-white dark:bg-zinc-700 shadow-sm text-primary-600 dark:text-primary-300 font-medium' : 'text-slate-500 dark:text-zinc-400 hover:text-slate-700 dark:hover:text-zinc-300']">
        <span class="material-icons !text-lg">image</span>
        File
      </button>
    </div>

    <div v-show="mode === 'camera'" class="relative aspect-square bg-black rounded-lg overflow-hidden group">
      <video ref="videoRef" class="w-full h-full object-cover"></video>
      <div class="absolute inset-0 border-2 border-primary-500/30 pointer-events-none"></div>
      <div class="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-48 h-48 border-2 border-primary-500 rounded-lg pointer-events-none]">
        <div class="absolute top-0 left-0 w-4 h-4 border-t-4 border-l-4 border-white -translate-x-1 -translate-y-1"></div>
        <div class="absolute top-0 right-0 w-4 h-4 border-t-4 border-r-4 border-white translate-x-1 -translate-y-1"></div>
        <div class="absolute bottom-0 left-0 w-4 h-4 border-b-4 border-l-4 border-white -translate-x-1 translate-y-1"></div>
        <div class="absolute bottom-0 right-0 w-4 h-4 border-b-4 border-r-4 border-white translate-x-1 translate-y-1"></div>
      </div>
      
      <div v-if="error" class="absolute inset-0 flex items-center justify-center bg-black/60 p-4 text-center">
        <div class="text-white">
          <span class="material-icons text-red-400 text-4xl mb-2">error_outline</span>
          <p class="text-sm">{{ error }}</p>
          <button @click="startScanning" type="button" class="mt-4 px-4 py-2 bg-primary-600 rounded-lg text-xs font-medium hover:bg-primary-700 transition-colors">
            Try Again
          </button>
        </div>
      </div>
    </div>

    <div v-show="mode === 'file'" class="flex flex-col items-center justify-center aspect-square border-2 border-dashed border-slate-300 dark:border-zinc-700 rounded-lg p-6 text-center transition-colors hover:border-primary-400 dark:hover:border-primary-500 cursor-pointer relative" @click="fileInputRef.click()">
      <input ref="fileInputRef" type="file" accept="image/*" class="hidden" @change="handleFileChange"/>
      <div v-if="!previewImage" class="flex flex-col items-center">
        <div class="w-16 h-16 rounded-full bg-primary-50 dark:bg-primary-950/30 flex items-center justify-center mb-4">
          <span class="material-icons text-primary-500 text-3xl">upload_file</span>
        </div>
        <p class="text-sm font-medium text-slate-700 dark:text-zinc-300">Click to upload or drag an image</p>
      </div>
      <div v-else class="w-full h-full flex flex-col items-center">
        <img :src="previewImage" class="w-full h-full object-contain rounded-lg mb-2" alt="Barcode Preview" />
        <button @click.stop="previewImage = null; error = null" type="button" class="absolute top-2 right-2 p-1 bg-white/80 dark:bg-zinc-800/80 rounded-full shadow hover:bg-white dark:hover:bg-zinc-700 transition-colors">
          <span class="material-icons text-sm">close</span>
        </button>
      </div>
      
      <div v-if="mode === 'file' && error" class="mt-2 text-red-500 text-xs">{{ error }}</div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted, watch } from 'vue';
import { BrowserMultiFormatReader, BarcodeFormat, DecodeHintType } from '@zxing/library';

const props = defineProps({
  active: {
    type: Boolean,
    default: true
  }
});

const emit = defineEmits(['scanned', 'error']);

const mode = ref('camera');
const videoRef = ref(null);
const fileInputRef = ref(null);
const previewImage = ref(null);
const error = ref(null);

const hints = new Map();
hints.set(DecodeHintType.POSSIBLE_FORMATS, [BarcodeFormat.EAN_13]);

const codeReader = new BrowserMultiFormatReader(hints);

async function startScanning() {
  if (mode.value !== 'camera' || !props.active) return;
  
  error.value = null;
  
  try {
    const videoInputDevices = await codeReader.listVideoInputDevices();
    if (videoInputDevices.length === 0) {
      throw new Error('No camera found');
    }
    
    const selectedDeviceId = videoInputDevices[0].deviceId;
    
    await codeReader.decodeFromVideoDevice(selectedDeviceId, videoRef.value, (result) => {
      if (result) {
        if (result.getBarcodeFormat() === BarcodeFormat.EAN_13) {
          emit('scanned', { decodedText: result.getText() });
        }
      }
    });
  } catch (err) {
    console.error('Camera access error:', err);
    error.value = 'Failed to access camera. Please check permissions.';
    emit('error', err);
  }
}

function stopScanning() {
  codeReader.reset();
}

async function handleFileChange(event) {
  const file = event.target.files[0];
  if (!file) return;

  error.value = null;
  previewImage.value = URL.createObjectURL(file);

  try {
    const result = await codeReader.decodeFromImageUrl(previewImage.value);
    if (result) {
      if (result.getBarcodeFormat() === BarcodeFormat.EAN_13) {
        emit('scanned', { decodedText: result.getText() });
      } else {
        error.value = 'Not an EAN-13 barcode. Please try another image.';
      }
    }
  } catch (err) {
    console.error('File scanning error:', err);
    error.value = 'Could not find a valid EAN-13 barcode in the image.';
    emit('error', err);
  }
}

watch(mode, (newMode) => {
  if (newMode === 'camera') {
    startScanning();
  } else {
    stopScanning();
  }
});

watch(() => props.active, (isActive) => {
  if (isActive && mode.value === 'camera') {
    startScanning();
  } else {
    stopScanning();
  }
});

onMounted(() => {
  if (props.active && mode.value === 'camera') {
    startScanning();
  }
});

onUnmounted(() => {
  stopScanning();
});
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
