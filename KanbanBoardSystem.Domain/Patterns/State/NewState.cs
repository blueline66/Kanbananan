namespace KanbanBoardSystem.Domain.Patterns.State
{
    public class NewState : ITaskState
    {
        public string Name => "New";
        public void MoveToNext(Models.TaskItem task)
        {
            task.State = new InProgressState(); // З New переходимо в InProgress
        }
    }
}