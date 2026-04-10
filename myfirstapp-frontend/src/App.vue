<template>
  <div class="app-shell">
    <aside class="app-sidebar">
      <div>
        <p class="app-kicker">myfirstapp</p>
        <h1>Frontend Console</h1>
        <p class="app-copy">針對既有 .NET API 的前端專案，聚焦狀態監看與 Application Profile CRUD。</p>
      </div>

      <nav class="app-nav">
        <RouterLink to="/" class="app-link">系統狀態</RouterLink>
        <RouterLink to="/profiles" class="app-link">Profile 管理</RouterLink>
      </nav>
    </aside>

    <main class="app-main">
      <RouterView />
    </main>
  </div>
</template>

<script setup lang="ts">
import { RouterLink, RouterView } from 'vue-router';
</script>