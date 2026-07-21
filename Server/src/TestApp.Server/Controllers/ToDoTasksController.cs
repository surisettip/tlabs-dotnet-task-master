#nullable enable

using Microsoft.AspNetCore.Mvc;
using TestApp.ToDoList.Entity;
using TestApp.ToDoList.Module;
using ToDoList.Module.Models;

namespace TestApp.Server.Controllers
{
  [ApiController]
  [Route("api/tasks")]
  public class ToDoTasksController : ControllerBase
  {
    private readonly IToDoListTracker toDoListTracker;

    public ToDoTasksController(IToDoListTracker toDoListTracker)
    {
      this.toDoListTracker = toDoListTracker;
    }

    [HttpGet]
    public ActionResult<PagedTaskResponse> GetTasks(
         string? search,
         string? sortBy = "id",
         bool ascending = true,
         string? tag = null,
        int pageNumber = 1,
         int pageSize = 10)
    {
      var tasks = toDoListTracker.GetTasks(
          search,
          sortBy,
          ascending,
          tag,
          pageNumber,
          pageSize);

      return Ok(tasks);
    }

    [HttpPost]
    public ToDoItem CreateTask([FromBody] ToDoItem newTask)
    {
      var task = toDoListTracker.AddItem(newTask.Title, newTask.Tags);
      return task;
    }

    [HttpPut("{id}")]
    public ToDoItem EditTask(int id, [FromBody] ToDoItem updatedTask)
    {
      var task = toDoListTracker.EditItem(id, updatedTask);
      return task;
    }

    [HttpDelete("{id}")]
    public ToDoItem DeleteTask(int id)
    {
      var task = toDoListTracker.RemoveItem(id);
      return task;
    }
  }
}