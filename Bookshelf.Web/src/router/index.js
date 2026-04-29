import { createRouter, createWebHistory } from 'vue-router';
import bookshelfApi from '../services/bookshelfApi';

const routes = [
  {
    path: '/',
    name: 'home',
    component: () => import('../views/HomeView.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/scan',
    name: 'scan',
    component: () => import('../views/ScanBook.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/libraries',
    name: 'libraries',
    component: () => import('../views/LibrariesList.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/login',
    name: 'login',
    component: () => import('../views/LoginView.vue')
  },
  {
    path: '/:pathMatch(.*)*',
    redirect: '/'
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes
});

router.beforeEach(async (to, from, next) => {
  const requiresAuth = to.matched.some(record => record.meta.requiresAuth);
  
  if (requiresAuth && !(await bookshelfApi.isAuthenticated())) {
    next('/login');
  } else {
    next();
  }
});

export default router;
