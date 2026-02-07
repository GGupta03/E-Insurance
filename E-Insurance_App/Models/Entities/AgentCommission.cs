namespace E_Insurance_App.Models.Entities
{
    public class AgentCommission
    {
        public int CommissionId { get; set; }
        public int AgentId { get; set; }
        public int CustomerPolicyId { get; set; }
        public decimal? CommissionRate { get; set; }
        public decimal? CommissionAmount { get; set; }
        public DateTime CreatedAt { get; set; }

        // Display properties (not in DB)
        public string? AgentName { get; set; }
        public string? PolicyName { get; set; }
        public string? CustomerName { get; set; }
        public decimal? PolicyPremium { get; set; }
    }
}
