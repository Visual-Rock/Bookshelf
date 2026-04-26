import { createApp } from 'vue'
import './style.css'
import App from './App.vue'
import router from './router'

const theme = localStorage.getItem('theme');
if (theme === 'dark' || (!theme && window.matchMedia('(prefers-color-scheme: dark)').matches)) {
    document.documentElement.classList.add('dark');
} else {
    document.documentElement.classList.remove('dark');
}

const app = createApp(App).use(router).mount('#app')
