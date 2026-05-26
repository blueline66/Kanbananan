namespace KanbanBoardSystem.Domain.Patterns.State
{
    public interface ITaskState
    {
        string DisplayName { get; }
        void MoveToNext(Models.TaskItem task);
    }
}