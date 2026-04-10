import { request } from './apiClient';
import type { PagedResult } from '../types/api';
import type {
  ApplicationProfileDto,
  ApplicationProfileQuery,
  CreateApplicationProfileCommand,
  UpdateApplicationProfileCommand
} from '../types/profile';

function toQueryString(query: ApplicationProfileQuery): string {
  const params = new URLSearchParams();

  if (query.keyword) {
    params.set('keyword', query.keyword);
  }

  if (typeof query.pageNumber === 'number') {
    params.set('pageNumber', String(query.pageNumber));
  }

  if (typeof query.pageSize === 'number') {
    params.set('pageSize', String(query.pageSize));
  }

  if (typeof query.isActive === 'boolean') {
    params.set('isActive', String(query.isActive));
  }

  const text = params.toString();
  return text ? `?${text}` : '';
}

export async function searchProfiles(query: ApplicationProfileQuery): Promise<PagedResult<ApplicationProfileDto>> {
  const response = await request<PagedResult<ApplicationProfileDto>>(`/api/application-profiles${toQueryString(query)}`);
  return response.data;
}

export async function getProfile(profileKey: string): Promise<ApplicationProfileDto> {
  const response = await request<ApplicationProfileDto>(`/api/application-profiles/${encodeURIComponent(profileKey)}`);
  return response.data;
}

export async function createProfile(command: CreateApplicationProfileCommand): Promise<ApplicationProfileDto> {
  const response = await request<ApplicationProfileDto>('/api/application-profiles', {
    method: 'POST',
    body: JSON.stringify(command)
  });
  return response.data;
}

export async function updateProfile(profileKey: string, command: UpdateApplicationProfileCommand): Promise<ApplicationProfileDto> {
  const response = await request<ApplicationProfileDto>(`/api/application-profiles/${encodeURIComponent(profileKey)}`, {
    method: 'PUT',
    body: JSON.stringify(command)
  });
  return response.data;
}