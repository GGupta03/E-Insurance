namespace E_Insurance_App.Models.Entities
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int CustomerPolicyId { get; set; }
        public decimal AmountPaid { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMode { get; set; } // Cash, Card, UPI, NetBanking
        public string? TransactionRef { get; set; }
        public string? PaymentStatus { get; set; } // Pending, Completed, Failed
        
        // Navigation properties (for display - not in DB)
        public string? PolicyName { get; set; }
        public string? CustomerName { get; set; }
    }
}
