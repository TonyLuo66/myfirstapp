<template>
  <section class="profiles-layout">
    <article class="panel profiles-panel">
      <div class="panel-head">
        <div>
          <p class="eyebrow">Profiles</p>
          <h2>Application Profiles</h2>
        </div>

        <div class="toolbar">
          <input v-model.trim="keyword" class="search-box" placeholder="搜尋 profile key 或名稱" @keyup.enter="loadProfiles" />
          <button class="ghost-button" @click="loadProfiles" :disabled="loading">搜尋</button>
          <button class="primary-button" @click="startCreate">新增</button>
        </div>
      </div>

      <p v-if="errorMessage" class="error-text">{{ errorMessage }}</p>

      <div class="table-wrap">
        <table>
          <thead>
            <tr>
              <th>Profile Key</th>
              <th>Display Name</th>
              <th>Owner Team</th>
              <th>Environment</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in profiles" :key="item.profileKey" @click="selectProfile(item.profileKey)" :class="{ selected: selectedProfile?.profileKey === item.profileKey }">
              <td>{{ item.profileKey }}</td>
              <td>{{ item.displayName }}</td>
              <td>{{ item.ownerTeam }}</td>
              <td>{{ item.environment }}</td>
              <td>{{ item.isActive ? 'Active' : 'Inactive' }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </article>

    <article class="detail-column">
      <article v-if="selectedProfile && !editingMode" class="panel detail-panel">
        <div class="panel-head">
          <div>
            <p class="eyebrow">Detail</p>
            <h2>{{ selectedProfile.displayName }}</h2>
          </div>
          <button class="ghost-button" @click="startEdit">編輯</button>
        </div>

        <dl class="detail-grid">
          <div>
            <dt>Profile Key</dt>
            <dd>{{ selectedProfile.profileKey }}</dd>
          </div>
          <div>
            <dt>Owner Team</dt>
            <dd>{{ selectedProfile.ownerTeam }}</dd>
          </div>
          <div>
            <dt>Environment</dt>
            <dd>{{ selectedProfile.environment }}</dd>
          </div>
          <div>
            <dt>Status</dt>
            <dd>{{ selectedProfile.isActive ? 'Active' : 'Inactive' }}</dd>
          </div>
          <div>
            <dt>Updated At</dt>
            <dd>{{ formatDate(selectedProfile.updatedAt) }}</dd>
          </div>
        </dl>
      </article>

      <ProfileForm
        v-if="editingMode"
        :mode="editingMode"
        :initial-value="selectedProfile"
        :submitting="submitting"
        :error-message="formErrorMessage"
        @submit="submitForm"
        @cancel="cancelEdit"
      />

      <article v-if="!selectedProfile && !editingMode" class="panel empty-panel">
        <p class="eyebrow">Selection</p>
        <h2>尚未選取資料</h2>
        <p>先從左側選一筆 Profile，或直接新增一筆資料。</p>
      </article>
    </article>
  </section>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue';
import ProfileForm from '../components/ProfileForm.vue';
import { createProfile, getProfile, searchProfiles, updateProfile } from '../services/profileService';
import type {
  ApplicationProfileDto,
  CreateApplicationProfileCommand,
  UpdateApplicationProfileCommand
} from '../types/profile';

const keyword = ref('');
const loading = ref(false);
const submitting = ref(false);
const errorMessage = ref('');
const formErrorMessage = ref('');
const profiles = ref<ApplicationProfileDto[]>([]);
const selectedProfile = ref<ApplicationProfileDto | null>(null);
const editingMode = ref<'create' | 'edit' | null>(null);

async function loadProfiles(): Promise<void> {
  loading.value = true;
  errorMessage.value = '';

  try {
    const result = await searchProfiles({ keyword: keyword.value, pageNumber: 1, pageSize: 20 });
    profiles.value = result.items;
    if (selectedProfile.value) {
      const stillExists = result.items.find(item => item.profileKey === selectedProfile.value?.profileKey);
      selectedProfile.value = stillExists ?? null;
    }
  } catch (error) {
    errorMessage.value = error instanceof Error ? error.message : '讀取列表失敗。';
  } finally {
    loading.value = false;
  }
}

async function selectProfile(profileKey: string): Promise<void> {
  editingMode.value = null;
  formErrorMessage.value = '';

  try {
    selectedProfile.value = await getProfile(profileKey);
  } catch (error) {
    errorMessage.value = error instanceof Error ? error.message : '讀取詳細資料失敗。';
  }
}

function startCreate(): void {
  selectedProfile.value = null;
  editingMode.value = 'create';
  formErrorMessage.value = '';
}

function startEdit(): void {
  editingMode.value = 'edit';
  formErrorMessage.value = '';
}

function cancelEdit(): void {
  editingMode.value = null;
  formErrorMessage.value = '';
}

async function submitForm(payload: CreateApplicationProfileCommand | (UpdateApplicationProfileCommand & { profileKey?: string })): Promise<void> {
  submitting.value = true;
  formErrorMessage.value = '';

  try {
    if (editingMode.value === 'create') {
      const created = await createProfile(payload as CreateApplicationProfileCommand);
      selectedProfile.value = created;
    } else if (editingMode.value === 'edit' && selectedProfile.value) {
      const updated = await updateProfile(selectedProfile.value.profileKey, payload as UpdateApplicationProfileCommand);
      selectedProfile.value = updated;
    }

    editingMode.value = null;
    await loadProfiles();
  } catch (error) {
    formErrorMessage.value = error instanceof Error ? error.message : '儲存資料失敗。';
  } finally {
    submitting.value = false;
  }
}

function formatDate(value: string): string {
  return new Date(value).toLocaleString('zh-TW');
}

onMounted(loadProfiles);
</script>