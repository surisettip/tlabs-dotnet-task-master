using System.Collections.Generic;
using TestApp.ToDoList.Entity;
using ToDoList.Module.Models;

namespace TestApp.ToDoList.Module
{
  /// <summary>
  /// Tracking to-do list items.
  /// </summary>
  public interface IToDoListTracker
  {
    /// <summary>
    /// Adds a new to-do item.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="tags"></param>
    /// <returns></returns>
    ToDoItem AddItem(string title, string? tags = null);
    /// <summary>
    /// Removes a to-do item.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    ToDoItem RemoveItem(int id);
    /// <summary>
    /// Gets a to-do item by its Id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    ToDoItem GetItem(int id);
    /// <summary>
    /// Gets all to-do items.
    /// </summary>
    /// <returns></returns>
    IEnumerable<ToDoItem> GetAllItems();
    /// <summary>
    /// Edits a to-do item.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="updatedTask"></param>
    /// <returns></returns>
    ToDoItem EditItem(int id, ToDoItem updatedTask);
        /// <summary>
    /// Gets to-do items with optional search, sorting, and filtering.
    /// </summary>
    /// <param name="search">Optional search term to filter items.</param>
    /// <param name="sortBy">Field to sort by (default: "id").</param>
    /// <param name="ascending">Sort order (default: true for ascending).</param>
    /// <param name="pageNumber">Page number for pagination (default: 1).</param>
    /// <param name="pageSize">Number of items per page for pagination (default: 10).</param>
    /// <returns>List of to-do items.</returns>
    PagedTaskResponse GetTasks(
            string? search = null,
            string? sortBy = "id",
            bool ascending = true,
            string? tag = null,
            int pageNumber = 1,
            int pageSize = 10 );
  }
}