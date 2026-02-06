using System.ComponentModel.DataAnnotations;
using E_Insurance_App.Models.Entities;

namespace E_Insurance_App.Models.ViewModels
{
    public class PremiumViewModel
    {
        [Required(ErrorMessage = "Select a policy")]
        public int PolicyTypeId { get; set; }

        [Required]
        [Range(18, 100)]
        public int Age { get; set; }

        public decimal Result { get; set; }

        // 🔥 IMPORTANT FIX
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public List<PolicyType> Policies { get; set; }
    }
}
