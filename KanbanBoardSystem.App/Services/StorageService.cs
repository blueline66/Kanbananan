using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using KanbanBoardSystem.Domain.Models;
using KanbanBoardSystem.Domain.Patterns.State;

namespace KanbanBoardSystem.App.Services
{
    public class TaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string StateName { get; set; } = "New";
        public string AssigneeName { get; set; } = "Немає";

        // НОВІ ПОЛЯ ДЛЯ ПІДТРИМКИ ДЕДЛАЙНІВ ТА ВАЖЛИВОСТІ
        public DateTime Deadline { get; set; }
        public string Priority { get; set; } = "Середній";
    }

    public class BoardSnapshot
    {
        public List<string> Users { get; set; } = new List<string>();
        public List<TaskDto> Tasks { get; set; } = new List<TaskDto>();
    }

    public static class StorageService
    {
        private static readonly string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "kanban.json");

        public static void SaveBoard(List<string> users, List<TaskItem> allTasks)
        {
            var snapshot = new BoardSnapshot { Users = users };

            foreach (var task in allTasks)
            {
                snapshot.Tasks.Add(new TaskDto
                {
                    Title = task.Title,
                    Description = task.Description,
                    StateName = task.State.GetType().Name.Replace("State", ""),
                    AssigneeName = task.Assignee?.Name ?? "Немає",
                    
                    // ЗБЕРІГАЄМО НОВІ ДАНІ У ФАЙЛ JSON
                    Deadline = task.Deadline,
                    Priority = task.Priority
                });
            }

            string json = JsonSerializer.Serialize(snapshot, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }

        public static BoardSnapshot? LoadBoard()
        {
            if (!File.Exists(FilePath)) return null;

            try
            {
                string json = File.ReadAllText(FilePath);
                return JsonSerializer.Deserialize<BoardSnapshot>(json);
            }
            catch
            {
                return null; 
            }
        }
    }
}