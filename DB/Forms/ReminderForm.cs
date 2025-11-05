using System;
using System.Windows.Forms;
using MarketingCRMSystem.Managers;

namespace DB.Forms
{
    public partial class ReminderForm : Form
    {
        private ReminderManager reminderManager;

        public ReminderForm()
        {
            InitializeComponent();
            reminderManager = new ReminderManager();

            // Налаштування DataGridView
            SetupDataGridViews();

            // Завантаження даних
            LoadReminders();
        }

        private void SetupDataGridViews()
        {
            // Налаштування для всіх DataGridView
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dataGridView2.ReadOnly = true;
            dataGridView2.AllowUserToAddRows = false;
            dataGridView2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dataGridView3.ReadOnly = true;
            dataGridView3.AllowUserToAddRows = false;
            dataGridView3.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void LoadReminders()
        {
            try
            {
                // Вкладка 1: Неактивні клієнти (понад 30 днів без контактів)
                var inactiveClients = reminderManager.GetInactiveClients(30);
                dataGridView1.DataSource = inactiveClients;

                // Вкладка 2: Пропозиції, що закінчуються протягом 7 днів
                var expiringProposals = reminderManager.GetExpiringProposals(7);
                dataGridView2.DataSource = expiringProposals;

                // Вкладка 3: Попередження про бюджет кампаній (80%+)
                var budgetAlerts = reminderManager.GetCampaignBudgetAlerts();
                dataGridView3.DataSource = budgetAlerts;

                // Оновити назви вкладок з кількістю записів
                UpdateTabTitles(inactiveClients.Rows.Count,
                               expiringProposals.Rows.Count,
                               budgetAlerts.Rows.Count);

                // Підсвітити важливі рядки
                HighlightCriticalRows();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження нагадувань: {ex.Message}\n\n{ex.StackTrace}",
                              "Помилка",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }

        private void UpdateTabTitles(int inactiveCount, int expiringCount, int budgetCount)
        {
            tabPage1.Text = $"Неактивні клієнти ({inactiveCount})";
            tabPage2.Text = $"Пропозиції закінчуються ({expiringCount})";
            tabPage3.Text = $"Попередження про бюджет ({budgetCount})";

            // Змінити колір вкладки, якщо є критичні нагадування
            if (inactiveCount > 0 || expiringCount > 0 || budgetCount > 0)
            {
                this.Text = $"Нагадування ({inactiveCount + expiringCount + budgetCount})";
            }
            else
            {
                this.Text = "Нагадування - немає";
            }
        }

        private void HighlightCriticalRows()
        {
            // Підсвітити клієнтів без контактів понад 60 днів червоним
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["DaysSinceContact"].Value != DBNull.Value)
                {
                    int days = Convert.ToInt32(row.Cells["DaysSinceContact"].Value);
                    if (days > 60)
                    {
                        row.DefaultCellStyle.BackColor = System.Drawing.Color.LightCoral;
                    }
                    else if (days > 30)
                    {
                        row.DefaultCellStyle.BackColor = System.Drawing.Color.LightYellow;
                    }
                }
            }

            // Підсвітити пропозиції, що закінчуються менше ніж за 3 дні
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                if (row.Cells["DaysUntilExpiry"].Value != DBNull.Value)
                {
                    int days = Convert.ToInt32(row.Cells["DaysUntilExpiry"].Value);
                    if (days <= 3)
                    {
                        row.DefaultCellStyle.BackColor = System.Drawing.Color.LightCoral;
                    }
                    else if (days <= 7)
                    {
                        row.DefaultCellStyle.BackColor = System.Drawing.Color.LightYellow;
                    }
                }
            }

            // Підсвітити кампанії з вичерпаним бюджетом
            foreach (DataGridViewRow row in dataGridView3.Rows)
            {
                if (row.Cells["BudgetUsedPercent"].Value != DBNull.Value)
                {
                    decimal percent = Convert.ToDecimal(row.Cells["BudgetUsedPercent"].Value);
                    if (percent >= 100)
                    {
                        row.DefaultCellStyle.BackColor = System.Drawing.Color.LightCoral;
                    }
                    else if (percent >= 80)
                    {
                        row.DefaultCellStyle.BackColor = System.Drawing.Color.LightYellow;
                    }
                }
            }
        }

        // Обробник кнопки "Оновити"
        private void button1_Click(object sender, EventArgs e)
        {
            LoadReminders();
            MessageBox.Show("Нагадування оновлено!",
                          "Інформація",
                          MessageBoxButtons.OK,
                          MessageBoxIcon.Information);
        }

        // Обробники для порожніх подій (можна видалити або залишити)
        private void tabPage1_Click(object sender, EventArgs e)
        {
            // Подія кліку на вкладку - не потрібна
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Подія кліку на комірку - можна використати для деталей
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                string companyName = row.Cells["CompanyName"].Value?.ToString() ?? "";
                string contactPerson = row.Cells["ContactPerson"].Value?.ToString() ?? "";
                string phone = row.Cells["Phone"].Value?.ToString() ?? "";

                MessageBox.Show($"Клієнт: {companyName}\n" +
                              $"Контактна особа: {contactPerson}\n" +
                              $"Телефон: {phone}",
                              "Деталі клієнта",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Information);
            }
        }

        // Додатковий обробник для подвійного кліку - відкриття деталей
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                int clientId = Convert.ToInt32(row.Cells["ClientID"].Value);

                MessageBox.Show($"Тут буде відкрита картка клієнта з ID: {clientId}",
                              "Інформація",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Information);

                // TODO: Відкрити форму деталей клієнта
                // ClientDetailsForm form = new ClientDetailsForm(clientId);
                // form.ShowDialog();
            }
        }
    }
}