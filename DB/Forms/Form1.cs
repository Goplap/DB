using System;
using System.Windows.Forms;
using DB.Database;

namespace DB.Forms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CheckDatabaseConnection();
        }

        private void CheckDatabaseConnection()
        {
            DatabaseHelper dbHelper = new DatabaseHelper();
            if (dbHelper.TestConnection())
            {
                MessageBox.Show("Підключення до бази даних успішне!",
                              "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Не вдалося підключитися до бази даних!",
                              "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Відкрити форму клієнтів
            ClientForm clientForm = new ClientForm();
            clientForm.ShowDialog();
        }
    }
}