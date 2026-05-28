using System;
using KanbanBoardSystem.Domain.Common;
using KanbanBoardSystem.Domain.Patterns.State;

namespace KanbanBoardSystem.Domain.Models
{
    public class TaskItem : Entity, IDisposable
    {
        private bool _disposed = false;

        public event Action<TaskItem, string>? OnStateChanged;

        public string Title { get; set; }
        public string Description { get; set; }
        public ITaskState State { get; set; } 
        public User? Assignee { get; set; }

        
        public string Priority { get; set; } = "Середній"; 

        public TaskItem() : base()
        {
            Title = "Нове завдання";
            Description = string.Empty;
            State = new NewState(); 
            Assignee = null;
        }

        public TaskItem(string title, string description, User? assignee) : base()
        {
            Title = string.IsNullOrWhiteSpace(title) ? throw new ArgumentException("Заголовок обов'язковий") : title;
            Description = description;
            Assignee = assignee;
            State = new NewState(); 
        }

        public TaskItem(TaskItem other) : base()
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            Id = other.Id;
            Title = other.Title + " (Копія)";
            Description = other.Description;
            State = other.State; 
            Priority = other.Priority; 
            Assignee = other.Assignee != null ? new User(other.Assignee) : null;
        }

        public void MoveNext()
        {
            string oldStatusName = State.DisplayName;

            State.MoveToNext(this);

            OnStateChanged?.Invoke(this, $"Статус змінено з '{oldStatusName}' на '{State.DisplayName}'");
        }

        public DateTime Deadline { get; set; } = DateTime.Now.AddDays(3);
        
        public virtual string GetDetails()
        {
            return $"[{State.GetType().Name.Replace("State", "")}] {Title} (Важливість: {Priority}, До: {Deadline.ToShortDateString()})";
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