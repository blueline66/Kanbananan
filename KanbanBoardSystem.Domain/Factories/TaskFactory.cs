using System;
using KanbanBoardSystem.Domain.Models;

namespace KanbanBoardSystem.Domain.Factories
{
    public static class TaskFactory
    {
        
        public static TaskItem CreateTask(string type, string title, string description, User assignee)
        {
            return type.ToLower() switch
            {
                
                "bug" => new BugTask(title, description, assignee, "Normal"),
                
                
                "feature" => new FeatureTask(title, description, assignee, 1),
                
                
                _ => new TaskItem(title, description, assignee) 
            };
        }
    }
}