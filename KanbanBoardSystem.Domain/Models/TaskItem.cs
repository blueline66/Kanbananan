using System;
using KanbanBoardSystem.Domain.Common;
using KanbanBoardSystem.Domain.Patterns.State;

namespace KanbanBoardSystem.Domain.Models
{
    public class TaskItem : Entity, IDisposable
    {
        private bool _disposed = false;

        // РОЗДІЛ III: Подія для сповіщення форми (українською)
        public event Action<TaskItem, string>? OnStateChanged;

        public string Title { get; set; }
        public string Description { get; set; }
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
            State = other.State; 
            Assignee = other.Assignee != null ? new User(other.Assignee) : null;
        }

        // Метод зміни стану з підтримкою українських DisplayName подій
        public void MoveNext()
        {
            // 1. Запам'ятовуємо українську назву поточного стану
            string oldStatusName = State.DisplayName;

            // 2. Патерн State переводить задачу далі
            State.MoveToNext(this);

            // 3. Смикаємо подію (тепер повністю українською)
            OnStateChanged?.Invoke(this, $"Статус змінено з '{oldStatusName}' на '{State.DisplayName}'");
        }

        // Виправлений метод: використовує DisplayName замість неіснуючого Name
        public virtual string GetDetails()
        {
            return $"[TASK] {Title}: {Description} (Статус: {State.DisplayName})";
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