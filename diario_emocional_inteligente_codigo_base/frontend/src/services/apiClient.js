export const API_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5000/api';

export function createApiClient({ token, onUnauthorized }) {
  async function request(path, options = {}) {
    const response = await fetch(`${API_URL}${path}`, {
      ...options,
      headers: {
        'Content-Type': 'application/json',
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
        ...(options.headers ?? {})
      }
    });

    if (response.status === 401) {
      onUnauthorized?.();
      throw new Error('Sessão expirada. Entre novamente.');
    }

    if (!response.ok) {
      throw new Error(await readError(response));
    }

    if (response.status === 204) return null;
    return response.json();
  }

  async function download(path) {
    const response = await fetch(`${API_URL}${path}`, {
      headers: token ? { Authorization: `Bearer ${token}` } : {}
    });

    if (!response.ok) throw new Error(await readError(response));
    return response.blob();
  }

  return { request, download };
}

async function readError(response) {
  try {
    const data = await response.json();
    return data.mensagem ?? 'Não foi possível concluir a ação.';
  } catch {
    return response.text();
  }
}
