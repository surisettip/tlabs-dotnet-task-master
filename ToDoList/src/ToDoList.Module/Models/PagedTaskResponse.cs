using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq;
using TestApp.ToDoList.Entity;

namespace ToDoList.Module.Models
{
  public class PagedTaskResponse
{
    public IList<ToDoItem> Items { get; set; }
        = new List<ToDoItem>();

    public int TotalCount { get; set; }
}

}