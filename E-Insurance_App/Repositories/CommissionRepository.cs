using System.Data;
using Microsoft.Data.SqlClient;
using E_Insurance_App.Helpers;
using E_Insurance_App.Models.Entities;
using E_Insurance_App.Models.ViewModels;

namespace E_Insurance_App.Repositories
{
    public class CommissionRepository : BaseRepository
    {
        public CommissionRepository(DbHelper db) : base(db) { }

        // Get all agents (users with Role = 'Agent')
        public List<AgentListItem> GetAllAgents()
        {
            string query = @"
                SELECT u.UserId AS AgentId, 
                       u.FirstName + ' ' + u.LastName AS AgentName,
                       u.Email,
                       ISNULL(COUNT(c.CommissionId), 0) AS TotalPoliciesSold,
                       ISNULL(SUM(c.CommissionAmount), 0) AS TotalCommission
                FROM Users u
                LEFT JOIN AgentCommissions c ON u.UserId = c.AgentId
                WHERE u.Role = 'Agent' AND u.IsActive = 1
                GROUP BY u.UserId, u.FirstName, u.LastName, u.Email
                ORDER BY u.FirstName, u.LastName";

            var dt = _db.ExecuteQuery(query);
            var list = new List<AgentListItem>();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new AgentListItem
                {
                    AgentId = (int)row["AgentId"],
                    AgentName = row["AgentName"]?.ToString() ?? "",
                    Email = row["Email"]?.ToString() ?? "",
                    TotalPoliciesSold = (int)row["TotalPoliciesSold"],
                    TotalCommission = row["TotalCommission"] != DBNull.Value 
                        ? (decimal)row["TotalCommission"] : 0
                });
            }

            return list;
        }

        // Get agent by ID
        public AgentListItem? GetAgentById(int agentId)
        {
            string query = @"
                SELECT u.UserId AS AgentId, 
                       u.FirstName + ' ' + u.LastName AS AgentName,
                       u.Email,
                       ISNULL(COUNT(c.CommissionId), 0) AS TotalPoliciesSold,
                       ISNULL(SUM(c.CommissionAmount), 0) AS TotalCommission
                FROM Users u
                LEFT JOIN AgentCommissions c ON u.UserId = c.AgentId
                WHERE u.UserId = @AgentId AND u.Role = 'Agent'
                GROUP BY u.UserId, u.FirstName, u.LastName, u.Email";

            SqlParameter[] parameters = { new("@AgentId", agentId) };
            var dt = _db.ExecuteQuery(query, parameters);

            if (dt.Rows.Count == 0) return null;

            var row = dt.Rows[0];
            return new AgentListItem
            {
                AgentId = (int)row["AgentId"],
                AgentName = row["AgentName"]?.ToString() ?? "",
                Email = row["Email"]?.ToString() ?? "",
                TotalPoliciesSold = (int)row["TotalPoliciesSold"],
                TotalCommission = row["TotalCommission"] != DBNull.Value 
                    ? (decimal)row["TotalCommission"] : 0
            };
        }

        // Get all commissions for an agent
        public List<AgentCommission> GetCommissionsByAgentId(int agentId)
        {
            string query = @"
                SELECT c.*, 
                       u.FirstName + ' ' + u.LastName AS AgentName,
                       pt.PolicyName,
                       cu.FirstName + ' ' + cu.LastName AS CustomerName,
                       cp.PremiumAmount AS PolicyPremium
                FROM AgentCommissions c
                JOIN Users u ON c.AgentId = u.UserId
                JOIN CustomerPolicies cp ON c.CustomerPolicyId = cp.CustomerPolicyId
                JOIN PolicyTypes pt ON cp.PolicyTypeId = pt.PolicyTypeId
                JOIN Users cu ON cp.UserId = cu.UserId
                WHERE c.AgentId = @AgentId
                ORDER BY c.CreatedAt DESC";

            SqlParameter[] parameters = { new("@AgentId", agentId) };
            var dt = _db.ExecuteQuery(query, parameters);

            return MapCommissions(dt);
        }

        // Get all commissions (for admin view)
        public List<AgentCommission> GetAllCommissions()
        {
            string query = @"
                SELECT c.*, 
                       u.FirstName + ' ' + u.LastName AS AgentName,
                       pt.PolicyName,
                       cu.FirstName + ' ' + cu.LastName AS CustomerName,
                       cp.PremiumAmount AS PolicyPremium
                FROM AgentCommissions c
                JOIN Users u ON c.AgentId = u.UserId
                JOIN CustomerPolicies cp ON c.CustomerPolicyId = cp.CustomerPolicyId
                JOIN PolicyTypes pt ON cp.PolicyTypeId = pt.PolicyTypeId
                JOIN Users cu ON cp.UserId = cu.UserId
                ORDER BY c.CreatedAt DESC";

            var dt = _db.ExecuteQuery(query);
            return MapCommissions(dt);
        }

        // Get policies that can have commission assigned (not yet assigned to this agent)
        public List<PolicyListItem> GetPoliciesForCommission()
        {
            string query = @"
                SELECT cp.CustomerPolicyId,
                       pt.PolicyName,
                       u.FirstName + ' ' + u.LastName AS CustomerName,
                       cp.PremiumAmount
                FROM CustomerPolicies cp
                JOIN PolicyTypes pt ON cp.PolicyTypeId = pt.PolicyTypeId
                JOIN Users u ON cp.UserId = u.UserId
                WHERE cp.PolicyStatus = 'Active'
                ORDER BY cp.CustomerPolicyId DESC";

            var dt = _db.ExecuteQuery(query);
            var list = new List<PolicyListItem>();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new PolicyListItem
                {
                    CustomerPolicyId = (int)row["CustomerPolicyId"],
                    PolicyName = row["PolicyName"]?.ToString() ?? "",
                    CustomerName = row["CustomerName"]?.ToString() ?? "",
                    PremiumAmount = (decimal)row["PremiumAmount"]
                });
            }

            return list;
        }

        // Get policy premium by CustomerPolicyId
        public decimal GetPolicyPremium(int customerPolicyId)
        {
            string query = "SELECT PremiumAmount FROM CustomerPolicies WHERE CustomerPolicyId = @CustomerPolicyId";
            SqlParameter[] parameters = { new("@CustomerPolicyId", customerPolicyId) };
            
            var result = _db.ExecuteScalar(query, parameters);
            return result != null && result != DBNull.Value ? (decimal)result : 0;
        }

        // Create commission record
        public bool CreateCommission(AgentCommission commission)
        {
            string query = @"
                INSERT INTO AgentCommissions 
                (AgentId, CustomerPolicyId, CommissionRate, CommissionAmount, CreatedAt)
                VALUES
                (@AgentId, @CustomerPolicyId, @CommissionRate, @CommissionAmount, @CreatedAt)";

            SqlParameter[] parameters =
            {
                new("@AgentId", commission.AgentId),
                new("@CustomerPolicyId", commission.CustomerPolicyId),
                new("@CommissionRate", (object?)commission.CommissionRate ?? DBNull.Value),
                new("@CommissionAmount", (object?)commission.CommissionAmount ?? DBNull.Value),
                new("@CreatedAt", commission.CreatedAt)
            };

            return _db.ExecuteNonQuery(query, parameters) > 0;
        }

        // Check if commission already exists for this agent and policy
        public bool CommissionExists(int agentId, int customerPolicyId)
        {
            string query = @"
                SELECT COUNT(*) FROM AgentCommissions 
                WHERE AgentId = @AgentId AND CustomerPolicyId = @CustomerPolicyId";

            SqlParameter[] parameters =
            {
                new("@AgentId", agentId),
                new("@CustomerPolicyId", customerPolicyId)
            };

            var result = _db.ExecuteScalar(query, parameters);
            return result != null && (int)result > 0;
        }

        // Get commission summary for an agent
        public AgentCommissionSummary? GetAgentCommissionSummary(int agentId)
        {
            var agent = GetAgentById(agentId);
            if (agent == null) return null;

            var commissions = GetCommissionsByAgentId(agentId);

            return new AgentCommissionSummary
            {
                AgentId = agent.AgentId,
                AgentName = agent.AgentName,
                Email = agent.Email,
                TotalPoliciesSold = commissions.Count,
                TotalCommissionEarned = commissions.Sum(c => c.CommissionAmount ?? 0),
                CommissionDetails = commissions
            };
        }

        private List<AgentCommission> MapCommissions(DataTable dt)
        {
            var list = new List<AgentCommission>();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new AgentCommission
                {
                    CommissionId = (int)row["CommissionId"],
                    AgentId = (int)row["AgentId"],
                    CustomerPolicyId = (int)row["CustomerPolicyId"],
                    CommissionRate = row["CommissionRate"] != DBNull.Value 
                        ? (decimal)row["CommissionRate"] : null,
                    CommissionAmount = row["CommissionAmount"] != DBNull.Value 
                        ? (decimal)row["CommissionAmount"] : null,
                    CreatedAt = row["CreatedAt"] != DBNull.Value 
                        ? (DateTime)row["CreatedAt"] : DateTime.Now,
                    AgentName = row["AgentName"]?.ToString(),
                    PolicyName = row["PolicyName"]?.ToString(),
                    CustomerName = row["CustomerName"]?.ToString(),
                    PolicyPremium = row["PolicyPremium"] != DBNull.Value 
                        ? (decimal)row["PolicyPremium"] : null
                });
            }

            return list;
        }
    }
}
