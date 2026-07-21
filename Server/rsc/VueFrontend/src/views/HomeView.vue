<template>
  <div>
    <div v-if="!isLoggedIn" class="loginContainer">
      <h2>Login</h2>

      <input
        v-model="username"
        placeholder="Username"
      />

      <input
        v-model="password"
        type="password"
        placeholder="Password"
      />

      <button @click="doLogin">
        Login
      </button>

      <div
        v-if="error"
        class="errorMessage"
      >
        {{ error }}
      </div>
    </div>

    <div v-else>
      <button
        class="logoutButton"
        @click="logoutUser"
      >
        Logout
      </button>

      <ToDoList />
    </div>
  </div>
</template>

<script setup>
import { ref } from "vue";
import ToDoList from "../components/ToDoList.vue";
import { login, logout } from "../stores/authStore.js";

const username = ref("");
const password = ref("");
const error = ref("");

const isLoggedIn = ref(
  !!localStorage.getItem("token")
);

const doLogin = async () => {
  try {
    error.value = "";

    await login(
      username.value,
      password.value
    );

    isLoggedIn.value = true;
  } catch (err) {
    error.value = err.message;
  }
};

const logoutUser = () => {
  logout();
  isLoggedIn.value = false;
};
</script>

<style scoped>
.loginContainer {
  width: 300px;
  margin: 50px auto;

  display: flex;
  flex-direction: column;
  gap: 10px;
}

.errorMessage {
  color: red;
}

.logoutButton {
  margin-bottom: 10px;
}
</style>
