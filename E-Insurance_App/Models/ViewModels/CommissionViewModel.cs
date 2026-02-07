using System.ComponentModel.DataAnnotations;
using E_Insurance_App.Models.Entities;

namespace E_Insurance_App.Models.ViewModels
{
    public class CommissionViewModel
    {
        public List<AgentListItem> Agents { get; set; } = new();
        public List<AgentCommission> Commissions { get; set; } = new();
        public AgentCommissionSummary? Summary { get; set; }
    }

    public class AgentListItem
    {
        public int AgentId { get; set; }
        public string AgentName { get; set; } = "";
        public string Email { get; set; } = "";
        public int TotalPoliciesSold { get; set; }
        public decimal TotalCommission { get; set; }
    }

    public class AgentCommissionSummary
    {
        public int AgentId { get; set; }
        public string AgentName { get; set; } = "";
        public string Email { get; set; } = "";
        public int TotalPoliciesSold { get; set; }
        public decimal TotalCommissionEarned { get; set; }
        public decimal PendingCommission { get; set; }
        public List<AgentCommission> CommissionDetails { get; set; } = new();
    }

    public class CalculateCommissionViewModel
    {
        [Required(ErrorMessage = "Please select an agent")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select an agent")]
        [Display(Name = "Agent")]
        public int AgentId { get; set; }

        [Required(ErrorMessage = "Please select a policy")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a policy")]
        [Display(Name = "Customer Policy")]
        public int CustomerPolicyId { get; set; }

        [Required(ErrorMessage = "Commission rate is required")]
        [Range(0.01, 100, ErrorMessage = "Commission rate must be between 0.01% and 100%")]
        [Display(Name = "Commission Rate (%)")]
        public decimal CommissionRate { get; set; } = 5; // Default 5%

        public decimal PolicyPremium { get; set; }
        public decimal CalculatedCommission { get; set; }

        public List<AgentListItem> Agents { get; set; } = new();
        public List<PolicyListItem> Policies { get; set; } = new();
    }

    public class PolicyListItem
    {
        public int CustomerPolicyId { get; set; }
        public string PolicyName { get; set; } = "";
        public string CustomerName { get; set; } = "";
        public decimal PremiumAmount { get; set; }
    }
}
