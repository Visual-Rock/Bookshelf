import { ref } from 'vue';
import { apiClient } from './apiClient';

class BookshelfApi {
  constructor() {
    this._user = ref(null);
  }
  async isAuthenticated() {
    try {
      this._user.value = await apiClient.get('/user/info');
      return this._user.value !== null;
    } catch (error) {
      this._user.value = null;
      return false;
    }
  }

  async logout() {
    try {
      const result = await apiClient.post('/user/logout');
      this._user.value = null;
      return result;
    }
    catch (error) {
      console.error('Logout failed:', error);
      this._user.value = null;
      return false;
    }
  }

  getLoginUrl(redirectUri) {
    return `${apiClient.baseUrl}/user/login?redirectUri=${encodeURIComponent(redirectUri)}`;
  }
}

const bookshelfApi = new BookshelfApi();
export default bookshelfApi;
