using Microsoft.AspNetCore.Mvc;
using TestApp.ToDoList.Entity;
using TestApp.ToDoList.Module;

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
    public IList<ToDoItem> GetTasks(
        string? search = null,
        string? sortBy = "id",
        bool ascending = true,
        string? tag = null) // NEW
    {
        var tasks = toDoListTracker.GetAllItems();

        // Search
        if (!string.IsNullOrWhiteSpace(search))
        {
            tasks = tasks
                .Where(x =>
                    x.Title.Contains(search, StringComparison.OrdinalIgnoreCase)
                    || (!string.IsNullOrWhiteSpace(x.Tags)
                        && x.Tags.Contains(search, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }


        // NEW: filter by tag
        if (!string.IsNullOrWhiteSpace(tag))
        {
            tasks = tasks
                .Where(x => !string.IsNullOrWhiteSpace(x.Tags) &&
                            x.Tags.Split(',', StringSplitOptions.TrimEntries)
                                  .Contains(tag, StringComparer.OrdinalIgnoreCase))
                .ToList();
        }

        tasks = (sortBy?.ToLower() switch
        {
            "title" => ascending ? tasks.OrderBy(x => x.Title) : tasks.OrderByDescending(x => x.Title),
            "iscompleted" => ascending
                ? tasks.OrderBy(x => x.IsCompleted).ThenBy(x => x.Title)
                : tasks.OrderByDescending(x => x.IsCompleted).ThenBy(x => x.Title),
            "createdat" => ascending ? tasks.OrderBy(x => x.CreatedAt) : tasks.OrderByDescending(x => x.CreatedAt),
            "completedat" => ascending ? tasks.OrderBy(x => x.CompletedAt) : tasks.OrderByDescending(x => x.CompletedAt),
            _ => ascending ? tasks.OrderBy(x => x.Id) : tasks.OrderByDescending(x => x.Id)
        }).ToList();

        return tasks.ToList();
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