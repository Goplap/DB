using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using MarketingCRMSystem.Database;

namespace MarketingCRMSystem.Managers
{
    public class ReminderManager
    {
        private DatabaseHelper dbHelper;

        public ReminderManager()
        {
            dbHelper = new DatabaseHelper();
        }

        // 1. Клієнти без контактів понад 30 днів
        public DataTable GetInactiveClients(int days = 30)
        {
            string query = @"
                SELECT cl.ClientID, cl.CompanyName, cl.ContactPerson, 
                       cl.Phone, cl.ClientType,
                       MAX(c.ContactDate) AS LastContact,
                       DATEDIFF(DAY, MAX(c.ContactDate), GETDATE()) AS DaysSinceContact
                FROM Clients cl
                LEFT JOIN Contacts c ON cl.ClientID = c.ClientID
                WHERE cl.ClientType != 'Потенційний'
                GROUP BY cl.ClientID, cl.CompanyName, cl.ContactPerson, 
                         cl.Phone, cl.ClientType
                HAVING DATEDIFF(DAY, MAX(c.ContactDate), GETDATE()) > @Days
                    OR MAX(c.ContactDate) IS NULL
                ORDER BY DaysSinceContact DESC";

            SqlParameter[] parameters = {
                new SqlParameter("@Days", days)
            };

            return dbHelper.ExecuteQuery(query, parameters);
        }

        // 2. Пропозиції, термін дії яких закінчується протягом 7 днів
        public DataTable GetExpiringProposals(int days = 7)
        {
            string query = @"
                SELECT p.ProposalID, p.ProposalNumber, 
                       cl.CompanyName, p.CreateDate, p.ValidUntil, 
                       p.Status, p.TotalAmount,
                       DATEDIFF(DAY, GETDATE(), p.ValidUntil) AS DaysUntilExpiry
                FROM Proposals p
                JOIN Clients cl ON p.ClientID = cl.ClientID
                WHERE p.Status IN ('Відправлено', 'Чернетка')
                  AND p.ValidUntil IS NOT NULL
                  AND DATEDIFF(DAY, GETDATE(), p.ValidUntil) BETWEEN 0 AND @Days
                ORDER BY DaysUntilExpiry ASC";

            SqlParameter[] parameters = {
                new SqlParameter("@Days", days)
            };

            return dbHelper.ExecuteQuery(query, parameters);
        }

        // 3. Контроль бюджету кампаній (досягнення 80% та 100%)
        public DataTable GetCampaignBudgetAlerts()
        {
            string query = @"
                SELECT CampaignID, CampaignName, CampaignType, 
                       Budget, ActualSpent, StartDate, EndDate,
                       CAST((ActualSpent / NULLIF(Budget, 0) * 100) AS DECIMAL(5,2)) AS BudgetUsedPercent,
                       CASE 
                           WHEN ActualSpent >= Budget THEN 'Бюджет вичерпано (100%)'
                           WHEN ActualSpent >= Budget * 0.8 THEN 'Попередження (80%+)'
                           ELSE 'OK'
                       END AS AlertLevel
                FROM Campaigns
                WHERE ActualSpent >= Budget * 0.8
                  AND (EndDate IS NULL OR EndDate >= GETDATE())
                ORDER BY BudgetUsedPercent DESC";

            return dbHelper.ExecuteQuery(query);
        }

        // 4. Отримати всі нагадування разом
        public List<string> GetAllReminders()
        {
            List<string> reminders = new List<string>();

            // Неактивні клієнти
            DataTable inactiveClients = GetInactiveClients(30);
            if (inactiveClients.Rows.Count > 0)
            {
                reminders.Add($"⚠️ {inactiveClients.Rows.Count} клієнтів без контактів понад 30 днів");
            }

            // Пропозиції, що закінчуються
            DataTable expiringProposals = GetExpiringProposals(7);
            if (expiringProposals.Rows.Count > 0)
            {
                reminders.Add($"⏰ {expiringProposals.Rows.Count} пропозицій закінчуються протягом 7 днів");
            }

            // Попередження про бюджет
            DataTable budgetAlerts = GetCampaignBudgetAlerts();
            if (budgetAlerts.Rows.Count > 0)
            {
                reminders.Add($"💰 {budgetAlerts.Rows.Count} кампаній досягли 80%+ бюджету");
            }

            return reminders;
        }

        // 5. Отримати кількість нагадувань для відображення на головній формі
        public int GetTotalRemindersCount()
        {
            int count = 0;

            count += GetInactiveClients(30).Rows.Count;
            count += GetExpiringProposals(7).Rows.Count;
            count += GetCampaignBudgetAlerts().Rows.Count;

            return count;
        }
    }
}