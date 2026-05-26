using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using KanbanBoardSystem.Domain.Models;
using KanbanBoardSystem.Domain.Patterns.State;
using KanbanBoardSystem.Domain.Common;   
using KanbanBoardSystem.Domain.Services; 
using KanbanBoardSystem.App.Services;
using KanbanBoardSystem.App.Infrastructure;

namespace KanbanBoardSystem.App
{
    public partial class Form1 : Form
    {
        // Поля підключаються до створених у Domain сервісів
        private readonly InMemRepository<TaskItem> _taskRepo = new InMemRepository<TaskItem>();
        private readonly TaskService _taskService;

        private ListBox? lstNew;
        private ListBox? lstInProgress;
        private ListBox? lstDone;
        
        private TextBox? txtTitle;
        private TextBox? txtDescription;
        private ComboBox? cmbAssignees; 
        private ComboBox? cmbTaskType; // РОЗДІЛ III: Вибір типу задачі для Фабрики
        private Button? btnAddTask;
        
        private TextBox? txtUserName;
        private Button? btnCreateUser;

        private Button? btnMoveToProgress;
        private Button? btnMoveToDone;
        private Label? lblStats; 

        public Form1()
        {
            InitializeComponent();
            _taskService = new TaskService(_taskRepo); // Зв'язуємо шари архітектури
            SetupLayout();
            LoadData(); 
        }

        private void InitializeComponent()
        {
            this.Text = "Канбан-дошка трекер завдань (ООП Практика 2026)";
            this.Size = new System.Drawing.Size(880, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void SetupLayout()
        {
            GroupBox grpUser = new GroupBox { Text = " Керування користувачами ", Location = new System.Drawing.Point(20, 10), Size = new System.Drawing.Size(820, 60) };
            Label lblUserName = new Label { Text = "Ім'я користувача:", Location = new System.Drawing.Point(15, 25), Size = new System.Drawing.Size(110, 20) };
            txtUserName = new TextBox { Location = new System.Drawing.Point(130, 22), Size = new System.Drawing.Size(150, 20) };
            btnCreateUser = new Button { Text = "➕ Створити юзера", Location = new System.Drawing.Point(290, 20), Size = new System.Drawing.Size(140, 24) };
            btnCreateUser.Click += BtnCreateUser_Click;
            grpUser.Controls.AddRange(new Control[] { lblUserName, txtUserName, btnCreateUser });

            GroupBox grpTask = new GroupBox { Text = " Нове завдання ", Location = new System.Drawing.Point(20, 80), Size = new System.Drawing.Size(820, 65) };
            Label lblTitle = new Label { Text = "Назва:", Location = new System.Drawing.Point(15, 28), Size = new System.Drawing.Size(50, 20) };
            txtTitle = new TextBox { Location = new System.Drawing.Point(65, 25), Size = new System.Drawing.Size(120, 20) };
            Label lblDesc = new Label { Text = "Опис:", Location = new System.Drawing.Point(195, 28), Size = new System.Drawing.Size(40, 20) };
            txtDescription = new TextBox { Location = new System.Drawing.Point(240, 25), Size = new System.Drawing.Size(150, 20) };
            
            // РОЗДІЛ III: Додаємо вибір типу задачі на UI для Фабрики
            Label lblType = new Label { Text = "Тип:", Location = new System.Drawing.Point(400, 28), Size = new System.Drawing.Size(35, 20) };
            cmbTaskType = new ComboBox { Location = new System.Drawing.Point(435, 25), Size = new System.Drawing.Size(85, 20), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbTaskType.Items.AddRange(new string[] { "Звичайна задача", "Помилка", "Нова функція" });
            cmbTaskType.SelectedIndex = 0;

            Label lblAssignee = new Label { Text = "Виконавець:", Location = new System.Drawing.Point(530, 28), Size = new System.Drawing.Size(75, 20) };
            cmbAssignees = new ComboBox { Location = new System.Drawing.Point(610, 25), Size = new System.Drawing.Size(100, 20), DropDownStyle = ComboBoxStyle.DropDownList };
            btnAddTask = new Button { Text = "🚀 Додати", Location = new System.Drawing.Point(720, 23), Size = new System.Drawing.Size(85, 25) };
            btnAddTask.Click += BtnAddTask_Click;
            
            grpTask.Controls.AddRange(new Control[] { lblTitle, txtTitle, lblDesc, txtDescription, lblType, cmbTaskType, lblAssignee, cmbAssignees, btnAddTask });

            Label lblNew = new Label { Text = "NEW (Нові)", Location = new System.Drawing.Point(20, 160), Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold) };
            lstNew = new ListBox { Location = new System.Drawing.Point(20, 180), Size = new System.Drawing.Size(260, 280) };
            Label lblProgress = new Label { Text = "IN PROGRESS (В роботі)", Location = new System.Drawing.Point(300, 160), Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold) };
            lstInProgress = new ListBox { Location = new System.Drawing.Point(300, 180), Size = new System.Drawing.Size(260, 280) };
            Label lblDone = new Label { Text = "DONE (Виконано)", Location = new System.Drawing.Point(580, 160), Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold) };
            lstDone = new ListBox { Location = new System.Drawing.Point(580, 180), Size = new System.Drawing.Size(260, 280) };

            btnMoveToProgress = new Button { Text = "👉 В роботу", Location = new System.Drawing.Point(20, 475), Size = new System.Drawing.Size(260, 32) };
            btnMoveToProgress.Click += BtnMoveToProgress_Click;
            btnMoveToDone = new Button { Text = "👉 Виконано", Location = new System.Drawing.Point(300, 475), Size = new System.Drawing.Size(260, 32) };
            btnMoveToDone.Click += BtnMoveToDone_Click;

            lblStats = new Label { Location = new System.Drawing.Point(20, 525), Size = new System.Drawing.Size(820, 25), Font = new System.Drawing.Font("Arial", 9, System.Drawing.FontStyle.Italic), ForeColor = System.Drawing.Color.DarkSlateGray };

            this.Controls.AddRange(new Control[] { grpUser, grpTask, lblNew, lstNew, lblProgress, lstInProgress, lblDone, lstDone, btnMoveToProgress, btnMoveToDone, lblStats });
        }

        private void LoadData()
        {
            if (cmbAssignees == null) return;

            var snapshot = StorageService.LoadBoard();

            if (snapshot == null)
            {
                cmbAssignees.Items.Add("Олексій");
                cmbAssignees.SelectedIndex = 0;
                RefreshScreenLists();
                return;
            }

            foreach (var user in snapshot.Users) cmbAssignees.Items.Add(user);
            if (cmbAssignees.Items.Count > 0) cmbAssignees.SelectedIndex = 0;

            foreach (var dto in snapshot.Tasks)
            {
                var task = new TaskItem(dto.Title, dto.Description, new User(dto.AssigneeName));
                
                if (dto.StateName == "InProgress") task.State = new InProgressState();
                else if (dto.StateName == "Done") task.State = new DoneState();
                else task.State = new NewState();

                // РОЗДІЛ III: Підписуємо завантажені таски на події сповіщення
                task.OnStateChanged += (t, msg) => {
                    MessageBox.Show($"🔔 Задача '{t.Title}': {msg}", "Повідомлення системи", MessageBoxButtons.OK, MessageBoxIcon.Information);
                };

                _taskService.CreateTask(task);
            }

            RefreshScreenLists();
        }

       // Заняття 7: Синхронізація списків через LINQ GroupBy (Українські ключі)
private void RefreshScreenLists()
{
    if (lstNew == null || lstInProgress == null || lstDone == null) return;

    lstNew.Items.Clear();
    lstInProgress.Items.Clear();
    lstDone.Items.Clear();

    var groupedTasks = _taskService.GetTasksGroupedByStatus();

    // Шукаємо по українським DisplayName, які повертають класи станів
    if (groupedTasks.ContainsKey("Нова"))
        foreach (var t in groupedTasks["Нова"]) lstNew.Items.Add(t);

    if (groupedTasks.ContainsKey("В роботі"))
        foreach (var t in groupedTasks["В роботі"]) lstInProgress.Items.Add(t);

    if (groupedTasks.ContainsKey("Виконано"))
        foreach (var t in groupedTasks["Виконано"]) lstDone.Items.Add(t);

    UpdateMetrics();
}

        // Самостійна робота 5: Використання узагальненого алгоритму Reduce
        private void UpdateMetrics()
        {
            if (lblStats == null) return;

            var tasks = _taskRepo.GetAll();
            int totalDescCharacters = GenericAlgorithms.Reduce(tasks, 0, (currentSum, task) => currentSum + task.Description.Length);

            lblStats.Text = $"📊 Статистика (Алгоритм Reduce): Загальний обсяг текстів описів усіх завдань — {totalDescCharacters} симв.";
        }

        // Самостійна робота 8: Асинхронне збереження через RetryPolicy
        private async void TriggerSaveWithRetry()
        {
            if (cmbAssignees == null) return;

            var users = new List<string>();
            foreach (var item in cmbAssignees.Items) users.Add(item.ToString()!);

            var allTasks = _taskRepo.GetAll().ToList();

            await RetryPolicy.ExecuteWithRetryAsync(async () =>
            {
                await Task.Run(() => StorageService.SaveBoard(users, allTasks));
            }, maxRetries: 3);
        }

        private void BtnCreateUser_Click(object? sender, EventArgs e)
        {
            if (txtUserName == null || cmbAssignees == null) return;

            try
            {
                var newUser = new User(txtUserName.Text);
                cmbAssignees.Items.Add(newUser.Name);
                cmbAssignees.SelectedItem = newUser.Name;
                txtUserName.Clear();
                TriggerSaveWithRetry(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка валідації", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAddTask_Click(object? sender, EventArgs e)
        {
            if (txtTitle == null || txtDescription == null || cmbAssignees == null || cmbTaskType == null) return;

            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Введіть назву завдання!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedName = cmbAssignees.SelectedItem?.ToString() ?? "Анонім";
            string taskType = cmbTaskType.SelectedItem?.ToString() ?? "Task";

            // РОЗДІЛ III: Використовуємо паттерн Фабрика для генерації об'єкта
            var newTask = KanbanBoardSystem.Domain.Factories.TaskFactory.CreateTask(taskType, txtTitle.Text, txtDescription.Text, new User(selectedName));
            
            // РОЗДІЛ III: Підписуємо нову таску на подію
            newTask.OnStateChanged += (t, msg) => {
                MessageBox.Show($"🔔 Задача '{t.Title}': {msg}", "Повідомлення системи", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            _taskService.CreateTask(newTask);

            txtTitle.Clear();
            txtDescription.Clear();
            
            RefreshScreenLists();
            TriggerSaveWithRetry(); 
        }

        private void BtnMoveToProgress_Click(object? sender, EventArgs e)
        {
            if (lstNew?.SelectedItem is TaskItem selectedTask)
            {
                try
                {
                    selectedTask.MoveNext(); // Тут автоматично вистрілить наша подія (Event)
                    RefreshScreenLists();    
                    TriggerSaveWithRetry(); 
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Помилка зміни стану", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnMoveToDone_Click(object? sender, EventArgs e)
        {
            if (lstInProgress?.SelectedItem is TaskItem selectedTask)
            {
                try
                {
                    selectedTask.MoveNext(); // Тут автоматично вистрілить наша подія (Event)
                    RefreshScreenLists();    
                    TriggerSaveWithRetry(); 
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Помилка зміни стану", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}