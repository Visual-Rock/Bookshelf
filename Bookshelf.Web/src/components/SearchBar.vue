<template>
  <div class="relative w-full max-w-md">
    <span class="material-icons absolute left-3 top-1/2 -translate-y-1/2 !text-[1rem] pointer-events-none text-slate-400 dark:text-zinc-500 transition-colors duration-300">
      search
    </span>
    <input
      v-model="modelValue"
      type="search"
      placeholder="Search…"
      class="w-full pl-9 pr-3 py-1.5 text-sm rounded-lg border outline-none transition-all duration-200 focus:ring-2 
             bg-slate-100 border-slate-200 placeholder-slate-400 text-zinc-900 focus:border-indigo-400 focus:ring-indigo-400/20
             dark:bg-zinc-800 dark:border-zinc-700 dark:placeholder-zinc-500 dark:text-zinc-100 dark:focus:border-indigo-500 dark:focus:ring-indigo-500/20"
      @keyup.enter="$emit('search')"
    />
  </div>
</template>

<script setup>
const modelValue = defineModel()
defineEmits(['search'])
</script>
