using System;
using System.Collections.Generic;
using System.Linq;
using TestApp.ToDoList.Entity;
using TestApp.ToDoList.Module;
using TestApp.ToDoList.Repository;
using ToDoList.Module.Models; 

namespace TestApp.ToDoList.Tracker
{
  /// <summary>
  /// Implementation of the to-do list tracking.
  /// </summary>
  public class ToDoListTracker : IToDoListTracker
  {
    private readonly IToDoItemsRepository repository;
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="repository"></param>
    public ToDoListTracker(IToDoItemsRepository repository)
    {
      this.repository = repository;
    }

    /// <inheritdoc/>
    public ToDoItem AddItem(string title, string? tags = null)
    {
      var newItem = new ToDoItem { Title = title, IsCompleted = false, Tags = tags }; 
      newItem = repository.Create(newItem);
      return newItem;
    }
    /// <inheritdoc/>
    public ToDoItem RemoveItem(int id)
    {
      var item = repository.GetItemById(id);
      if (null == item)
        throw new ArgumentException($"Item with id {id} not found");

      repository.Delete(id);
      return item;
    }
    /// <inheritdoc/>
    public ToDoItem GetItem(int id)
    {
      // Implementation for getting a specific to-do item
      var item = repository.GetItemById(id);
      if (null == item)
        throw new ArgumentException($"Item with id {id} not found");

      return item;
    }
    /// <inheritdoc/>
    public IEnumerable<ToDoItem> GetAllItems()
    {
      // Implementation for getting all to-do items
      return repository.GetAllItems().ToList();
    }
    /// <inheritdoc/>
    public ToDoItem EditItem(int id, ToDoItem updatedTask)
    {
      var item = repository.GetItemById(id);
      if (null == item)
        throw new ArgumentException($"Item with id {id} not found");

      item.Title = updatedTask.Title;
      item.IsCompleted = updatedTask.IsCompleted;
      item.CompletedAt = updatedTask.IsCompleted ? DateTime.UtcNow : null;
      item.Tags = updatedTask.Tags;
      repository.Update(item);
      return item;
    }

     /// <summary>
    /// Retrieves all to-do items with optional search, sorting, and filtering capabilities.
    /// </summary>
    /// <param name="search">Optional search term to filter items by title or tags</param>
    /// <param name="sortBy">Field to sort by: "id", "title", "iscompleted", "createdat", or "completedat". Defaults to "id"</param>
    /// <param name="ascending">Sort direction; true for ascending, false for descending</param>
    /// <param name="tag">Optional tag to filter items by</param>
    /// <param name="pageNumber">Page number for pagination; defaults to 1</param>
    /// <param name="pageSize">Number of items per page for pagination; defaults to 10</param>
    /// <returns>A list of to-do items matching the specified criteria</returns>
    public PagedTaskResponse GetTasks(
        string? search = null,
        string? sortBy = "id",
        bool ascending = true,
        string? tag = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        var tasks = GetAllItems();

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

        // Sort
        tasks = sortBy?.ToLower() switch
        {
            "title" => ascending
                ? tasks.OrderBy(x => x.Title).ToList()
                : tasks.OrderByDescending(x => x.Title).ToList(),

            "iscompleted" => ascending
                ? tasks.OrderBy(x => x.IsCompleted).ThenBy(x => x.Title).ToList()
                : tasks.OrderByDescending(x => x.IsCompleted).ThenBy(x => x.Title).ToList(),

            "createdat" => ascending
                ? tasks.OrderBy(x => x.CreatedAt).ToList()
                : tasks.OrderByDescending(x => x.CreatedAt).ToList(),

            "completedat" => ascending
                ? tasks.OrderBy(x => x.CompletedAt).ToList()
                : tasks.OrderByDescending(x => x.CompletedAt).ToList(),

            _ => ascending
                ? tasks.OrderBy(x => x.Id).ToList()
                : tasks.OrderByDescending(x => x.Id).ToList()
        };

        var totalCount = tasks.Count();

        var pagedTasks = tasks
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var result = new PagedTaskResponse
        {
            Items = pagedTasks,
            TotalCount = totalCount
        };
        return result;
    } 
  }
}