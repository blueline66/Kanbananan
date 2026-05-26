namespace KanbanBoardSystem.Domain.Patterns.State
{
    public class NewState : ITaskState
    {
       public string DisplayName => "Нова";
        public void MoveToNext(Models.TaskItem task)
        {
            task.State = new InProgressState(); // З New переходимо в InProgress
        }
    }
}