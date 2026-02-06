using System.Data;
using Microsoft.Data.SqlClient;
using E_Insurance_App.Helpers;
using E_Insurance_App.Models.Entities;

namespace E_Insurance_App.Repositories
{
    public class PaymentRepository : BaseRepository
    {
        public PaymentRepository(DbHelper db) : base(db) { }

        public bool CreatePayment(Payment payment)
        {
            string query = @"
                INSERT INTO Payments 
                (CustomerPolicyId, AmountPaid, PaymentDate, PaymentMode, TransactionRef, PaymentStatus)
                VALUES
                (@CustomerPolicyId, @AmountPaid, @PaymentDate, @PaymentMode, @TransactionRef, @PaymentStatus);
                SELECT SCOPE_IDENTITY();";

            SqlParameter[] parameters =
            {
                new("@CustomerPolicyId", payment.CustomerPolicyId),
                new("@AmountPaid", payment.AmountPaid),
                new("@PaymentDate", payment.PaymentDate),
                new("@PaymentMode", payment.PaymentMode),
                new("@TransactionRef", (object?)payment.TransactionRef ?? DBNull.Value),
                new("@PaymentStatus", (object?)payment.PaymentStatus ?? DBNull.Value)
            };

            var result = _db.ExecuteScalar(query, parameters);
            if (result != null && result != DBNull.Value)
            {
                payment.PaymentId = Convert.ToInt32(result);
                return true;
            }
            return false;
        }

        public List<Payment> GetPaymentsByUserId(int userId)
        {
            string query = @"
                SELECT p.*, pt.PolicyName, u.FirstName + ' ' + u.LastName AS CustomerName
                FROM Payments p
                JOIN CustomerPolicies cp ON p.CustomerPolicyId = cp.CustomerPolicyId
                JOIN PolicyTypes pt ON cp.PolicyTypeId = pt.PolicyTypeId
                JOIN Users u ON cp.UserId = u.UserId
                WHERE cp.UserId = @UserId
                ORDER BY p.PaymentDate DESC";

            SqlParameter[] parameters = { new("@UserId", userId) };

            var dt = _db.ExecuteQuery(query, parameters);
            return MapPayments(dt);
        }

        public Payment? GetPaymentById(int paymentId)
        {
            string query = @"
                SELECT p.*, pt.PolicyName, u.FirstName + ' ' + u.LastName AS CustomerName
                FROM Payments p
                JOIN CustomerPolicies cp ON p.CustomerPolicyId = cp.CustomerPolicyId
                JOIN PolicyTypes pt ON cp.PolicyTypeId = pt.PolicyTypeId
                JOIN Users u ON cp.UserId = u.UserId
                WHERE p.PaymentId = @PaymentId";

            SqlParameter[] parameters = { new("@PaymentId", paymentId) };

            var dt = _db.ExecuteQuery(query, parameters);
            var payments = MapPayments(dt);
            return payments.Count > 0 ? payments[0] : null;
        }

        public List<Payment> GetAllPayments()
        {
            string query = @"
                SELECT p.*, pt.PolicyName, u.FirstName + ' ' + u.LastName AS CustomerName
                FROM Payments p
                JOIN CustomerPolicies cp ON p.CustomerPolicyId = cp.CustomerPolicyId
                JOIN PolicyTypes pt ON cp.PolicyTypeId = pt.PolicyTypeId
                JOIN Users u ON cp.UserId = u.UserId
                ORDER BY p.PaymentDate DESC";

            var dt = _db.ExecuteQuery(query);
            return MapPayments(dt);
        }

        public CustomerPolicy? GetCustomerPolicyById(int customerPolicyId)
        {
            string query = @"
                SELECT cp.*, pt.PolicyName
                FROM CustomerPolicies cp
                JOIN PolicyTypes pt ON cp.PolicyTypeId = pt.PolicyTypeId
                WHERE cp.CustomerPolicyId = @CustomerPolicyId";

            SqlParameter[] parameters = { new("@CustomerPolicyId", customerPolicyId) };

            var dt = _db.ExecuteQuery(query, parameters);
            
            if (dt.Rows.Count == 0) return null;

            var row = dt.Rows[0];
            return new CustomerPolicy
            {
                CustomerPolicyId = (int)row["CustomerPolicyId"],
                UserId = (int)row["UserId"],
                PolicyTypeId = (int)row["PolicyTypeId"],
                StartDate = (DateTime)row["StartDate"],
                EndDate = (DateTime)row["EndDate"],
                PremiumAmount = (decimal)row["PremiumAmount"],
                PolicyStatus = row["PolicyStatus"].ToString() ?? ""
            };
        }

        public List<CustomerPolicy> GetUnpaidPoliciesByUserId(int userId)
        {
            string query = @"
                SELECT cp.*, pt.PolicyName
                FROM CustomerPolicies cp
                JOIN PolicyTypes pt ON cp.PolicyTypeId = pt.PolicyTypeId
                WHERE cp.UserId = @UserId 
                AND cp.PolicyStatus = 'Active'
                AND cp.CustomerPolicyId NOT IN (
                    SELECT CustomerPolicyId FROM Payments WHERE PaymentStatus = 'Completed'
                )
                ORDER BY cp.StartDate DESC";

            SqlParameter[] parameters = { new("@UserId", userId) };

            var dt = _db.ExecuteQuery(query, parameters);
            var list = new List<CustomerPolicy>();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new CustomerPolicy
                {
                    CustomerPolicyId = (int)row["CustomerPolicyId"],
                    UserId = (int)row["UserId"],
                    PolicyTypeId = (int)row["PolicyTypeId"],
                    StartDate = (DateTime)row["StartDate"],
                    EndDate = (DateTime)row["EndDate"],
                    PremiumAmount = (decimal)row["PremiumAmount"],
                    PolicyStatus = row["PolicyStatus"].ToString() ?? ""
                });
            }

            return list;
        }

        private List<Payment> MapPayments(DataTable dt)
        {
            var list = new List<Payment>();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new Payment
                {
                    PaymentId = (int)row["PaymentId"],
                    CustomerPolicyId = (int)row["CustomerPolicyId"],
                    AmountPaid = (decimal)row["AmountPaid"],
                    PaymentDate = (DateTime)row["PaymentDate"],
                    PaymentMode = row["PaymentMode"]?.ToString() ?? "",
                    TransactionRef = row["TransactionRef"]?.ToString(),
                    PaymentStatus = row["PaymentStatus"]?.ToString(),
                    PolicyName = row["PolicyName"]?.ToString(),
                    CustomerName = row["CustomerName"]?.ToString()
                });
            }

            return list;
        }

        // Invoice methods
        public bool CreateInvoice(Invoice invoice)
        {
            string query = @"
                INSERT INTO Invoices 
                (PaymentId, InvoiceNumber, InvoiceDate, PdfPath)
                VALUES
                (@PaymentId, @InvoiceNumber, @InvoiceDate, @PdfPath)";

            SqlParameter[] parameters =
            {
                new("@PaymentId", invoice.PaymentId),
                new("@InvoiceNumber", (object?)invoice.InvoiceNumber ?? DBNull.Value),
                new("@InvoiceDate", (object?)invoice.InvoiceDate ?? DBNull.Value),
                new("@PdfPath", (object?)invoice.PdfPath ?? DBNull.Value)
            };

            return _db.ExecuteNonQuery(query, parameters) > 0;
        }

        public Invoice? GetInvoiceByPaymentId(int paymentId)
        {
            string query = @"
                SELECT i.*, p.PaymentMode, p.TransactionRef, p.AmountPaid,
                       pt.PolicyName, 
                       u.FirstName + ' ' + u.LastName AS CustomerName,
                       u.Email AS CustomerEmail
                FROM Invoices i
                JOIN Payments p ON i.PaymentId = p.PaymentId
                JOIN CustomerPolicies cp ON p.CustomerPolicyId = cp.CustomerPolicyId
                JOIN PolicyTypes pt ON cp.PolicyTypeId = pt.PolicyTypeId
                JOIN Users u ON cp.UserId = u.UserId
                WHERE i.PaymentId = @PaymentId";

            SqlParameter[] parameters = { new("@PaymentId", paymentId) };

            var dt = _db.ExecuteQuery(query, parameters);

            if (dt.Rows.Count == 0) return null;

            var row = dt.Rows[0];
            decimal amount = (decimal)row["AmountPaid"];
            decimal tax = Math.Round(amount * 0.18m, 2);
            
            return new Invoice
            {
                InvoiceId = (int)row["InvoiceId"],
                PaymentId = (int)row["PaymentId"],
                InvoiceNumber = row["InvoiceNumber"]?.ToString(),
                InvoiceDate = row["InvoiceDate"] != DBNull.Value ? (DateTime)row["InvoiceDate"] : null,
                PdfPath = row["PdfPath"]?.ToString(),
                Amount = amount,
                Tax = tax,
                TotalAmount = amount + tax,
                CustomerName = row["CustomerName"]?.ToString(),
                CustomerEmail = row["CustomerEmail"]?.ToString(),
                PolicyName = row["PolicyName"]?.ToString(),
                PaymentMode = row["PaymentMode"]?.ToString(),
                TransactionRef = row["TransactionRef"]?.ToString()
            };
        }
    }
}
