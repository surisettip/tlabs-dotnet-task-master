import { defineStore } from "pinia";

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
    }

    let errorMessage =
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
  }),
  actions: {
    async fetchTasks(search = "", sortBy = "id", ascending = true, tag = "") {
      try {
        const response = await fetch(
          `http://localhost:5000/api/tasks?search=${encodeURIComponent(search)}&sortBy=${sortBy}&ascending=${ascending}&tag=${encodeURIComponent(tag)}`
        );
         const data = await handleResponse(response);
        this.tasks = data;
      } catch (error) {
          if (error instanceof TypeError) {
            this.error =
              "Unable to connect to the server. Please check if the backend is running.";
          } else {
            this.error = error.message;
          }

          console.error("Fetch Tasks Error:", error);
        //alert(error.message);
      }
    },

  async addTask(task) {
  try {
    // Validate title
    if (!task.title || task.title.trim() === "") {
      this.error = "Title is required.";
      return;
    }

    const response = await fetch("http://localhost:5000/api/tasks", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(task),
    });

    await handleResponse(response);
    await this.fetchTasks();
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
        const response = await fetch(`http://localhost:5000/api/tasks/${task.id}`, {
          method: "PUT",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(task),
        });
        await handleResponse(response);
        await this.fetchTasks();
      } catch (error) {
        if (error instanceof TypeError) {
          this.error =
            "Unable to connect to the server. Please check if the backend is running.";
        } else {
          this.error = error.message || "Failed to edit task.";
        }
        console.error("Edit Task Error:", error);
        // alert(error.message);
      }
    },

    async deleteTask(task) {
      try {
        const response = await fetch(`http://localhost:5000/api/tasks/${task.id}`, {
          method: "DELETE",
        });
        await handleResponse(response);
        await this.fetchTasks();
      } catch (error) {
        if (error instanceof TypeError) {
          this.error =
            "Unable to connect to the server. Please check if the backend is running.";
        } else {
          this.error = error.message || "Failed to delete task.";
        }
        console.error("Delete Task Error:", error);
        // alert(error.message);
      }
    },

    clearError() {
      this.error = null;
    },
  },
});
