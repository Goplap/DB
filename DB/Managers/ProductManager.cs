using System;
using System.Data;
using System.Data.SqlClient;
using MarketingCRMSystem.Database;

namespace MarketingCRMSystem.Managers
{
    public class ProductManager
    {
        private DatabaseHelper dbHelper;

        public ProductManager()
        {
            dbHelper = new DatabaseHelper();
        }

        public DataTable GetAllProducts()
        {
            string query = @"SELECT ProductID, ProductName, ProductType, 
                                   Specifications, Unit, BasePrice
                            FROM Products
                            ORDER BY ProductName";

            return dbHelper.ExecuteQuery(query);
        }

        public DataTable GetProductsByType(string productType)
        {
            string query = @"SELECT ProductID, ProductName, Specifications, Unit, BasePrice
                            FROM Products
                            WHERE ProductType = @ProductType
                            ORDER BY BasePrice DESC";

            SqlParameter[] parameters = {
                new SqlParameter("@ProductType", productType)
            };

            return dbHelper.ExecuteQuery(query, parameters);
        }

        public bool AddProduct(string productName, string productType,
                              string specifications, string unit, decimal basePrice)
        {
            string query = @"INSERT INTO Products 
                            (ProductName, ProductType, Specifications, Unit, BasePrice)
                            VALUES (@ProductName, @ProductType, @Specifications, @Unit, @BasePrice)";

            SqlParameter[] parameters = {
                new SqlParameter("@ProductName", productName),
                new SqlParameter("@ProductType", productType),
                new SqlParameter("@Specifications", specifications ?? (object)DBNull.Value),
                new SqlParameter("@Unit", unit),
                new SqlParameter("@BasePrice", basePrice)
            };

            return dbHelper.ExecuteNonQuery(query, parameters) > 0;
        }
    }
}