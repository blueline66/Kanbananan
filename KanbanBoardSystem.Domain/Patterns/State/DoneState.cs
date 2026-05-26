using System;

namespace KanbanBoardSystem.Domain.Patterns.State
{
    public class DoneState : ITaskState
    {
        public string DisplayName => "Виконано";
        public void MoveToNext(Models.TaskItem task)
        {
            // Фішка патерну: стан Done фінальний, далі йти не можна!
            throw new InvalidOperationException("Завдання вже виконано, його статус не можна змінити.");
        }
    }
}