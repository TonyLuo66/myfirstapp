<template>
  <section class="page-grid">
    <article class="panel hero-panel">
      <p class="eyebrow">Runtime Status</p>
      <h2>{{ systemStatus?.appName ?? 'myfirstapp' }}</h2>
      <p class="hero-copy">這個前端專案直接銜接既有後端 API，提供系統監看與應用設定資料管理。</p>
      <button class="primary-button" @click="loadDashboard" :disabled="loading">
        {{ loading ? '更新中...' : '重新整理狀態' }}
      </button>
      <p v-if="errorMessage" class="error-text">{{ errorMessage }}</p>
    </article>

    <article class="panel metric-panel">
      <p class="eyebrow">Health</p>
      <h3>{{ healthStatus?.status ?? '--' }}</h3>
      <p>Environment: {{ healthStatus?.appEnvironment ?? '--' }}</p>
      <p>Run Mode: {{ healthStatus?.runMode ?? '--' }}</p>
    </article>

    <article class="panel metric-panel">
      <p class="eyebrow">Heartbeat</p>
      <h3>{{ systemStatus?.heartbeatCount ?? '--' }}</h3>
      <p>Interval: {{ systemStatus?.heartbeatSeconds ?? '--' }} 秒</p>
      <p>Last: {{ formatDate(systemStatus?.lastHeartbeatAt) }}</p>
    </article>

    <article class="panel metric-panel">
      <p class="eyebrow">Message</p>
      <h3>{{ systemStatus?.runModeSource ?? '--' }}</h3>
      <p>{{ systemStatus?.appMessage ?? '未設定訊息' }}</p>
      <p>Reported: {{ formatDate(systemStatus?.reportedAt) }}</p>
    </article>
  </section>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { getHealthStatus, getSystemStatus } from '../services/systemService';
import type { HealthStatusDto, SystemStatusDto } from '../types/system';

const loading = ref(false);
const errorMessage = ref('');
const healthStatus = ref<HealthStatusDto | null>(null);
const systemStatus = ref<SystemStatusDto | null>(null);

async function loadDashboard(): Promise<void> {
  loading.value = true;
  errorMessage.value = '';

  try {
    const [health, status] = await Promise.all([getHealthStatus(), getSystemStatus()]);
    healthStatus.value = health;
    systemStatus.value = status;
  } catch (error) {
    errorMessage.value = error instanceof Error ? error.message : '讀取系統狀態失敗。';
  } finally {
    loading.value = false;
  }
}

function formatDate(value?: string | null): string {
  return value ? new Date(value).toLocaleString('zh-TW') : '--';
}

onMounted(loadDashboard);
</script>