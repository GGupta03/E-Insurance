using System.Data;
using Microsoft.Data.SqlClient;
using E_Insurance_App.Helpers;
using E_Insurance_App.Models.Entities;

namespace E_Insurance_App.Repositories
{
    public class BankRepository : BaseRepository
    {
        public BankRepository(DbHelper db) : base(db) { }

        public int GetTotalBanksCount()
        {
            string query = "SELECT COUNT(*) FROM Banks";
            return (int)_db.ExecuteScalar(query);
        }

        public List<Bank> GetBanksPaged(int page, int pageSize)
        {
            int offset = (page - 1) * pageSize;

            string query = @"
                SELECT BankId, BankName, IFSC, Branch, IsActive
                FROM Banks
                ORDER BY BankId
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY";

            SqlParameter[] parameters =
            {
                new SqlParameter("@Offset", offset),
                new SqlParameter("@PageSize", pageSize)
            };

            DataTable dt = _db.ExecuteQuery(query, parameters);

            List<Bank> banks = new();

            foreach (DataRow row in dt.Rows)
            {
                banks.Add(new Bank
                {
                    BankId = (int)row["BankId"],
                    BankName = row["BankName"].ToString(),
                    IFSC = row["IFSC"].ToString(),
                    Branch = row["Branch"].ToString(),
                    IsActive = (bool)row["IsActive"]
                });
            }

            return banks;
        }

        public bool CreateBank(Bank bank)
        {
            string query = @"
                INSERT INTO Banks (BankName, IFSC, Branch, IsActive)
                VALUES (@BankName, @IFSC, @Branch, 1)";

            SqlParameter[] parameters =
            {
                new SqlParameter("@BankName", bank.BankName),
                new SqlParameter("@IFSC", bank.IFSC),
                new SqlParameter("@Branch", bank.Branch)
            };

            return _db.ExecuteNonQuery(query, parameters) > 0;
        }
        public Bank GetBankById(int bankId)
        {
            string query = "SELECT * FROM Banks WHERE BankId = @BankId";

            SqlParameter[] parameters =
            {
                new SqlParameter("@BankId", bankId)
            };

            DataTable dt = _db.ExecuteQuery(query, parameters);

            if (dt.Rows.Count == 0)
                return null;

            var row = dt.Rows[0];

            return new Bank
            {
                BankId = (int)row["BankId"],
                BankName = row["BankName"].ToString(),
                IFSC = row["IFSC"].ToString(),
                Branch = row["Branch"].ToString(),
                IsActive = (bool)row["IsActive"]
            };
        }
        public bool UpdateBank(Bank bank)
        {
            string query = @"
                UPDATE Banks
                SET BankName = @BankName,
                    IFSC = @IFSC,
                    Branch = @Branch,
                    IsActive = @IsActive
                WHERE BankId = @BankId";

            SqlParameter[] parameters =
            {
                new SqlParameter("@BankName", bank.BankName),
                new SqlParameter("@IFSC", bank.IFSC),
                new SqlParameter("@Branch", bank.Branch),
                new SqlParameter("@IsActive", bank.IsActive),
                new SqlParameter("@BankId", bank.BankId)
            };

            return _db.ExecuteNonQuery(query, parameters) > 0;
        }
        public bool UpdateBankStatus(int bankId, bool isActive)
        {
            string query = @"
                UPDATE Banks
                SET IsActive = @IsActive
                WHERE BankId = @BankId";

            SqlParameter[] parameters =
            {
                new SqlParameter("@IsActive", isActive),
                new SqlParameter("@BankId", bankId)
            };

            return _db.ExecuteNonQuery(query, parameters) > 0;
        }

    }
}
