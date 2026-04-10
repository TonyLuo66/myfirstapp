import type { ApiResponse } from '../types/api';

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL?.trim() ?? '';

function buildUrl(path: string): string {
  if (!apiBaseUrl) {
    return path;
  }

  return `${apiBaseUrl.replace(/\/$/, '')}${path}`;
}

export async function request<TData>(path: string, init?: RequestInit): Promise<ApiResponse<TData>> {
  const response = await fetch(buildUrl(path), {
    headers: {
      'Content-Type': 'application/json',
      ...(init?.headers ?? {})
    },
    ...init
  });

  const payload = (await response.json()) as ApiResponse<TData>;

  if (!response.ok || payload.rtnCode !== '0000') {
    throw new Error(payload.rtnMsg || 'API request failed.');
  }

  return payload;
}