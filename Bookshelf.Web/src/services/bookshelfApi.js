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
  
  updateSettings() {
    return apiClient.post('/user/settings', this.user.settings);
  }
  
  getBookByIsbn(isbn) {
    return apiClient.get(`/book?isbn=${encodeURIComponent(isbn)}`);
  }

  getLogoutUrl(redirectUri) {
    return `${apiClient.baseUrl}/user/logout?redirectUri=${encodeURIComponent(redirectUri)}`;
  }

  getLoginUrl(redirectUri) {
    return `${apiClient.baseUrl}/user/login?redirectUri=${encodeURIComponent(redirectUri)}`;
  }
  
  addBook(isbn) {
    return apiClient.post('/book/add', { isbn: isbn });
  }

  removeBook(id, amount) {
    return apiClient.post(`/book/remove/${id}?amount=${amount}`);
  }

  listBooks(userId = null) {
    let query = userId ? `?userId=${userId}` : ""; 
    return apiClient.get(`/library/list${query}`);
  }
  
  getCoverUrl(bookId) {
    return `${apiClient.baseUrl}/book/${bookId}/cover`;
  }
  
  getPublicLibraries() {
    return apiClient.get(`/library/public`);
  }

  getThumbnailUrl(bookId) {
    return `${apiClient.baseUrl}/book/${bookId}/thumbnail`;
  }
}

const bookshelfApi = new BookshelfApi();
export default bookshelfApi;
