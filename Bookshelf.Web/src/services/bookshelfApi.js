import { ref } from 'vue';
import { apiClient } from './apiClient';

class BookshelfApi {
  constructor() {
    this._user = ref(null);
  }

  get user() {
    return this._user.value;
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

  getLogoutUrl(redirectUri) {
    return `${apiClient.baseUrl}/user/logout?redirectUri=${encodeURIComponent(redirectUri)}`;
  }

  getLoginUrl(redirectUri) {
    return `${apiClient.baseUrl}/user/login?redirectUri=${encodeURIComponent(redirectUri)}`;
  }
}

const bookshelfApi = new BookshelfApi();
export default bookshelfApi;
