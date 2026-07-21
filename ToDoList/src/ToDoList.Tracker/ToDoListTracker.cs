using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
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
    private readonly IMemoryCache cache;
    private static int cacheVersion = 0; // Change this value to invalidate the cache
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="repository"></param>
    /// <param name="cache"></param>
    public ToDoListTracker(IToDoItemsRepository repository, IMemoryCache cache)
    {
      this.repository = repository;
      this.cache = cache;
    }

    /// <inheritdoc/>
    public ToDoItem AddItem(string title, string? tags = null)
    {
      // Implementation for adding a to-do item
      if (string.IsNullOrWhiteSpace(title))
        throw new ArgumentException("Title cannot be null or empty", nameof(title));

      var newItem = new ToDoItem { Title = title, IsCompleted = false, Tags = tags ?? string.Empty };
      newItem = repository.Create(newItem);

      cacheVersion++;

      Console.WriteLine(
            $"[CACHE INVALIDATED] AddItem | New Cache Version = {cacheVersion}");

      if (newItem == null)
        throw new InvalidOperationException("Failed to create item in repository");

      return newItem;
    }
    /// <inheritdoc/>
    public ToDoItem RemoveItem(int id)
    {
      var item = repository.GetItemById(id);
      if (null == item)
        throw new ArgumentException($"Item with id {id} not found");

      repository.Delete(id);

      cacheVersion++;
      Console.WriteLine(
            $"[CACHE INVALIDATED] RemoveItem | New Cache Version = {cacheVersion}");
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
      var allItems = repository.GetAllItems().ToList();
      Console.WriteLine($"REPOSITORY COUNT = {allItems.Count}");
      return allItems;
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
      cacheVersion++;
      Console.WriteLine(
            $"[CACHE INVALIDATED] EditItem | New Cache Version = {cacheVersion}");
      return item;
    }

    /// <summary>
    /// Retrieves all to-do items with optional search, sorting, and filtering capabilities.
    /// </summary>
    /// <param name="search">Optional search term to filter items by title or tags</param>
    /// <param name="sortBy">Field to sort by: "id", "title", "iscompleted", "createdat", or "completedat". Defaults to "id"</param>
    /// <param name="ascending">Sort direction; true for ascending, false for descending</param>
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
        string cacheKey = $"tasks-{cacheVersion}";
        Console.WriteLine($"[CACHE LOOKUP] cacheKey: {cacheKey}");

        if (!cache.TryGetValue(
            cacheKey,
            out List<ToDoItem>? tasks))
        {
            Console.WriteLine($"[CACHE MISS] cacheKey: {cacheKey}");
            var sw = System.Diagnostics.Stopwatch.StartNew();

            tasks = repository
                .GetAllItems()
                .ToList();

            sw.Stop();

            Console.WriteLine(
                $"DB Call: {sw.ElapsedMilliseconds}ms");
                
            cache.Set(
                cacheKey,
                tasks,
                TimeSpan.FromMinutes(5));

            Console.WriteLine($"[CACHE STORE] cacheKey: {cacheKey}");
        }
        else
        {
            Console.WriteLine($"[CACHE HIT] cacheKey: {cacheKey}");
        }

        IEnumerable<ToDoItem> result = tasks;

        // Search
        if (!string.IsNullOrWhiteSpace(search))
        {
            result = result.Where(x =>
                x.Title.Contains(
                    search,
                    StringComparison.OrdinalIgnoreCase)
                ||
                (!string.IsNullOrWhiteSpace(x.Tags)
                    &&
                    x.Tags.Contains(
                        search,
                        StringComparison.OrdinalIgnoreCase)));
        }
           // NEW: filter by tag
        if (!string.IsNullOrWhiteSpace(tag))
        {
            result = result
                .Where(x => !string.IsNullOrWhiteSpace(x.Tags) &&
                            x.Tags.Split(',', StringSplitOptions.TrimEntries)
                                  .Contains(tag, StringComparer.OrdinalIgnoreCase))
                .ToList();
        }

        // Sort
        result = sortBy?.ToLower() switch
        {
            "title" => ascending
                ? result.OrderBy(x => x.Title)
                : result.OrderByDescending(x => x.Title),

            "iscompleted" => ascending
                ? result.OrderBy(x => x.IsCompleted)
                        .ThenBy(x => x.Title)
                : result.OrderByDescending(x => x.IsCompleted)
                        .ThenBy(x => x.Title),

            "createdat" => ascending
                ? result.OrderBy(x => x.CreatedAt)
                : result.OrderByDescending(x => x.CreatedAt),

            "completedat" => ascending
                ? result.OrderBy(x => x.CompletedAt)
                : result.OrderByDescending(x => x.CompletedAt),

            _ => ascending
                ? result.OrderBy(x => x.Id)
                : result.OrderByDescending(x => x.Id)
        };

        var totalCount = result.Count();

        var pagedTasks = result
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        Console.WriteLine(
            $"TOTAL COUNT = {totalCount}, PAGED COUNT = {pagedTasks.Count}");
        Console.WriteLine("-----------------------------------------------------\n");
        return new PagedTaskResponse
        {
            Items = pagedTasks,
            TotalCount = totalCount
        };
    }
  }
}