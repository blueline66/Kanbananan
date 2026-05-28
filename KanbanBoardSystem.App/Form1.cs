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
        private readonly InMemRepository<TaskItem> _taskRepo = new InMemRepository<TaskItem>();
        private readonly TaskService _taskService;

        private ListBox? lstNew;
        private ListBox? lstInProgress;
        private ListBox? lstDone;
        
        private TextBox? txtTitle;
        private TextBox? txtDescription;
        private ComboBox? cmbAssignees; 
        private ComboBox? cmbTaskType; 
        private Button? btnAddTask;
        
        private TextBox? txtUserName;
        private Button? btnCreateUser;

        private Button? btnMoveToProgress;
        private Button? btnMoveToDone;
        private Label? lblStats; 
        private TextBox? txtSearch;

        // ЕЛЕМЕНТИ ДЛЯ ДЕДЛАЙНУ ТА ВАЖЛИВОСТІ
        private DateTimePicker? dtpDeadline;
        private ComboBox? cmbPriority;

        public Form1()
        {
            InitializeComponent();
            _taskService = new TaskService(_taskRepo); 
            SetupLayout();
            LoadData(); 
        }

        private void InitializeComponent()
        {
            this.Text = "Канбан-дошка трекер завдань (ООП Практика 2026)";
            this.Size = new System.Drawing.Size(940, 680); 
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void SetupLayout()
        {
            GroupBox grpUser = new GroupBox { Text = " Керування користувачами ", Location = new System.Drawing.Point(20, 10), Size = new System.Drawing.Size(880, 60) };
            Label lblUserName = new Label { Text = "Ім'я користувача:", Location = new System.Drawing.Point(15, 25), Size = new System.Drawing.Size(110, 20) };
            txtUserName = new TextBox { Location = new System.Drawing.Point(130, 22), Size = new System.Drawing.Size(150, 20) };
            btnCreateUser = new Button { Text = "➕ Створити юзера", Location = new System.Drawing.Point(290, 20), Size = new System.Drawing.Size(140, 24) };
            btnCreateUser.Click += BtnCreateUser_Click;
            grpUser.Controls.AddRange(new Control[] { lblUserName, txtUserName, btnCreateUser });

            GroupBox grpTask = new GroupBox { Text = " Нове завдання ", Location = new System.Drawing.Point(20, 80), Size = new System.Drawing.Size(880, 100) };
            
            Label lblTitle = new Label { Text = "Назва:", Location = new System.Drawing.Point(15, 28), Size = new System.Drawing.Size(50, 20) };
            txtTitle = new TextBox { Location = new System.Drawing.Point(65, 25), Size = new System.Drawing.Size(120, 20) };
            
            Label lblDesc = new Label { Text = "Опис:", Location = new System.Drawing.Point(195, 28), Size = new System.Drawing.Size(40, 20) };
            txtDescription = new TextBox { Location = new System.Drawing.Point(240, 25), Size = new System.Drawing.Size(140, 20) };
            
            Label lblType = new Label { Text = "Тип:", Location = new System.Drawing.Point(390, 28), Size = new System.Drawing.Size(35, 20) };
            cmbTaskType = new ComboBox { Location = new System.Drawing.Point(425, 25), Size = new System.Drawing.Size(110, 20), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbTaskType.Items.AddRange(new string[] { "Звичайна задача", "Помилка", "Нова функція" });
            cmbTaskType.SelectedIndex = 0;

            Label lblAssignee = new Label { Text = "Виконавець:", Location = new System.Drawing.Point(545, 28), Size = new System.Drawing.Size(75, 20) };
            cmbAssignees = new ComboBox { Location = new System.Drawing.Point(620, 25), Size = new System.Drawing.Size(100, 20), DropDownStyle = ComboBoxStyle.DropDownList };

            Label lblPriority = new Label { Text = "Важливість:", Location = new System.Drawing.Point(15, 63), Size = new System.Drawing.Size(75, 20) };
            cmbPriority = new ComboBox { Location = new System.Drawing.Point(95, 60), Size = new System.Drawing.Size(90, 20), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbPriority.Items.AddRange(new string[] { "Низький", "Середній", "Високий" });
            cmbPriority.SelectedIndex = 1;

            Label lblDeadline = new Label { Text = "Дедлайн:", Location = new System.Drawing.Point(200, 63), Size = new System.Drawing.Size(60, 20) };
            dtpDeadline = new DateTimePicker { Location = new System.Drawing.Point(265, 60), Size = new System.Drawing.Size(140, 20), Format = DateTimePickerFormat.Short };

            btnAddTask = new Button { Text = "🚀 Додати задачу", Location = new System.Drawing.Point(720, 23), Size = new System.Drawing.Size(140, 57) };
            btnAddTask.Click += BtnAddTask_Click;
            
            grpTask.Controls.AddRange(new Control[] { 
                lblTitle, txtTitle, lblDesc, txtDescription, lblType, cmbTaskType, lblAssignee, cmbAssignees, 
                lblPriority, cmbPriority, lblDeadline, dtpDeadline, btnAddTask 
            });

            Label lblSearch = new Label { Text = "🔍 Швидкий пошук карток:", Location = new System.Drawing.Point(20, 195), Size = new System.Drawing.Size(160, 20), Font = new System.Drawing.Font("Arial", 9, System.Drawing.FontStyle.Bold) };
            txtSearch = new TextBox { Location = new System.Drawing.Point(180, 192), Size = new System.Drawing.Size(720, 20) };
            txtSearch.TextChanged += TxtSearch_TextChanged;

            Label lblNew = new Label { Text = "NEW (Нові)", Location = new System.Drawing.Point(20, 230), Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold) };
            lstNew = new ListBox { Location = new System.Drawing.Point(20, 250), Size = new System.Drawing.Size(280, 280) };
            Label lblProgress = new Label { Text = "IN PROGRESS (В роботі)", Location = new System.Drawing.Point(320, 230), Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold) };
            lstInProgress = new ListBox { Location = new System.Drawing.Point(320, 250), Size = new System.Drawing.Size(280, 280) };
            Label lblDone = new Label { Text = "DONE (Виконано)", Location = new System.Drawing.Point(620, 230), Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold) };
            lstDone = new ListBox { Location = new System.Drawing.Point(620, 250), Size = new System.Drawing.Size(280, 280) };

            // ПІДПИСКА НА ПОДВІЙНИЙ КЛІК ДЛЯ РЕДАГУВАННЯ ЗАДАННЯ
            lstNew.DoubleClick += Lst_DoubleClick;
            lstInProgress.DoubleClick += Lst_DoubleClick;
            lstDone.DoubleClick += Lst_DoubleClick;

            btnMoveToProgress = new Button { Text = "👉 В роботу", Location = new System.Drawing.Point(20, 545), Size = new System.Drawing.Size(280, 32) };
            btnMoveToProgress.Click += BtnMoveToProgress_Click;
            btnMoveToDone = new Button { Text = "👉 Виконано", Location = new System.Drawing.Point(320, 545), Size = new System.Drawing.Size(280, 32) };
            btnMoveToDone.Click += BtnMoveToDone_Click;

            lblStats = new Label { Location = new System.Drawing.Point(20, 595), Size = new System.Drawing.Size(880, 35), Font = new System.Drawing.Font("Arial", 9, System.Drawing.FontStyle.Bold), ForeColor = System.Drawing.Color.DarkBlue };

            this.Controls.AddRange(new Control[] { grpUser, grpTask, lblSearch, txtSearch, lblNew, lstNew, lblProgress, lstInProgress, lblDone, lstDone, btnMoveToProgress, btnMoveToDone, lblStats });
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

                if (dto.Deadline != default) task.Deadline = dto.Deadline;
                if (!string.IsNullOrEmpty(dto.Priority)) task.Priority = dto.Priority;

                task.OnStateChanged += (t, msg) => {
                    MessageBox.Show($"🔔 Задача '{t.Title}': {msg}", "Повідомлення системи", MessageBoxButtons.OK, MessageBoxIcon.Information);
                };

                _taskService.CreateTask(task);
            }

            RefreshScreenLists();
        }

        private void TxtSearch_TextChanged(object? sender, EventArgs e)
        {
            RefreshScreenLists();
        }

        private void RefreshScreenLists()
        {
            if (lstNew == null || lstInProgress == null || lstDone == null || txtSearch == null) return;

            lstNew.Items.Clear();
            lstInProgress.Items.Clear();
            lstDone.Items.Clear();

            string filter = txtSearch.Text.Trim().ToLower();
            var groupedTasks = _taskService.GetTasksGroupedByStatus();

            void FillListBoxWithFilteredTasks(ListBox listBox, string statusKey)
            {
                if (!groupedTasks.ContainsKey(statusKey)) return;

                foreach (var t in groupedTasks[statusKey])
                {
                    if (!string.IsNullOrEmpty(filter) && 
                        !t.Title.ToLower().Contains(filter) && 
                        !t.Description.ToLower().Contains(filter))
                    {
                        continue;
                    }

                    string priorityMarker = "🔹 [Середня]";
                    if (t.Priority == "Високий") priorityMarker = "🔺 [ВІЗУАЛЬНО ВАЖЛИВА]";
                    else if (t.Priority == "Низький") priorityMarker = "🟢 [Низька]";

                    listBox.Items.Add($"{priorityMarker} {t.Title} ({t.Assignee?.Name ?? "Юзер"})");
                }
            }

            FillListBoxWithFilteredTasks(lstNew, "Нова");
            FillListBoxWithFilteredTasks(lstInProgress, "В роботі");
            FillListBoxWithFilteredTasks(lstDone, "Виконано");

            UpdateMetrics();
        }

        // ОНОВЛЕНИЙ ОБРОБНИК ПОДВІЙНОГО КЛІКУ: ВІКНО МОДЕРНОВОГО РЕДАГУВАННЯ КАРТКИ
        private void Lst_DoubleClick(object? sender, EventArgs e)
        {
            if (sender is ListBox listBox && listBox.SelectedItem is string selectedText)
            {
                string taskTitle = ExtractTitleFromRow(selectedText);
                var task = _taskRepo.GetAll().FirstOrDefault(t => t.Title == taskTitle);

                if (task != null)
                {
                    // Створюємо окрему діалогову форму для редагування
                    Form editForm = new Form
                    {
                        Text = $"✏️ Редагування завдання: {task.Title}",
                        Size = new System.Drawing.Size(400, 440),
                        StartPosition = FormStartPosition.CenterParent,
                        FormBorderStyle = FormBorderStyle.FixedDialog,
                        MaximizeBox = false,
                        MinimizeBox = false
                    };

                    Label lblEditTitle = new Label { Text = "Назва завдання:", Location = new System.Drawing.Point(20, 15), Size = new System.Drawing.Size(150, 20), Font = new System.Drawing.Font("Arial", 9, System.Drawing.FontStyle.Bold) };
                    TextBox txtEditTitle = new TextBox { Text = task.Title, Location = new System.Drawing.Point(20, 35), Size = new System.Drawing.Size(340, 22) };

                    Label lblEditDesc = new Label { Text = "Опис (деталі):", Location = new System.Drawing.Point(20, 70), Size = new System.Drawing.Size(150, 20), Font = new System.Drawing.Font("Arial", 9, System.Drawing.FontStyle.Bold) };
                    TextBox txtEditDesc = new TextBox { Text = task.Description, Location = new System.Drawing.Point(20, 90), Size = new System.Drawing.Size(340, 70), Multiline = true, ScrollBars = ScrollBars.Vertical };

                    Label lblEditAssignee = new Label { Text = "Виконавець:", Location = new System.Drawing.Point(20, 175), Size = new System.Drawing.Size(150, 20), Font = new System.Drawing.Font("Arial", 9, System.Drawing.FontStyle.Bold) };
                    ComboBox cmbEditAssignee = new ComboBox { Location = new System.Drawing.Point(20, 195), Size = new System.Drawing.Size(160, 22), DropDownStyle = ComboBoxStyle.DropDownList };
                    
                    // Копіюємо існуючих користувачів з головного вікна
                    if (cmbAssignees != null)
                    {
                        foreach (var item in cmbAssignees.Items) cmbEditAssignee.Items.Add(item.ToString()!);
                        cmbEditAssignee.SelectedItem = task.Assignee?.Name ?? "Анонім";
                    }

                    Label lblEditPriority = new Label { Text = "Важливість:", Location = new System.Drawing.Point(200, 175), Size = new System.Drawing.Size(150, 20), Font = new System.Drawing.Font("Arial", 9, System.Drawing.FontStyle.Bold) };
                    ComboBox cmbEditPriority = new ComboBox { Location = new System.Drawing.Point(200, 195), Size = new System.Drawing.Size(160, 22), DropDownStyle = ComboBoxStyle.DropDownList };
                    cmbEditPriority.Items.AddRange(new string[] { "Низький", "Середній", "Високий" });
                    cmbEditPriority.SelectedItem = task.Priority;

                    Label lblEditDeadline = new Label { Text = "Термін виконання (Дедлайн):", Location = new System.Drawing.Point(20, 235), Size = new System.Drawing.Size(200, 20), Font = new System.Drawing.Font("Arial", 9, System.Drawing.FontStyle.Bold) };
                    DateTimePicker dtpEditDeadline = new DateTimePicker { Value = task.Deadline, Location = new System.Drawing.Point(20, 255), Size = new System.Drawing.Size(160, 22), Format = DateTimePickerFormat.Short };

                    Button btnSave = new Button { Text = "💾 Зберегти зміни", Location = new System.Drawing.Point(20, 320), Size = new System.Drawing.Size(160, 40), DialogResult = DialogResult.OK, Font = new System.Drawing.Font("Arial", 9, System.Drawing.FontStyle.Bold) };
                    Button btnCancel = new Button { Text = "❌ Скасувати", Location = new System.Drawing.Point(200, 320), Size = new System.Drawing.Size(160, 40), DialogResult = DialogResult.Cancel };

                    editForm.Controls.AddRange(new Control[] { 
                        lblEditTitle, txtEditTitle, lblEditDesc, txtEditDesc, lblEditAssignee, cmbEditAssignee, 
                        lblEditPriority, cmbEditPriority, lblEditDeadline, dtpEditDeadline, btnSave, btnCancel 
                    });

                    editForm.AcceptButton = btnSave;
                    editForm.CancelButton = btnCancel;

                    // Якщо користувач натиснув "Зберегти"
                    if (editForm.ShowDialog() == DialogResult.OK)
                    {
                        if (string.IsNullOrWhiteSpace(txtEditTitle.Text))
                        {
                            MessageBox.Show("Назва завдання не може бути порожньою!", "Помилка валідації", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // Оновлюємо властивості об'єкта в репозиторії
                        task.Title = txtEditTitle.Text.Trim();
                        task.Description = txtEditDesc.Text.Trim();
                        task.Priority = cmbEditPriority.SelectedItem?.ToString() ?? "Середній";
                        task.Deadline = dtpEditDeadline.Value;
                        task.Assignee = new User(cmbEditAssignee.SelectedItem?.ToString() ?? "Анонім");

                        // Перемальовуємо інтерфейс та автоматично зберігаємо зміни у JSON
                        RefreshScreenLists();
                        TriggerSaveWithRetry();

                        MessageBox.Show("✅ Зміни успішно збережено!", "Система", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private string ExtractTitleFromRow(string rowText)
        {
            if (!rowText.Contains("]")) return rowText;
            string afterMarker = rowText.Split(']')[1].Trim();
            if (afterMarker.Contains("("))
            {
                int lastOpenParenthesis = afterMarker.LastIndexOf('(');
                return afterMarker.Substring(0, lastOpenParenthesis).Trim();
            }
            return afterMarker;
        }

        private void UpdateMetrics()
        {
            if (lblStats == null) return;

            var tasks = _taskRepo.GetAll().ToList();
            int totalDescCharacters = GenericAlgorithms.Reduce(tasks, 0, (currentSum, task) => currentSum + task.Description.Length);

            int total = tasks.Count;
            int newCount = tasks.Count(t => t.State is NewState);
            int inProgressCount = tasks.Count(t => t.State is InProgressState);
            int doneCount = tasks.Count(t => t.State is DoneState);

            lblStats.Text = $"📊 Статистика дошки: Всього: {total} (Нові: {newCount}, В роботі: {inProgressCount}, Виконані: {doneCount})\n" +
                            $"🧬 Обсяг описів (Алгоритм Reduce): {totalDescCharacters} симв.";
        }

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
            if (txtTitle == null || txtDescription == null || cmbAssignees == null || cmbTaskType == null || dtpDeadline == null || cmbPriority == null) return;

            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Введіть назву завдання!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedName = cmbAssignees.SelectedItem?.ToString() ?? "Анонім";
            string taskType = cmbTaskType.SelectedItem?.ToString() ?? "Task";

            var newTask = KanbanBoardSystem.Domain.Factories.TaskFactory.CreateTask(taskType, txtTitle.Text, txtDescription.Text, new User(selectedName));
            
            newTask.Deadline = dtpDeadline.Value;
            newTask.Priority = cmbPriority.SelectedItem?.ToString() ?? "Середній";

            newTask.OnStateChanged += (t, msg) => {
                MessageBox.Show($"🔔 Задача '{t.Title}': {msg}", "Повідомлення системи", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            _taskService.CreateTask(newTask);

            txtTitle.Clear();
            txtDescription.Clear();
            cmbPriority.SelectedIndex = 1;
            dtpDeadline.Value = DateTime.Now.AddDays(3);
            
            RefreshScreenLists();
            TriggerSaveWithRetry(); 
        }

        private void BtnMoveToProgress_Click(object? sender, EventArgs e)
        {
            if (lstNew?.SelectedItem is string selectedText)
            {
                string taskTitle = ExtractTitleFromRow(selectedText);
                var selectedTask = _taskRepo.GetAll().FirstOrDefault(t => t.Title == taskTitle && t.State is NewState);

                if (selectedTask != null)
                {
                    try
                    {
                        selectedTask.MoveNext(); 
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

        private void BtnMoveToDone_Click(object? sender, EventArgs e)
        {
            if (lstInProgress?.SelectedItem is string selectedText)
            {
                string taskTitle = ExtractTitleFromRow(selectedText);
                var selectedTask = _taskRepo.GetAll().FirstOrDefault(t => t.Title == taskTitle && t.State is InProgressState);

                if (selectedTask != null)
                {
                    try
                    {
                        selectedTask.MoveNext(); 
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
}