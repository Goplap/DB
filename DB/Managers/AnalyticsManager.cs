using System;
using System.Data;
using DB.Database;

namespace DB.Managers
{
    public class AnalyticsManager
    {
        private DatabaseHelper dbHelper;

        public AnalyticsManager()
        {
            dbHelper = new DatabaseHelper();
        }

        // ✅ 1. ПІДРАХУНОК КІЛЬКОСТІ клієнтів за типом
        public DataTable GetClientCountByType()
        {
            string query = @"
                SELECT ClientType, 
                       COUNT(*) AS TotalClients
                FROM Clients
                GROUP BY ClientType
                ORDER BY TotalClients DESC";

            return dbHelper.ExecuteQuery(query);
        }

        // ✅ 2. СЕРЕДНЯ ВАРТІСТЬ пропозицій
        public decimal GetAverageProposalAmount()
        {
            string query = "SELECT AVG(TotalAmount) FROM Proposals WHERE Status != 'Чернетка'";
            object result = dbHelper.ExecuteScalar(query);
            return result != null && result != DBNull.Value ? Convert.ToDecimal(result) : 0;
        }

        // ✅ 3. ЗАГАЛЬНА СУМА прийнятих пропозицій
        public decimal GetTotalAcceptedProposals()
        {
            string query = "SELECT SUM(TotalAmount) FROM Proposals WHERE Status = 'Прийнято'";
            object result = dbHelper.ExecuteScalar(query);
            return result != null && result != DBNull.Value ? Convert.ToDecimal(result) : 0;
        }

        // ✅ 4. МАКСИМАЛЬНА пропозиція
        public decimal GetMaxProposalAmount()
        {
            string query = "SELECT MAX(TotalAmount) FROM Proposals";
            object result = dbHelper.ExecuteScalar(query);
            return result != null && result != DBNull.Value ? Convert.ToDecimal(result) : 0;
        }

        // ✅ 5. МІНІМАЛЬНА пропозиція
        public decimal GetMinProposalAmount()
        {
            string query = "SELECT MIN(TotalAmount) FROM Proposals WHERE TotalAmount > 0";
            object result = dbHelper.ExecuteScalar(query);
            return result != null && result != DBNull.Value ? Convert.ToDecimal(result) : 0;
        }

        // ✅ 6. ВІДСОТОК виконання бюджету кампаній
        public DataTable GetCampaignBudgetPercentage()
        {
            string query = @"
                SELECT CampaignName,
                       Budget,
                       ActualSpent,
                       CAST((ActualSpent / NULLIF(Budget, 0) * 100) AS DECIMAL(5,2)) AS BudgetUsedPercent
                FROM Campaigns
                WHERE Budget > 0
                ORDER BY BudgetUsedPercent DESC";

            return dbHelper.ExecuteQuery(query);
        }

        // ✅ 7. СТАТИСТИКА по галузях (кількість клієнтів)
        public DataTable GetClientsByIndustry()
        {
            string query = @"
                SELECT Industry, 
                       COUNT(*) AS ClientCount
                FROM Clients
                WHERE Industry IS NOT NULL
                GROUP BY Industry
                ORDER BY ClientCount DESC";

            return dbHelper.ExecuteQuery(query);
        }

        // ✅ 8. ТОП-10 найбільших пропозицій
        public DataTable GetTop10Proposals()
        {
            string query = @"
                SELECT TOP 10 
                       p.ProposalNumber,
                       c.CompanyName,
                       p.TotalAmount,
                       p.Status,
                       p.CreateDate
                FROM Proposals p
                JOIN Clients c ON p.ClientID = c.ClientID
                ORDER BY p.TotalAmount DESC";

            return dbHelper.ExecuteQuery(query);
        }

        // ✅ 9. Отримати всю статистику разом
        public string GetFullStatistics()
        {
            decimal avgAmount = GetAverageProposalAmount();
            decimal totalAccepted = GetTotalAcceptedProposals();
            decimal maxAmount = GetMaxProposalAmount();
            decimal minAmount = GetMinProposalAmount();

            return $"📊 СТАТИСТИКА:\n\n" +
                   $"💰 Середня вартість пропозицій: {avgAmount:N2} грн\n" +
                   $"✅ Загальна сума прийнятих: {totalAccepted:N2} грн\n" +
                   $"📈 Максимальна пропозиція: {maxAmount:N2} грн\n" +
                   $"📉 Мінімальна пропозиція: {minAmount:N2} грн";
        }
    }
}