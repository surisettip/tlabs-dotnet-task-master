<template>
  <div>
    <input
      v-model="username"
      placeholder="Username"
    />

    <input
      type="password"
      v-model="password"
      placeholder="Password"
    />

    <button @click="doLogin">
      Login
    </button>

    <div v-if="error">
      {{ error }}
    </div>
  </div>
</template>

<script setup>
import { ref } from "vue";
import { login } from "../stores/authStore.js";

const username = ref("");
const password = ref("");
const error = ref("");

const doLogin = async () => {
  try {
    error.value = "";

    await login(
      username.value,
      password.value
    );

    window.location.reload();
  } catch (err) {
    error.value = err.message;
  }
};
</script>
