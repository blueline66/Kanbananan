using System;

namespace KanbanBoardSystem.Domain.Models
{
    public class BugTask : TaskItem
    {
        public string Severity { get; set; } 

        public BugTask(string title, string description, User? assignee, string severity) 
            : base(title, description, assignee) // Викликаємо конструктор батька через base
        {
            Severity = string.IsNullOrWhiteSpace(severity) ? "Minor" : severity;
        }

        
        public override string GetDetails()
        {
    
    return base.GetDetails() + $" | Критичність: {Severity}";
        }
    }
}