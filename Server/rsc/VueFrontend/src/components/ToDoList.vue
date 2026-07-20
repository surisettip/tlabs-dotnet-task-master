<template>
  <div class="formContainer">
    <div class="createToDoItemContainer">
      <input
        class="createInput"
        v-model="newTask"
        @keyup.enter="addTask"
        placeholder="Add a new todo"
      />
      <button class="createButton" @click="addTask">Add</button>
    </div>

    <!-- NEW: Sort Dropdown -->
    <div class="sortContainer">
      <label for="sortBy">Sort By:</label>
      <select id="sortBy" v-model="selectedSort" @change="sortTasks">
        <option value="id">Id</option>
        <option value="title">Title</option>
        <option value="iscompleted">Completed Status</option>
        <option value="createdat">Created Date</option>
        <option value="completedat">Completed Date</option>
      </select>
      <label for="orderBy">Order By:</label>
      <select id="orderBy" v-model="selectedOrder" @change="sortTasks">
        <option value="true">Ascending</option>
        <option value="false">Descending</option>
      </select>
    </div>


    <div class="searchContainer">
      <input
        v-model="searchText"
        @input="refreshTasks"
        placeholder="Search tasks..."
      />
    </div>

    <div class="toDoListContainer">
      <div v-for="task in todos" :key="task.id">
        <div class="toDoRow">
          <input
            type="checkbox"
            @change="updateTask(task)"
            v-model="task.isCompleted"
          />

          <input
            :class="{ completed: task.isCompleted, editInput: true }"
            v-model="task.title"
            @blur="updateTask(task)"
            @keyup.enter="updateTask(task)"
          />

          <button class="deleteButton" @click="deleteTask(task)">
            Delete
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, computed, defineComponent } from "vue";
import { useToDoStore } from "../stores/toDoStore.js";

export default defineComponent({
  name: "ToDoList",

  async setup() {
    const todoStore = useToDoStore();
    const todos = computed(() => todoStore.tasks);

    const selectedSort = ref("title");
    const selectedOrder = ref(true);
    await todoStore.fetchTasks("", selectedSort.value, selectedOrder.value);

    const searchText = ref("");
    const refreshTasks = async () => {
      await todoStore.fetchTasks(searchText.value, selectedSort.value, selectedOrder.value);
    };

    const sortTasks = async () => {
      await refreshTasks();
    };
    const newTask = ref("");
    const addTask = () => {
      if (newTask.value.trim()) {
        todoStore.addTask({
          title: newTask.value,
          isCompleted: false,
        });
        newTask.value = "";
      }
    };

    const deleteTask = async (task) => {
      await todoStore.deleteTask(task);
    };

    const updateTask = async (task) => {
      await todoStore.editTask(task);
    };

    return {
      todos,
      newTask,
      selectedSort,
      selectedOrder,
      sortTasks,
      addTask,
      deleteTask,
      updateTask,
      searchText,
      refreshTasks,
    };
  },
});
</script>

<style scoped>
.completed {
  text-decoration: line-through;
}

.sortContainer {
  display: flex;
  gap: 10px;
  align-items: center;
  margin-top: 15px;
}

.toDoListContainer {
  display: flex;
  flex-direction: column;
  gap: 10px;
  margin-top: 30px;
}

.createToDoItemContainer {
  display: flex;
  flex-direction: row;
  gap: 10px;
}

.formContainer {
  display: flex;
  flex-direction: column;
  margin-left: auto;
  margin-right: auto;
  width: 400px;
}

.createButton {
  margin-right: auto;
  margin-left: 0;
  width: 50px;
}

.toDoRow {
  display: flex;
  gap: 10px;
}

.deleteButton {
  margin-right: auto;
  margin-left: 0;
  width: 50px;
}

.updateButton {
  margin-right: auto;
  margin-left: 0;
  width: 60px;
}

.createInput {
  flex-grow: 1;
}

.editInput {
  flex-grow: 1;
}

.searchContainer {
  margin-top: 15px;
}

.searchContainer input {
  width: 100%;
}

</style>
