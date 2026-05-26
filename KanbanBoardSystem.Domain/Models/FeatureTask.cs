using System;

namespace KanbanBoardSystem.Domain.Models
{
    public class FeatureTask : TaskItem
    {
        public int StoryPoints { get; set; }

       
        public FeatureTask(string title, string description, User? assignee, int storyPoints) 
            : base(title, description, assignee)
        {
            StoryPoints = storyPoints < 0 ? 0 : storyPoints;
        }

        
        public new string GetDetails()
        {
            return $"[FEATURE] {Title} ({StoryPoints} SP) — {Description}";
        }
    }
}