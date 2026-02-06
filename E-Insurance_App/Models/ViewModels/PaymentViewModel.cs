using System.ComponentModel.DataAnnotations;

namespace E_Insurance_App.Models.ViewModels
{
    public class PaymentViewModel
    {
        public int CustomerPolicyId { get; set; }
        
        public string? PolicyName { get; set; }
        
        public decimal PremiumAmount { get; set; }

        [Required(ErrorMessage = "Please select a payment method")]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }

        public List<PaymentHistoryItem> PaymentHistory { get; set; } = new();
    }

    public class PaymentHistoryItem
    {
        public int PaymentId { get; set; }
        public string PolicyName { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionId { get; set; }
        public string Status { get; set; }
    }

    public class MakePaymentViewModel
    {
        public int CustomerPolicyId { get; set; }
        
        public string PolicyName { get; set; }
        
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Please select a payment method")]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }
    }
}
