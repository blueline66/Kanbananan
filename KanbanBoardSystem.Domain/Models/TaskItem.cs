using System;

namespace KanbanBoardSystem.Domain.Models
{
    public class TaskItem : IDisposable
    {
        private bool _disposed = false;

        public Guid Id { get; private set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } 
        public User Assignee { get; set; }

        
        public TaskItem()
        {
            Id = Guid.NewGuid();
            Title = "Нове завдання";
            Description = string.Empty;
            Status = "New";
        }

        
        public TaskItem(string title, string description, User assignee)
        {
            Id = Guid.NewGuid();
            Title = string.IsNullOrWhiteSpace(title) ? throw new ArgumentException("Заголовок обов'яковий") : title;
            Description = description;
            Assignee = assignee ?? throw new ArgumentNullException(nameof(assignee));
            Status = "New";
        }

      
        public TaskItem(TaskItem other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            Id = other.Id;
            Title = other.Title + " (Копія)";
            Description = other.Description;
            Status = other.Status;
            Assignee = new User(other.Assignee); // Глибоке копіювання користувача через його копіювальний конструктор
        }

       
        ~TaskItem()
        {
            Dispose(false);
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