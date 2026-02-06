namespace E_Insurance_App.Models.Entities
{
    public class Invoice
    {
        public int InvoiceId { get; set; }
        public int PaymentId { get; set; }
        public string? InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string? PdfPath { get; set; }
        
        // Calculated/display properties (not in DB)
        public decimal Amount { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalAmount { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public string? PolicyName { get; set; }
        public string? PaymentMode { get; set; }
        public string? TransactionRef { get; set; }
    }
}
