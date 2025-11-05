using System;
using System.Data;
using System.Data.SqlClient;
using MarketingCRMSystem.Database;

namespace MarketingCRMSystem.Managers
{
    public class ClientManager
    {
        private DatabaseHelper dbHelper;

        public ClientManager()
        {
            dbHelper = new DatabaseHelper();
        }

        public bool AddClient(string companyName, string contactPerson,
                             string phone, string email, string address,
                             string industry, string clientType)
        {
            string query = @"INSERT INTO Clients 
                            (CompanyName, ContactPerson, Phone, Email, Address, Industry, ClientType)
                            VALUES (@CompanyName, @ContactPerson, @Phone, @Email, @Address, @Industry, @ClientType)";

            SqlParameter[] parameters = {
                new SqlParameter("@CompanyName", companyName),
                new SqlParameter("@ContactPerson", contactPerson),
                new SqlParameter("@Phone", phone),
                new SqlParameter("@Email", email ?? (object)DBNull.Value),
                new SqlParameter("@Address", address ?? (object)DBNull.Value),
                new SqlParameter("@Industry", industry ?? (object)DBNull.Value),
                new SqlParameter("@ClientType", clientType)
            };

            return dbHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool UpdateClient(int clientId, string companyName, string contactPerson,
                                string phone, string email, string address,
                                string industry, string clientType)
        {
            string query = @"UPDATE Clients 
                            SET CompanyName = @CompanyName, 
                                ContactPerson = @ContactPerson, 
                                Phone = @Phone, 
                                Email = @Email, 
                                Address = @Address, 
                                Industry = @Industry, 
                                ClientType = @ClientType
                            WHERE ClientID = @ClientID";

            SqlParameter[] parameters = {
                new SqlParameter("@ClientID", clientId),
                new SqlParameter("@CompanyName", companyName),
                new SqlParameter("@ContactPerson", contactPerson),
                new SqlParameter("@Phone", phone),
                new SqlParameter("@Email", email ?? (object)DBNull.Value),
                new SqlParameter("@Address", address ?? (object)DBNull.Value),
                new SqlParameter("@Industry", industry ?? (object)DBNull.Value),
                new SqlParameter("@ClientType", clientType)
            };

            return dbHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool DeleteClient(int clientId)
        {
            string query = "DELETE FROM Clients WHERE ClientID = @ClientID";
            SqlParameter[] parameters = {
                new SqlParameter("@ClientID", clientId)
            };

            return dbHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        public DataTable GetAllClients()
        {
            string query = @"SELECT ClientID, CompanyName, ContactPerson, 
                                   Phone, Email, Address, Industry, ClientType
                            FROM Clients
                            ORDER BY CompanyName";

            return dbHelper.ExecuteQuery(query);
        }

        public DataTable SearchClients(string searchTerm, string clientType = null)
        {
            string query = @"SELECT ClientID, CompanyName, ContactPerson, 
                                   Phone, Email, ClientType, Industry
                            FROM Clients
                            WHERE (@SearchTerm IS NULL OR 
                                  CompanyName LIKE '%' + @SearchTerm + '%' OR
                                  ContactPerson LIKE '%' + @SearchTerm + '%')
                            AND (@ClientType IS NULL OR ClientType = @ClientType)
                            ORDER BY CompanyName";

            SqlParameter[] parameters = {
                new SqlParameter("@SearchTerm", searchTerm ?? (object)DBNull.Value),
                new SqlParameter("@ClientType", clientType ?? (object)DBNull.Value)
            };

            return dbHelper.ExecuteQuery(query, parameters);
        }

        public DataTable GetClientsByType(string clientType)
        {
            string query = @"SELECT ClientID, CompanyName, ContactPerson, Phone, Email, Industry
                            FROM Clients
                            WHERE ClientType = @ClientType
                            ORDER BY CompanyName";

            SqlParameter[] parameters = {
                new SqlParameter("@ClientType", clientType)
            };

            return dbHelper.ExecuteQuery(query, parameters);
        }
    }
}