using System;

namespace KanbanBoardSystem.Domain.Exceptions
{
    public class TaskDomainException : Exception
    {
        public TaskDomainException(string message) : base(message) { }
    }
}