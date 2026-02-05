using System.ComponentModel.DataAnnotations;

namespace E_Insurance_App.Models.ViewModels
{
    public class EditPolicyViewModel
    {
        public int PolicyTypeId { get; set; }

        [Required]
        public string PolicyName { get; set; }

        [Required]
        public string PolicyCategory { get; set; }

        public decimal BasePremium { get; set; }

        public decimal InterestRate { get; set; }

        public int MinAge { get; set; }

        public int MaxAge { get; set; }

        public int TermYears { get; set; }

        public bool IsActive { get; set; }
    }
}
