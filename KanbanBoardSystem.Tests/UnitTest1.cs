using Xunit;
using KanbanBoardSystem.Domain.Models;

namespace KanbanBoardSystem.Tests
{
    public class EpicTests
    {
        [Fact]
        public void AddTaskToEpic_ShouldIncreaseSubTasksCount_AAA_Structure()
        {
            // Arrange (Налаштування: створюємо епік, користувача та задачу)
            var epic = new Epic("Розробка ядра системи");
            var user = new User("Олексій");
            var task = new TaskItem("Написати репозиторій", "Реалізувати інтерфейс IRepository", user);

            // Act (Дія: додаємо задачу до епіка через наш перевантажений оператор +)
            epic = epic + task;

            // Assert (Перевірка результату)
            Assert.Equal(1, epic.SubTasksCount);
            Assert.Equal(task, epic[0]); // Перевірка роботи індексатора
        }
    }
}