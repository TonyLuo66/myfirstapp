import { request } from './apiClient';
import type { HealthStatusDto, SystemStatusDto } from '../types/system';

export async function getHealthStatus(): Promise<HealthStatusDto> {
  const response = await request<HealthStatusDto>('/api/system-status/health');
  return response.data;
}

export async function getSystemStatus(): Promise<SystemStatusDto> {
  const response = await request<SystemStatusDto>('/api/system-status');
  return response.data;
}