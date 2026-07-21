import { defineStore } from "pinia";
import { getAuthHeaders } from "../stores/authStore.js";

async function handleResponse(response) {
  let data = null;
  let textFallback = null;

  try {
    const rawText = await response.text();

    try {
      data = JSON.parse(rawText);
    } catch {
      textFallback = rawText;
    }
  } catch {
    // Ignore parsing errors
  }

  if (!response.ok) {

  if (response.status === 401) {
        localStorage.removeItem("token");

        throw new Error(
          "Your session has expired. Please login again."
        );
    }    let errorMessage =
      data?.error ||
      data?.message;

    if (!errorMessage && data?.errors) {
      errorMessage = Object.values(data.errors)
        .flat()
        .join(", ");
    }

    if (!errorMessage) {
      errorMessage =
        data?.title ||
        textFallback ||
        `${response.status} ${response.statusText}`;
    }

    throw new Error(errorMessage);
  }

  return data;
}

export const useToDoStore = defineStore("toDoStore", {
  state: () => ({
    tasks: [],
    error: null,
    totalCount: 0,
  }),

  actions: {
    async fetchTasks(search = "", sortBy = "id", ascending = true, tag = "",pageNumber = 1,
      pageSize = 10) {
      try {
        this.error = null;

        const response = await fetch(
          `http://localhost:5000/api/tasks?search=${encodeURIComponent(search)}&sortBy=${sortBy}&ascending=${ascending}&tag=${encodeURIComponent(tag)}&pageNumber=${pageNumber}&pageSize=${pageSize}`,
          {
            headers: getAuthHeaders(),
          }
        );
        const data = await handleResponse(response);
        console.log("FULL API RESPONSE:", data);
        console.log("items:", data.items);
        console.log("totalCount:", data.totalCount);
        console.log("Items:", data.Items);
        console.log("TotalCount:", data.TotalCount);

        this.tasks = data.items || [];
        this.totalCount = data.totalCount || 0;

        console.log("Store Tasks:", this.tasks);
        console.log("Store Total Count:", this.totalCount);


      } catch (error) {
          if (error instanceof TypeError) {
            this.error =
              "Unable to connect to the server. Please check if the backend is running.";
          } else {
        this.error = error.message;
          }

        console.error("Fetch Tasks Error:", error);
        throw error;
      }
    },

    async addTask(task) {
      try {
        // Validate title
        this.error = null;
        if (!task.title || task.title.trim() === "") {
          this.error = "Title is required.";
          return;
        }

        const response = await fetch(
          "http://localhost:5000/api/tasks",
          {
            method: "POST",
            headers: getAuthHeaders(),
            body: JSON.stringify(task),
          }
        );

        await handleResponse(response);
      } catch (error) {
    if (error instanceof TypeError) {
      this.error =
        "Unable to connect to the server. Please check if the backend is running.";
    } else {
      this.error = error.message || "Failed to add task.";
    }
      }
    },

    async editTask(task) {
      try {
        this.error = null;

        const response = await fetch(
          `http://localhost:5000/api/tasks/${task.id}`,
          {
            method: "PUT",
            headers: getAuthHeaders(),
            body: JSON.stringify(task),
          }
        );

        await handleResponse(response);
      } catch (error) {
        if (error instanceof TypeError) {
          this.error =
            "Unable to connect to the server. Please check if the backend is running.";
        } else {
          this.error = error.message || "Failed to edit task.";
        }
        console.error("Edit Task Error:", error);
        throw error;
      }
    },

    async deleteTask(task) {
      try {
        this.error = null;

        const response = await fetch(
          `http://localhost:5000/api/tasks/${task.id}`,
          {
            method: "DELETE",
            headers: getAuthHeaders(),
          }
        );

        await handleResponse(response);
      } catch (error) {
        if (error instanceof TypeError) {
          this.error =
            "Unable to connect to the server. Please check if the backend is running.";
        } else {
          this.error = error.message || "Failed to delete task.";
        }
        console.error("Delete Task Error:", error);
        throw error;
      }
    },

    clearError() {
      this.error = null;
    },
  },
});
