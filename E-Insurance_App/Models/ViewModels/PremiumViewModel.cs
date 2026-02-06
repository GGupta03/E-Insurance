using System.ComponentModel.DataAnnotations;
using E_Insurance_App.Models.Entities;

namespace E_Insurance_App.Models.ViewModels
{
    public class PremiumViewModel
    {
        [Required(ErrorMessage = "Please select a policy")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a policy")]
        [Display(Name = "Policy")]
        public int PolicyTypeId { get; set; }

        [Required(ErrorMessage = "Age is required")]
        [Range(18, 100, ErrorMessage = "Age must be between 18 and 100")]
        [Display(Name = "Your Age")]
        public int Age { get; set; }

        public decimal Result { get; set; }

        public string? SelectedPolicyName { get; set; }

        public List<PolicyType> Policies { get; set; } = new();
    }
}
