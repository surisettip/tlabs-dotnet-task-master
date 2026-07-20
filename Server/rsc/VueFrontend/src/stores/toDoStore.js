import { defineStore } from "pinia";

export const useToDoStore = defineStore("toDoStore", {
  state: () => ({
    tasks: [],
  }),
  actions: {
   async fetchTasks(search = "", sortBy = "id", ascending = true, tag = "") {
      const response = await fetch(
        `http://localhost:5000/api/tasks?search=${encodeURIComponent(search)}&sortBy=${sortBy}&ascending=${ascending}&tag=${encodeURIComponent(tag)}`
      );
      const data = await response.json();
      this.tasks = data;
    },
    async addTask(task) {
      await fetch("http://localhost:5000/api/tasks", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(task),
      }).finally(() => {
        this.fetchTasks();
      });
    },

    async editTask(task) {
      await fetch(`http://localhost:5000/api/tasks/${task.id}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(task),
      }).finally(() => {
        this.fetchTasks();
      });
    },

    async deleteTask(task) {
      await fetch(`http://localhost:5000/api/tasks/${task.id}`, {
        method: "DELETE",
      }).finally(() => {
        this.fetchTasks();
      });
    },
  },
});
