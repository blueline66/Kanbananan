using System;
using System.Collections.Generic;

namespace KanbanBoardSystem.Domain.Common
{
    // Чистий узагальнений репозиторій (Generics - Заняття 5) без зовнішніх залежностей
    public class InMemRepository<T> where T : class
    {
        private readonly List<T> _storage = new List<T>();

        public void Add(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            _storage.Add(entity);
        }

        public IEnumerable<T> GetAll()
        {
            return _storage;
        }
    }
}