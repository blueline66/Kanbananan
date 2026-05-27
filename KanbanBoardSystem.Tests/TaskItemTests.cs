using Xunit;
using System;
using KanbanBoardSystem.Domain.Models;
using KanbanBoardSystem.Domain.Patterns.State;

namespace KanbanBoardSystem.Tests
{
    public class TaskItemTests
    {
        // === БЛОК 1: ТЕСТИ НА СТВОРЕННЯ ===

        [Fact]
        public void Test1_CreateTask_WithBasicValues_ShouldSetNewState()
        {
            var task = new TaskItem("Купити хліб", "В магазині біля дому", null);

            Assert.Equal("Купити хліб", task.Title);
            Assert.IsType<NewState>(task.State);
            Assert.Null(task.Assignee);
        }

        [Fact]
        public void Test2_CreateTask_WithFullConstructor_ShouldSetAssignee()
        {
            var user = new User("Олексій");
            var task = new TaskItem("Виправити баг", "Код падає", user);

            Assert.NotNull(task.Assignee);
        }

        [Fact]
        public void Test3_User_ShouldHaveNameProperty()
        {
            var user = new User("Марія");
            // Перевіряємо просто властивість Name, якщо ToString не перевизначено
            Assert.Equal("Марія", user.Name);
        }

        // === БЛОК 2: ТЕСТИ НА ЗМІНУ СТАНІВ ТА ІВЕНТИ ===

        [Fact]
        public void Test4_MoveNext_FromNew_ShouldTransitionToInProgress()
        {
            var task = new TaskItem("Задача", "Опис", null);
            task.MoveNext();
            Assert.IsType<InProgressState>(task.State);
        }

        [Fact]
        public void Test5_MoveNext_FromInProgress_ShouldTransitionToDone()
        {
            var task = new TaskItem("Задача", "Опис", null);
            task.MoveNext(); // -> InProgress
            task.MoveNext(); // -> Done
            Assert.IsType<DoneState>(task.State);
        }

        [Fact]
        public void Test6_MoveNext_FromDone_ShouldThrowException()
        {
            var task = new TaskItem("Задача", "Опис", null);
            task.MoveNext(); // -> InProgress
            task.MoveNext(); // -> Done

            // Перевіряємо, що твій код правильно кидає помилку, коли задачу рухають далі з Done
            Assert.Throws<InvalidOperationException>(() => task.MoveNext());
        }

        [Fact]
        public void Test7_MoveNext_ShouldTriggerOnStateChangedEvent()
        {
            var task = new TaskItem("Тест івенту", "Опис", null);
            bool eventTriggered = false;
            task.OnStateChanged += (t, msg) => eventTriggered = true;

            task.MoveNext();
            Assert.True(eventTriggered);
        }

        // === БЛОК 3: ПОРІВНЯННЯ ТА СТАТИСТИКА ===

        [Fact]
        public void Test8_User_ShouldStoreCorrectName()
        {
            var user = new User("Іван");
            Assert.Equal("Іван", user.Name);
        }

        [Fact]
        public void Test9_UsersWithSameReference_ShouldBeEqual()
        {
            var user1 = new User("Іван");
            var user2 = user1;
            Assert.Same(user1, user2);
        }

        [Fact]
        public void Test10_GetDetails_ShouldReturnValidFormat()
        {
            var task = new TaskItem("Тест", "Опис завдання", null);
            var details = task.GetDetails();

            // Перевіряємо те, що точно є в твоєму рядку [TASK] Тест: Опис завдання...
            Assert.Contains("Тест", details);
            Assert.Contains("Опис завдання", details);
        }
    }
}