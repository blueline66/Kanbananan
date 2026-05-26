namespace KanbanBoardSystem.Domain.Patterns.State
{
    public interface ITaskState
    {
        string Name { get; }
        void MoveToNext(Models.TaskItem task);
    }
}