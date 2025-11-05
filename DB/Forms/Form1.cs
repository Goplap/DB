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
            // TODO: This line of code loads data into the 'marketingDeptDataSet.ProposalItems' table. You can move, or remove it, as needed.
            this.proposalItemsTableAdapter.Fill(this.marketingDeptDataSet.ProposalItems);
            // TODO: This line of code loads data into the 'marketingDeptDataSet.Products' table. You can move, or remove it, as needed.
            this.productsTableAdapter.Fill(this.marketingDeptDataSet.Products);
            // TODO: This line of code loads data into the 'marketingDeptDataSet.Employees' table. You can move, or remove it, as needed.
            this.employeesTableAdapter.Fill(this.marketingDeptDataSet.Employees);
            // TODO: This line of code loads data into the 'marketingDeptDataSet.Contacts' table. You can move, or remove it, as needed.
            this.contactsTableAdapter.Fill(this.marketingDeptDataSet.Contacts);
            // TODO: This line of code loads data into the 'marketingDeptDataSet.Clients' table. You can move, or remove it, as needed.
            this.clientsTableAdapter.Fill(this.marketingDeptDataSet.Clients);
            // TODO: This line of code loads data into the 'marketingDeptDataSet.Campaigns' table. You can move, or remove it, as needed.
            this.campaignsTableAdapter.Fill(this.marketingDeptDataSet.Campaigns);
            // Відкрити форму клієнтів
            ClientForm clientForm = new ClientForm();
            clientForm.ShowDialog();
        }
    }
}