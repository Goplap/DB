using System;
using System.Data;
using System.Data.SqlClient;
using DB.Database;

namespace DB.Managers
{
    public class ClientManager
    {
        private DatabaseHelper dbHelper;

        public ClientManager()
        {
            dbHelper = new DatabaseHelper();
        }

        // ✅ 1. ДОДАВАННЯ
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
                new SqlParameter("@Email", string.IsNullOrWhiteSpace(email) ? (object)DBNull.Value : email),
                new SqlParameter("@Address", string.IsNullOrWhiteSpace(address) ? (object)DBNull.Value : address),
                new SqlParameter("@Industry", string.IsNullOrWhiteSpace(industry) ? (object)DBNull.Value : industry),
                new SqlParameter("@ClientType", clientType)
            };

            return dbHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        // ✅ 2. ОНОВЛЕННЯ
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
                new SqlParameter("@Email", string.IsNullOrWhiteSpace(email) ? (object)DBNull.Value : email),
                new SqlParameter("@Address", string.IsNullOrWhiteSpace(address) ? (object)DBNull.Value : address),
                new SqlParameter("@Industry", string.IsNullOrWhiteSpace(industry) ? (object)DBNull.Value : industry),
                new SqlParameter("@ClientType", clientType)
            };

            return dbHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        // ✅ 3. ВИДАЛЕННЯ
        public bool DeleteClient(int clientId)
        {
            string query = "DELETE FROM Clients WHERE ClientID = @ClientID";
            SqlParameter[] parameters = {
                new SqlParameter("@ClientID", clientId)
            };

            return dbHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        // ✅ 4. ОТРИМАННЯ ОДНОГО КЛІЄНТА
        public DataRow GetClientById(int clientId)
        {
            string query = @"SELECT ClientID, CompanyName, ContactPerson, 
                                   Phone, Email, Address, Industry, ClientType
                            FROM Clients
                            WHERE ClientID = @ClientID";

            SqlParameter[] parameters = {
                new SqlParameter("@ClientID", clientId)
            };

            DataTable dt = dbHelper.ExecuteQuery(query, parameters);

            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0];
            }

            return null;
        }

        // ✅ 5. ОТРИМАННЯ ВСІХ
        public DataTable GetAllClients()
        {
            string query = @"SELECT ClientID, CompanyName, ContactPerson, 
                                   Phone, Email, Address, Industry, ClientType
                            FROM Clients
                            ORDER BY CompanyName";

            return dbHelper.ExecuteQuery(query);
        }

        // ✅ 6. ПОШУК
        public DataTable SearchClients(string searchTerm, string clientType = null)
        {
            string query = @"SELECT ClientID, CompanyName, ContactPerson, 
                                   Phone, Email, ClientType, Industry
                            FROM Clients
                            WHERE (@SearchTerm IS NULL OR 
                                  CompanyName LIKE '%' + @SearchTerm + '%' OR
                                  ContactPerson LIKE '%' + @SearchTerm + '%' OR
                                  Phone LIKE '%' + @SearchTerm + '%' OR
                                  Email LIKE '%' + @SearchTerm + '%')
                            AND (@ClientType IS NULL OR ClientType = @ClientType)
                            ORDER BY CompanyName";

            SqlParameter[] parameters = {
                new SqlParameter("@SearchTerm", string.IsNullOrWhiteSpace(searchTerm) ? (object)DBNull.Value : searchTerm),
                new SqlParameter("@ClientType", string.IsNullOrWhiteSpace(clientType) ? (object)DBNull.Value : clientType)
            };

            return dbHelper.ExecuteQuery(query, parameters);
        }
    }
}