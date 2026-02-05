using System.ComponentModel.DataAnnotations;

namespace E_Insurance_App.Models.ViewModels
{
    public class CreatePolicyViewModel
    {
        [Required]
        public string PolicyName { get; set; }

        [Required]
        public string PolicyCategory { get; set; }

        [Range(0, double.MaxValue)]
        public decimal BasePremium { get; set; }

        [Range(0, 100)]
        public decimal InterestRate { get; set; }

        [Range(0, 120)]
        public int MinAge { get; set; }

        [Range(0, 120)]
        public int MaxAge { get; set; }

        [Range(1, 100)]
        public int TermYears { get; set; }
    }
}
