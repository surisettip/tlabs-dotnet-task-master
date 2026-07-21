<template>
  <div class="formContainer">
    <div v-if="error" class="errorMessage">
      {{ error }}
    </div>

    <div v-if="!error && todos.length === 0" class="infoMessage">No tasks found.</div>

    <h1 class="pageHeader">My To-Do List</h1>

    <div class="createToDoItemContainer">
      <input
        class="createInput"
        v-model="newTask"
        @keyup.enter="addTask"
        placeholder="Add a new todo"
      />
      <input
        class="tagInput"
        v-model="newTaskTags"
        @keyup.enter="addTask"
        placeholder="tags (comma separated)"
      />
      <button class="createButton" @click="addTask">Add</button>
    </div>

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
      <label for="tagFilter">Tag:</label>
      <select id="tagFilter" v-model="selectedTag" @change="sortTasks">
        <option value="">All</option>
        <option v-for="t in allTags" :key="t" :value="t">{{ t }}</option>
      </select>
    </div>

    <div class="chipToggleContainer">
      <label for="chipToggle">
        <input id="chipToggle" type="checkbox" v-model="showChips" />
        Show tags as chips
      </label>
    </div>

    <div class="searchContainer">
      <input
        v-model="searchText"
        @input="
          currentPage = 1;
          refreshTasks();
        "
        placeholder="Search tasks..."
      />
    </div>

    <div class="columnHeaderRow">
      <span class="colStatus"> </span>
      <span class="colTitle">Title</span>
      <span class="colTags">Tags</span>
      <span class="colActions">Actions</span>
    </div>

    <div class="toDoListContainer">
      <div v-for="task in todos" :key="task.id">
        <div class="toDoRow">
          <input type="checkbox" @change="updateTask(task)" v-model="task.isCompleted" />
          <input
            :class="{ completed: task.isCompleted, editInput: true }"
            v-model="task.title"
            @blur="updateTask(task)"
            @keyup.enter="updateTask(task)"
          />

          <input
            class="tagInput"
            v-model="task.tags"
            placeholder="tags"
            @blur="updateTask(task)"
            @keyup.enter="updateTask(task)"
          />

          <button class="deleteButton" @click="deleteTask(task)">Delete</button>
        </div>

        <div class="tagChips" v-if="showChips && task.tags">
          <span
            class="chip"
            v-for="t in task.tags
              .split(',')
              .map((s) => s.trim())
              .filter(Boolean)"
            :key="t"
          >
            {{ t }}
          </span>
        </div>
      </div>
      <div class="pagination">
        <button
          :disabled="currentPage === 1"
          @click="
            currentPage--;
            refreshTasks();
          "
        >
          Previous
        </button>

        <span>Page {{ currentPage }}</span>

        <label>Page Size:</label>

        <select
          v-model="pageSize"
          @change="
            currentPage = 1;
            refreshTasks();
          "
        >
          <option :value="5">5</option>
          <option :value="10">10</option>
          <option :value="20">20</option>
        </select>

        <button
          :disabled="!hasNextPage"
          @click="
            currentPage++;
            refreshTasks();
          "
        >
          Next
        </button>
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
    const error = computed(() => todoStore.error);
    const totalCount = computed(() => todoStore.totalCount);
    const currentPage = ref(1);
    const pageSize = ref(5);

    const hasNextPage = computed(() => {
      console.log(`Current Page: ${currentPage.value}`);
      console.log(`Page Size: ${pageSize.value}`);

      const hasMore = currentPage.value * pageSize.value < totalCount.value;

      console.log(`Current Page: ${currentPage.value}`);
      console.log(`Page Size: ${pageSize.value}`);
      console.log(`Total Count: ${totalCount.value}`);
      console.log(`Has Next Page: ${hasMore}`);

      return hasMore;
    });

    const selectedSort = ref("title");
    const selectedOrder = ref(true);
    const selectedTag = ref("");
    const searchText = ref("");
    const newTask = ref("");
    const newTaskTags = ref("");
    const showChips = ref(false);

    const refreshTasks = async () => {
      const ascendingBool = selectedOrder.value === true || selectedOrder.value === "true";

      await todoStore.fetchTasks(
        searchText.value,
        selectedSort.value,
        ascendingBool,
        selectedTag.value,
        currentPage.value,
        pageSize.value,
      );

      console.log("Store Total Count:", todoStore.totalCount);
    };

    await refreshTasks();

    const sortTasks = async () => {
      currentPage.value = 1;
      await refreshTasks();
    };

    const allTags = computed(() => {
      const tagSet = new Set();
      todos.value.forEach((t) => {
        (t.tags || "")
          .split(",")
          .map((s) => s.trim())
          .filter(Boolean)
          .forEach((tag) => tagSet.add(tag));
      });
      return Array.from(tagSet).sort();
    });

    const addTask = async () => {
      await todoStore.addTask({
        title: newTask.value,
        isCompleted: false,
        tags: newTaskTags.value.trim(),
      });

      if (todoStore.error) {
        return;
      }

      await refreshTasks();

      newTask.value = "";
      newTaskTags.value = "";
    };

    const deleteTask = async (task) => {
      await todoStore.deleteTask(task);

      const remainingItems = totalCount.value - 1;

      const maxPage = Math.max(1, Math.ceil(remainingItems / pageSize.value));

      if (currentPage.value > maxPage) {
        currentPage.value = maxPage;
      }

      await refreshTasks();
    };

    const updateTask = async (task) => {
      await todoStore.editTask(task);
      await refreshTasks();
    };

    return {
      todos,
      newTask,
      newTaskTags,
      selectedSort,
      selectedOrder,
      selectedTag,
      allTags,
      sortTasks,
      addTask,
      deleteTask,
      updateTask,
      searchText,
      refreshTasks,
      showChips,
      error,
      todoStore,
      currentPage,
      pageSize,
      hasNextPage,
    };
  },
});
</script>

<style scoped>
.pageHeader {
  text-align: center;
  margin-bottom: 8px;
}
.completed {
  text-decoration: line-through;
}
.sortContainer {
  display: flex;
  gap: 10px;
  align-items: center;
  margin-top: 15px;
}
.chipToggleContainer {
  margin-top: 10px;
  font-size: 14px;
}
.chipToggleContainer label {
  display: flex;
  align-items: center;
  gap: 6px;
  cursor: pointer;
}

.columnHeaderRow {
  display: flex;
  gap: 10px;
  margin-top: 20px;
  padding: 0 2px;
  font-weight: bold;
  font-size: 13px;
  color: #555;
  border-bottom: 1px solid #ccc;
  padding-bottom: 6px;
}
.colStatus {
  width: 20px;
}
.colTitle {
  flex-grow: 1;
}
.colTags {
  width: 120px;
}
.colActions {
  width: 50px;
}

.toDoListContainer {
  display: flex;
  flex-direction: column;
  gap: 10px;
  margin-top: 10px;
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
.createInput {
  flex-grow: 1;
}
.editInput {
  flex-grow: 1;
}
.tagInput {
  width: 120px;
}
.searchContainer {
  margin-top: 15px;
}
.searchContainer input {
  width: 100%;
}
.tagChips {
  display: flex;
  gap: 6px;
  margin: 4px 0 8px 0;
}
.chip {
  background: #eee;
  border-radius: 12px;
  padding: 2px 8px;
  font-size: 12px;
}

.errorMessage {
  margin-top: 10px;
  padding: 10px;

  color: #842029;
  background-color: #f8d7da;
  border: 1px solid #f5c2c7;
  border-radius: 4px;

  font-size: 14px;
}

.infoMessage {
  margin-top: 10px;
  padding: 10px;

  color: #055160;
  background-color: #cff4fc;
  border: 1px solid #b6effb;
  border-radius: 4px;

  font-size: 14px;
}
.pagination {
  margin-top: 20px;
  display: flex;
  gap: 10px;
  align-items: center;
}
</style>
