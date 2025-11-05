using System;
using System.Windows.Forms;
using DB.Managers;

namespace DB.Forms
{
    public partial class ClientForm : Form
    {
        private ClientManager clientManager;
        private AnalyticsManager analyticsManager;

        public ClientForm()
        {
            InitializeComponent();
            clientManager = new ClientManager();
            analyticsManager = new AnalyticsManager();

            // Підключення обробників подій
            this.Load += ClientForm_Load;
            txtSearch.TextChanged += txtSearch_TextChanged;
            btnAdd.Click += btnAdd_Click;
            btnEdit.Click += btnEdit_Click;
            btnDelete.Click += btnDelete_Click;
            btnRefresh.Click += btnRefresh_Click;
            btnStatistics.Click += btnStatistics_Click;

            SetupDataGridView();
        }

        private void SetupDataGridView()
        {
            dataGridViewClients.ReadOnly = true;
            dataGridViewClients.AllowUserToAddRows = false;
            dataGridViewClients.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewClients.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        // ✅ ОБРОБНИК ЗАВАНТАЖЕННЯ ФОРМИ
        private void ClientForm_Load(object sender, EventArgs e)
        {
            LoadClients();
        }

        // ✅ ОБРОБНИК КЛІКУ НА LABEL (просто порожній, щоб не було помилки)
        private void label1_Click(object sender, EventArgs e)
        {
            // Нічого не робимо
        }

        // ✅ ОБРОБНИК МАЛЮВАННЯ ПАНЕЛІ (просто порожній, щоб не було помилки)
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            // Нічого не робимо
        }

        // ✅ ЗАВАНТАЖЕННЯ КЛІЄНТІВ
        private void LoadClients()
        {
            try
            {
                var clients = clientManager.GetAllClients();
                dataGridViewClients.DataSource = clients;
                this.Text = $"Клієнти - Всього: {clients.Rows.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження: {ex.Message}",
                              "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ✅ ПОШУК
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    LoadClients();
                }
                else
                {
                    var results = clientManager.SearchClients(txtSearch.Text);
                    dataGridViewClients.DataSource = results;
                    this.Text = $"Клієнти - Знайдено: {results.Rows.Count}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка пошуку: {ex.Message}",
                              "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ✅ ДОДАВАННЯ
        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddEditClientForm form = new AddEditClientForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadClients();
            }
        }

        // ✅ РЕДАГУВАННЯ
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridViewClients.SelectedRows.Count > 0)
            {
                int clientId = Convert.ToInt32(dataGridViewClients.SelectedRows[0].Cells["ClientID"].Value);
                AddEditClientForm form = new AddEditClientForm(clientId);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadClients();
                }
            }
            else
            {
                MessageBox.Show("Виберіть клієнта для редагування",
                              "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ✅ ВИДАЛЕННЯ
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewClients.SelectedRows.Count > 0)
            {
                string companyName = dataGridViewClients.SelectedRows[0].Cells["CompanyName"].Value.ToString();
                var result = MessageBox.Show($"Видалити клієнта '{companyName}'?",
                                           "Підтвердження",
                                           MessageBoxButtons.YesNo,
                                           MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    int clientId = Convert.ToInt32(dataGridViewClients.SelectedRows[0].Cells["ClientID"].Value);
                    if (clientManager.DeleteClient(clientId))
                    {
                        MessageBox.Show("Клієнта успішно видалено!",
                                      "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadClients();
                    }
                }
            }
            else
            {
                MessageBox.Show("Виберіть клієнта для видалення",
                              "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ✅ ОНОВЛЕННЯ
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            LoadClients();
        }

        // ✅ СТАТИСТИКА (математична обробка)
        private void btnStatistics_Click(object sender, EventArgs e)
        {
            try
            {
                string stats = analyticsManager.GetFullStatistics();
                MessageBox.Show(stats, "Статистика",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка отримання статистики: {ex.Message}",
                              "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}