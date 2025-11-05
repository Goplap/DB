using System;
using System.Data;
using System.Data.SqlClient;
using MarketingCRMSystem.Database;

namespace MarketingCRMSystem.Managers
{
    public class CampaignAnalytics
    {
        private DatabaseHelper dbHelper;

        public CampaignAnalytics()
        {
            dbHelper = new DatabaseHelper();
        }

        // Розрахунок ROI кампанії
        public decimal CalculateROI(int campaignId)
        {
            string query = @"
                SELECT cam.ActualSpent,
                       SUM(CASE WHEN pr.Status = 'Прийнято' 
                           THEN pr.TotalAmount ELSE 0 END) AS Revenue
                FROM Campaigns cam
                LEFT JOIN Contacts c ON cam.CampaignID = c.CampaignID
                LEFT JOIN Proposals pr ON c.ClientID = pr.ClientID
                    AND pr.CreateDate >= cam.StartDate
                    AND (cam.EndDate IS NULL OR pr.CreateDate <= cam.EndDate)
                WHERE cam.CampaignID = @CampaignID
                GROUP BY cam.ActualSpent";

            SqlParameter[] parameters = {
                new SqlParameter("@CampaignID", campaignId)
            };

            DataTable dt = dbHelper.ExecuteQuery(query, parameters);

            if (dt.Rows.Count > 0)
            {
                decimal spent = Convert.ToDecimal(dt.Rows[0]["ActualSpent"]);
                decimal revenue = dt.Rows[0]["Revenue"] != DBNull.Value
                    ? Convert.ToDecimal(dt.Rows[0]["Revenue"]) : 0;

                if (spent > 0)
                {
                    // ROI = (Revenue - Cost) / Cost * 100%
                    return ((revenue - spent) / spent) * 100;
                }
            }

            return 0;
        }

        // Ефективність маркетингових кампаній
        public DataTable GetCampaignEffectiveness()
        {
            string query = @"
                SELECT cam.CampaignName, cam.CampaignType,
                       cam.Budget, cam.ActualSpent,
                       COUNT(DISTINCT c.ContactID) AS ContactsGenerated,
                       COUNT(DISTINCT CASE WHEN pr.Status = 'Прийнято' THEN pr.ProposalID END) AS SuccessfulProposals,
                       CASE WHEN cam.ActualSpent > 0 
                            THEN CAST(COUNT(DISTINCT c.ContactID) AS FLOAT) / cam.ActualSpent 
                            ELSE 0 END AS CostPerContact
                FROM Campaigns cam
                LEFT JOIN Contacts c ON cam.CampaignID = c.CampaignID
                LEFT JOIN Proposals pr ON c.ClientID = pr.ClientID 
                    AND pr.CreateDate >= cam.StartDate
                    AND (cam.EndDate IS NULL OR pr.CreateDate <= cam.EndDate)
                GROUP BY cam.CampaignID, cam.CampaignName, cam.CampaignType, 
                         cam.Budget, cam.ActualSpent
                ORDER BY CostPerContact DESC";

            return dbHelper.ExecuteQuery(query);
        }
    }
}