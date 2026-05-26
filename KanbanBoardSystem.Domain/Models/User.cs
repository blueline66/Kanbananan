using System;

namespace KanbanBoardSystem.Domain.Models
{
    public class User
    {
        
        public Guid Id { get; private set; }
        public string Name { get; set; }

        
        public User(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Ім'я користувача не може бути порожнім.", nameof(name));

            Id = Guid.NewGuid();
            Name = name;
        }

        
        public User(User other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            
            Id = other.Id; 
            Name = other.Name;
        }
    }
}