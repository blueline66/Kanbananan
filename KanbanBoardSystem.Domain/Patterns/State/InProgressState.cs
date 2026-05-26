namespace KanbanBoardSystem.Domain.Patterns.State
{
    public class InProgressState : ITaskState
    {
        public string Name => "InProgress";
        public void MoveToNext(Models.TaskItem task)
        {
            task.State = new DoneState(); // З InProgress переходимо в Done
        }
    }
}