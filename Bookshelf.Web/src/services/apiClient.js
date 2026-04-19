class ApiClient {
  constructor() {
    this.baseUrl = import.meta.env["BOOKSHELF_API_HTTPS"] || import.meta.env["BOOKSHELF_API_HTTP"];
  }

  async get(path) {
    const response = await fetch(`${this.baseUrl}${path}`, {
      credentials: 'include'
    });
    if (!response.ok) {
      if (response.status === 401) return null;
      throw new Error(`GET ${path} failed: ${response.statusText}`);
    }
    return response.json();
  }

  async post(path, data) {
    const response = await fetch(`${this.baseUrl}${path}`, {
      method: 'POST',
      credentials: 'include',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(data)
    });
    if (!response.ok) {
      throw new Error(`POST ${path} failed: ${response.statusText}`);
    }
    return response.json().catch(() => ({}));
  }
}

export const apiClient = new ApiClient();
