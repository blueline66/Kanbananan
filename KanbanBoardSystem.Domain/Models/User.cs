using System;

namespace KanbanBoardSystem.Domain.Models
{
    public class User
    {
        private string _name = string.Empty; 

        public Guid Id { get; private set; }

        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value) || value.Trim().Length < 2)
                    throw new ArgumentException("Ім'я користувача повинно містити принаймні 2 символи.");
                
                _name = value.Trim();
            }
        }

        public User(string name)
        {
            Id = Guid.NewGuid();
            Name = name; 
        }

        public User(User other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            Id = other.Id;
            Name = other.Name;
        }

        public static bool operator ==(User? left, User? right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left is null || right is null) return false;
            return left.Id == right.Id;
        }

        public static bool operator !=(User? left, User? right)
        {
            return !(left == right);
        }

        public override bool Equals(object? obj)
        {
            return obj is User user && this == user;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}