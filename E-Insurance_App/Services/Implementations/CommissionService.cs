using E_Insurance_App.Models.Entities;
using E_Insurance_App.Models.ViewModels;
using E_Insurance_App.Repositories;

namespace E_Insurance_App.Services.Implementations
{
    public class CommissionService
    {
        private readonly CommissionRepository _commissionRepo;

        public CommissionService(CommissionRepository commissionRepo)
        {
            _commissionRepo = commissionRepo;
        }

        // Calculate commission amount based on premium and rate
        public decimal CalculateCommission(decimal premium, decimal ratePercent)
        {
            return Math.Round(premium * (ratePercent / 100), 2);
        }

        // Process and save commission
        public (bool success, string message) ProcessCommission(
            int agentId, 
            int customerPolicyId, 
            decimal commissionRate)
        {
            // Check if commission already exists
            if (_commissionRepo.CommissionExists(agentId, customerPolicyId))
            {
                return (false, "Commission already assigned for this policy to this agent");
            }

            // Get policy premium
            decimal premium = _commissionRepo.GetPolicyPremium(customerPolicyId);
            if (premium <= 0)
            {
                return (false, "Invalid policy or premium amount");
            }

            // Calculate commission
            decimal commissionAmount = CalculateCommission(premium, commissionRate);

            var commission = new AgentCommission
            {
                AgentId = agentId,
                CustomerPolicyId = customerPolicyId,
                CommissionRate = commissionRate,
                CommissionAmount = commissionAmount,
                CreatedAt = DateTime.Now
            };

            bool created = _commissionRepo.CreateCommission(commission);

            if (created)
            {
                return (true, $"Commission of ?{commissionAmount:N2} assigned successfully");
            }

            return (false, "Failed to create commission record");
        }

        // Get all agents with their commission summary
        public List<AgentListItem> GetAllAgentsWithCommission()
        {
            return _commissionRepo.GetAllAgents();
        }

        // Get agent commission details
        public AgentCommissionSummary? GetAgentCommissionSummary(int agentId)
        {
            return _commissionRepo.GetAgentCommissionSummary(agentId);
        }

        // Get all commissions
        public List<AgentCommission> GetAllCommissions()
        {
            return _commissionRepo.GetAllCommissions();
        }

        // Get available policies for commission assignment
        public List<PolicyListItem> GetAvailablePolicies()
        {
            return _commissionRepo.GetPoliciesForCommission();
        }

        // Get policy premium
        public decimal GetPolicyPremium(int customerPolicyId)
        {
            return _commissionRepo.GetPolicyPremium(customerPolicyId);
        }
    }
}
