using System;
using System.Collections.Generic;
using System.Linq;
using KanbanBoardSystem.Domain.Common;
using KanbanBoardSystem.Domain.Models;

namespace KanbanBoardSystem.Domain.Services
{
    public class TaskService
    {
        private readonly InMemRepository<TaskItem> _repository;

        public TaskService(InMemRepository<TaskItem> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public void CreateTask(TaskItem task)
        {
            _repository.Add(task);
        }

       // Заняття 7: Фільтрація та декларативне LINQ GroupBy групування за DisplayName
public Dictionary<string, List<TaskItem>> GetTasksGroupedByStatus()
{
    return _repository.GetAll()
        .GroupBy(t => t.State.DisplayName) // Змінили .Name на .DisplayName
        .ToDictionary(g => g.Key, g => g.ToList());
}
    }
}
