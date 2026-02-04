namespace E_Insurance_App.Models.Entities
{
    public class Bank
    {
        public int BankId { get; set; }
        public string BankName { get; set; }
        public string IFSC { get; set; }
        public string Branch { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
