namespace E_Insurance_App.Models.Entities
{
    public class PolicyType
    {
        public int PolicyTypeId { get; set; }

        public string PolicyName { get; set; }

        public string PolicyCategory { get; set; }

        public decimal BasePremium { get; set; }

        public decimal InterestRate { get; set; }

        public int MinAge { get; set; }

        public int MaxAge { get; set; }

        public int TermYears { get; set; }

        public bool IsActive { get; set; }
    }
}
