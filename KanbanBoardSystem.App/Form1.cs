using System;
using System.Collections.Generic;
using System.Windows.Forms;
using KanbanBoardSystem.Domain.Models;
using KanbanBoardSystem.Domain.Patterns.State;
using KanbanBoardSystem.App.Services;

namespace KanbanBoardSystem.App
{
    public partial class Form1 : Form
    {
        private ListBox? lstNew;
        private ListBox? lstInProgress;
        private ListBox? lstDone;
        
        private TextBox? txtTitle;
        private TextBox? txtDescription;
        private ComboBox? cmbAssignees; 
        private Button? btnAddTask;
        
        private TextBox? txtUserName;
        private Button? btnCreateUser;

        private Button? btnMoveToProgress;
        private Button? btnMoveToDone;

        public Form1()
        {
            InitializeComponent();
            SetupLayout();
            LoadData(); // Завантажуємо збережені дані замість демо-даних
        }

        private void InitializeComponent()
        {
            this.Text = "Канбан-дошка трекер завдань (ООП Практика 2026)";
            this.Size = new System.Drawing.Size(880, 560);
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
            txtTitle = new TextBox { Location = new System.Drawing.Point(65, 25), Size = new System.Drawing.Size(140, 20) };
            Label lblDesc = new Label { Text = "Опис:", Location = new System.Drawing.Point(220, 28), Size = new System.Drawing.Size(45, 20) };
            txtDescription = new TextBox { Location = new System.Drawing.Point(270, 25), Size = new System.Drawing.Size(180, 20) };
            Label lblAssignee = new Label { Text = "Виконавець:", Location = new System.Drawing.Point(470, 28), Size = new System.Drawing.Size(80, 20) };
            cmbAssignees = new ComboBox { Location = new System.Drawing.Point(555, 25), Size = new System.Drawing.Size(120, 20), DropDownStyle = ComboBoxStyle.DropDownList };
            btnAddTask = new Button { Text = "🚀 Додати задачу", Location = new System.Drawing.Point(690, 23), Size = new System.Drawing.Size(115, 25) };
            btnAddTask.Click += BtnAddTask_Click;
            grpTask.Controls.AddRange(new Control[] { lblTitle, txtTitle, lblDesc, txtDescription, lblAssignee, cmbAssignees, btnAddTask });

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

            this.Controls.AddRange(new Control[] { grpUser, grpTask, lblNew, lstNew, lblProgress, lstInProgress, lblDone, lstDone, btnMoveToProgress, btnMoveToDone });
        }

        // Завантаження даних з JSON файлу
        private void LoadData()
        {
            if (cmbAssignees == null || lstNew == null || lstInProgress == null || lstDone == null) return;

            var snapshot = StorageService.LoadBoard();

            if (snapshot == null)
            {
                // Якщо файлу немає, створюємо дефолтного юзера
                cmbAssignees.Items.Add("Олексій");
                cmbAssignees.SelectedIndex = 0;
                return;
            }

            // Відновлюємо користувачів
            foreach (var user in snapshot.Users)
            {
                cmbAssignees.Items.Add(user);
            }
            if (cmbAssignees.Items.Count > 0) cmbAssignees.SelectedIndex = 0;

            // Відновлюємо задачі та розкладаємо по колонках
            foreach (var dto in snapshot.Tasks)
            {
                var task = new TaskItem(dto.Title, dto.Description, new User(dto.AssigneeName));
                
                // Штучно виставляємо потрібний стан
                if (dto.StateName == "InProgress")
                {
                    task.State = new InProgressState();
                    lstInProgress.Items.Add(task);
                }
                else if (dto.StateName == "Done")
                {
                    task.State = new DoneState();
                    lstDone.Items.Add(task);
                }
                else
                {
                    task.State = new NewState();
                    lstNew.Items.Add(task);
                }
            }
        }

        // Хелпер для збору всіх тасок з форми з метою збереження
        private void TriggerSave()
        {
            if (cmbAssignees == null || lstNew == null || lstInProgress == null || lstDone == null) return;

            var users = new List<string>();
            foreach (var item in cmbAssignees.Items) users.Add(item.ToString()!);

            var allTasks = new List<TaskItem>();
            foreach (var item in lstNew.Items) allTasks.Add((TaskItem)item);
            foreach (var item in lstInProgress.Items) allTasks.Add((TaskItem)item);
            foreach (var item in lstDone.Items) allTasks.Add((TaskItem)item);

            StorageService.SaveBoard(users, allTasks);
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
                TriggerSave(); // Хранилище оновлено
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка валідації", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAddTask_Click(object? sender, EventArgs e)
        {
            if (txtTitle == null || txtDescription == null || lstNew == null || cmbAssignees == null) return;

            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Введіть назву завдання!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedName = cmbAssignees.SelectedItem?.ToString() ?? "Анонім";
            var newTask = new TaskItem(txtTitle.Text, txtDescription.Text, new User(selectedName));
            lstNew.Items.Add(newTask);

            txtTitle.Clear();
            txtDescription.Clear();
            TriggerSave(); // Зберігаємо нову таску
        }

        private void BtnMoveToProgress_Click(object? sender, EventArgs e)
        {
            if (lstNew == null || lstInProgress == null) return;

            if (lstNew.SelectedItem is TaskItem selectedTask)
            {
                try
                {
                    lstNew.Items.Remove(selectedTask);
                    selectedTask.MoveNext(); 
                    lstInProgress.Items.Add(selectedTask);
                    TriggerSave(); // Зберігаємо зміну стану
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnMoveToDone_Click(object? sender, EventArgs e)
        {
            if (lstInProgress == null || lstDone == null) return;

            if (lstInProgress.SelectedItem is TaskItem selectedTask)
            {
                try
                {
                    lstInProgress.Items.Remove(selectedTask);
                    selectedTask.MoveNext(); 
                    lstDone.Items.Add(selectedTask);
                    TriggerSave(); // Зберігаємо зміну стану
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}