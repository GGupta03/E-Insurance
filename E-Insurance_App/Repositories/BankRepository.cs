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
    }
}
