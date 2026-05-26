using System;
using System.Collections.Generic;
using KanbanBoardSystem.Domain.Common;

namespace KanbanBoardSystem.Domain.Models
{
    public class Epic : Entity
    {
        private readonly List<TaskItem> _subTasks = new List<TaskItem>();
        private string _title = string.Empty; 
        
        public string Title
        {
            get => _title;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Заголовок епіка не може бути порожнім.");
                _title = value;
            }
        }

        public Epic(string title) : base()
        {
            Title = title;
        }

        public static Epic operator +(Epic epic, TaskItem task)
        {
            if (epic == null) throw new ArgumentNullException(nameof(epic));
            if (task == null) throw new ArgumentNullException(nameof(task));

            if (!epic._subTasks.Contains(task))
            {
                epic._subTasks.Add(task);
            }
            return epic;
        }

        public TaskItem this[int index]
        {
            get
            {
                if (index < 0 || index >= _subTasks.Count)
                    throw new IndexOutOfRangeException("Некоректний індекс підзадачі.");
                return _subTasks[index];
            }
        }

        public TaskItem this[string title]
        {
            get
            {
                var task = _subTasks.Find(t => t.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
                if (task == null)
                    throw new KeyNotFoundException($"Підзадачу з назвою '{title}' не знайдено в епіку.");
                return task;
            }
        }

        public int SubTasksCount => _subTasks.Count;
    }
}