<template>
  <form class="panel form-grid" @submit.prevent="submitForm">
    <div class="panel-head">
      <div>
        <p class="eyebrow">Profile Editor</p>
        <h2>{{ title }}</h2>
      </div>
      <button type="button" class="ghost-button" @click="$emit('cancel')">取消</button>
    </div>

    <label>
      <span>Profile Key</span>
      <input v-model.trim="form.profileKey" :disabled="mode === 'edit'" required maxlength="40" placeholder="例如 crm-api" />
    </label>

    <label>
      <span>Display Name</span>
      <input v-model.trim="form.displayName" required maxlength="100" />
    </label>

    <label>
      <span>Owner Team</span>
      <input v-model.trim="form.ownerTeam" required maxlength="100" />
    </label>

    <label>
      <span>Environment</span>
      <select v-model="form.environment">
        <option>Development</option>
        <option>SIT</option>
        <option>UAT</option>
        <option>Production</option>
        <option>local</option>
      </select>
    </label>

    <label class="checkbox-row">
      <input v-model="form.isActive" type="checkbox" />
      <span>啟用狀態</span>
    </label>

    <p v-if="errorMessage" class="error-text">{{ errorMessage }}</p>

    <div class="form-actions">
      <button type="submit" class="primary-button" :disabled="submitting">
        {{ submitting ? '處理中...' : submitLabel }}
      </button>
    </div>
  </form>
</template>

<script setup lang="ts">
import { computed, reactive, watch } from 'vue';
import type { ApplicationProfileDto, CreateApplicationProfileCommand, UpdateApplicationProfileCommand } from '../types/profile';

const props = defineProps<{
  mode: 'create' | 'edit';
  initialValue?: ApplicationProfileDto | null;
  submitting?: boolean;
  errorMessage?: string;
}>();

const emit = defineEmits<{
  submit: [payload: CreateApplicationProfileCommand | UpdateApplicationProfileCommand & { profileKey?: string }];
  cancel: [];
}>();

const form = reactive({
  profileKey: '',
  displayName: '',
  ownerTeam: '',
  environment: 'UAT',
  isActive: true
});

watch(
  () => props.initialValue,
  value => {
    form.profileKey = value?.profileKey ?? '';
    form.displayName = value?.displayName ?? '';
    form.ownerTeam = value?.ownerTeam ?? '';
    form.environment = value?.environment ?? 'UAT';
    form.isActive = value?.isActive ?? true;
  },
  { immediate: true }
);

const title = computed(() => (props.mode === 'create' ? '新增 Profile' : '編輯 Profile'));
const submitLabel = computed(() => (props.mode === 'create' ? '建立' : '更新'));

function submitForm(): void {
  if (props.mode === 'create') {
    emit('submit', {
      profileKey: form.profileKey,
      displayName: form.displayName,
      ownerTeam: form.ownerTeam,
      environment: form.environment,
      isActive: form.isActive
    });
    return;
  }

  emit('submit', {
    profileKey: form.profileKey,
    displayName: form.displayName,
    ownerTeam: form.ownerTeam,
    environment: form.environment,
    isActive: form.isActive
  });
}
</script>