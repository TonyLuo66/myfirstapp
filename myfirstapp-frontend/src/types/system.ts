export interface HealthStatusDto {
  status: string;
  appName: string;
  appEnvironment: string;
  runMode: string;
  lastHeartbeatAt?: string | null;
  checkedAt: string;
}

export interface SystemStatusDto {
  appName: string;
  appEnvironment: string;
  runMode: string;
  runModeSource: string;
  heartbeatSeconds: number;
  heartbeatCount: number;
  lastHeartbeatAt?: string | null;
  appMessage?: string | null;
  reportedAt: string;
}