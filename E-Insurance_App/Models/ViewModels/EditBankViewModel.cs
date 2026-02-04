using System.ComponentModel.DataAnnotations;

namespace E_Insurance_App.Models.ViewModels
{
    public class EditBankViewModel
    {
        [Required]
        public int BankId { get; set; }

        [Required, StringLength(100)]
        public string BankName { get; set; }

        [Required, StringLength(20)]
        public string IFSC { get; set; }

        [Required, StringLength(100)]
        public string Branch { get; set; }

        public bool IsActive { get; set; }
    }
}
