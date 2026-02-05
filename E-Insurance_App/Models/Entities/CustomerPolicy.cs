namespace E_Insurance_App.Models.Entities
{
    public class CustomerPolicy
    {
        public int CustomerPolicyId { get; set; }

        public int UserId { get; set; }

        public int PolicyTypeId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal PremiumAmount { get; set; }

        public string PolicyStatus { get; set; }
    }
}
