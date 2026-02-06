using E_Insurance_App.Models.Entities;
using E_Insurance_App.Repositories;

namespace E_Insurance_App.Services.Implementations
{
    public class PaymentService
    {
        private readonly PaymentRepository _paymentRepo;

        public PaymentService(PaymentRepository paymentRepo)
        {
            _paymentRepo = paymentRepo;
        }

        public (bool success, string message, int paymentId) ProcessPayment(
            int customerPolicyId, 
            decimal amount, 
            string paymentMode)
        {
            // Generate transaction reference
            string transactionRef = GenerateTransactionRef();

            var payment = new Payment
            {
                CustomerPolicyId = customerPolicyId,
                AmountPaid = amount,
                PaymentDate = DateTime.Now,
                PaymentMode = paymentMode,
                TransactionRef = transactionRef,
                PaymentStatus = "Completed" // Simulating successful payment
            };

            bool paymentCreated = _paymentRepo.CreatePayment(payment);

            if (!paymentCreated)
            {
                return (false, "Failed to process payment", 0);
            }

            // Generate invoice
            var invoice = GenerateInvoice(payment);
            bool invoiceCreated = _paymentRepo.CreateInvoice(invoice);

            if (!invoiceCreated)
            {
                return (true, "Payment processed but invoice generation failed", payment.PaymentId);
            }

            return (true, "Payment processed successfully", payment.PaymentId);
        }

        public Invoice GenerateInvoice(Payment payment)
        {
            return new Invoice
            {
                PaymentId = payment.PaymentId,
                InvoiceNumber = GenerateInvoiceNumber(),
                InvoiceDate = DateTime.Now,
                PdfPath = null // PDF generation can be added later
            };
        }

        private string GenerateTransactionRef()
        {
            return $"TXN{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";
        }

        private string GenerateInvoiceNumber()
        {
            return $"INV{DateTime.Now:yyyyMMdd}{new Random().Next(10000, 99999)}";
        }

        public List<Payment> GetUserPayments(int userId)
        {
            return _paymentRepo.GetPaymentsByUserId(userId);
        }

        public Invoice? GetInvoice(int paymentId)
        {
            return _paymentRepo.GetInvoiceByPaymentId(paymentId);
        }
    }
}
