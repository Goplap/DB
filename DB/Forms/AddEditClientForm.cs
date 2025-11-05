using System;
using System.Data;
using System.Windows.Forms;
using DB.Managers;

namespace DB.Forms
{
    public partial class AddEditClientForm : Form
    {
        private ClientManager clientManager;
        private int? clientId;

        public AddEditClientForm(int? clientId = null)
        {
            InitializeComponent();
            this.clientId = clientId;
            clientManager = new ClientManager();

            // Заповнити ComboBox
            cmbClientType.Items.AddRange(new string[] { "VIP", "Постійний", "Потенційний" });
            cmbClientType.SelectedIndex = 1;

            if (clientId.HasValue)
            {
                this.Text = "Редагувати клієнта";
                LoadClientData();
            }
            else
            {
                this.Text = "Додати клієнта";
            }
        }

        private void LoadClientData()
        {
            try
            {
                // Отримати дані клієнта через ClientManager
                DataRow client = clientManager.GetClientById(clientId.Value);

                if (client != null)
                {
                    // Заповнення полів даними з бази
                    txtCompanyName.Text = client["CompanyName"]?.ToString() ?? "";
                    txtContactPerson.Text = client["ContactPerson"]?.ToString() ?? "";
                    txtPhone.Text = client["Phone"]?.ToString() ?? "";
                    txtEmail.Text = client["Email"] != DBNull.Value ? client["Email"]?.ToString() : "";
                    txtAddress.Text = client["Address"] != DBNull.Value ? client["Address"]?.ToString() : "";
                    txtIndustry.Text = client["Industry"] != DBNull.Value ? client["Industry"]?.ToString() : "";

                    // Вибір типу клієнта
                    string clientType = client["ClientType"]?.ToString() ?? "";
                    if (!string.IsNullOrEmpty(clientType))
                    {
                        int index = cmbClientType.Items.IndexOf(clientType);
                        if (index >= 0)
                            cmbClientType.SelectedIndex = index;
                        else
                            cmbClientType.SelectedIndex = 1; // За замовчуванням "Постійний"
                    }
                }
                else
                {
                    MessageBox.Show("Клієнта не знайдено в базі даних!",
                                  "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження даних клієнта:\n{ex.Message}",
                              "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Перевірка обов'язкових полів
            if (string.IsNullOrWhiteSpace(txtCompanyName.Text))
            {
                MessageBox.Show("Введіть назву компанії!",
                              "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCompanyName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtContactPerson.Text))
            {
                MessageBox.Show("Введіть контактну особу!",
                              "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtContactPerson.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Введіть номер телефону!",
                              "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPhone.Focus();
                return;
            }

            if (cmbClientType.SelectedItem == null)
            {
                MessageBox.Show("Оберіть тип клієнта!",
                              "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbClientType.Focus();
                return;
            }

            try
            {
                bool success;
                if (clientId.HasValue)
                {
                    // Оновлення існуючого клієнта
                    success = clientManager.UpdateClient(
                        clientId.Value,
                        txtCompanyName.Text.Trim(),
                        txtContactPerson.Text.Trim(),
                        txtPhone.Text.Trim(),
                        txtEmail.Text.Trim(),
                        txtAddress.Text.Trim(),
                        txtIndustry.Text.Trim(),
                        cmbClientType.SelectedItem.ToString()
                    );

                    if (success)
                    {
                        MessageBox.Show("Дані клієнта успішно оновлено!",
                                      "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    // Додавання нового клієнта
                    success = clientManager.AddClient(
                        txtCompanyName.Text.Trim(),
                        txtContactPerson.Text.Trim(),
                        txtPhone.Text.Trim(),
                        txtEmail.Text.Trim(),
                        txtAddress.Text.Trim(),
                        txtIndustry.Text.Trim(),
                        cmbClientType.SelectedItem.ToString()
                    );

                    if (success)
                    {
                        MessageBox.Show("Клієнта успішно додано!",
                                      "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                if (success)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Не вдалося зберегти дані клієнта!",
                                  "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка збереження: {ex.Message}",
                              "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}