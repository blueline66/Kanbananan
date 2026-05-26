using System;
using KanbanBoardSystem.Domain.Common;
using KanbanBoardSystem.Domain.Patterns.State;

namespace KanbanBoardSystem.Domain.Models
{
    public class TaskItem : Entity, IDisposable
    {
        private bool _disposed = false;

        public string Title { get; set; }
        public string Description { get; set; }
        
        // Використовуємо інтерфейс стану замість звичайного рядка!
        public ITaskState State { get; set; } 
        public User? Assignee { get; set; }

        // Конструктор за замовчуванням
        public TaskItem() : base()
        {
            Title = "Нове завдання";
            Description = string.Empty;
            State = new NewState(); // Початковий стан
            Assignee = null;
        }

        // Основний конструктор
        public TaskItem(string title, string description, User? assignee) : base()
        {
            Title = string.IsNullOrWhiteSpace(title) ? throw new ArgumentException("Заголовок обов'язковий") : title;
            Description = description;
            Assignee = assignee;
            State = new NewState(); // Початковий стан
        }

        // Копіювальний конструктор
        public TaskItem(TaskItem other) : base()
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            Id = other.Id;
            Title = other.Title + " (Копія)";
            Description = other.Description;
            State = other.State; // Копіюємо поточний стан
            Assignee = other.Assignee != null ? new User(other.Assignee) : null;
        }

        // Метод для просування задачі вперед по канбан-дошці за допомогою патерну State
        public void MoveNext()
        {
            State.MoveToNext(this);
        }

        public virtual string GetDetails()
        {
            return $"[TASK] {Title}: {Description} (Статус: {State.Name})";
        }

        public override string ToString()
        {
            return $"{Title} (Користувач: {Assignee?.Name ?? "Немає"})";
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                }
                _disposed = true;
            }
        }
    }
}