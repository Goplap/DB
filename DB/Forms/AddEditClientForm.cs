using System;
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
            // TODO: Завантажити дані клієнта для редагування
            // Для простоти - пропускаємо
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCompanyName.Text))
            {
                MessageBox.Show("Введіть назву компанії!",
                              "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                bool success;
                if (clientId.HasValue)
                {
                    // Оновлення
                    success = clientManager.UpdateClient(
                        clientId.Value,
                        txtCompanyName.Text,
                        txtContactPerson.Text,
                        txtPhone.Text,
                        txtEmail.Text,
                        txtAddress.Text,
                        txtIndustry.Text,
                        cmbClientType.SelectedItem.ToString()
                    );
                }
                else
                {
                    // Додавання
                    success = clientManager.AddClient(
                        txtCompanyName.Text,
                        txtContactPerson.Text,
                        txtPhone.Text,
                        txtEmail.Text,
                        txtAddress.Text,
                        txtIndustry.Text,
                        cmbClientType.SelectedItem.ToString()
                    );
                }

                if (success)
                {
                    MessageBox.Show("Дані успішно збережено!",
                                  "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
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

        private void Скасувати_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}