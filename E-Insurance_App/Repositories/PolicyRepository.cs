using System.Data;
using Microsoft.Data.SqlClient;
using E_Insurance_App.Helpers;
using E_Insurance_App.Models.Entities;

namespace E_Insurance_App.Repositories
{
    public class PolicyRepository : BaseRepository
    {
        public PolicyRepository(DbHelper db) : base(db) { }

        public int GetTotalPoliciesCount()
        {
            return (int)_db.ExecuteScalar(
                "SELECT COUNT(*) FROM PolicyTypes");
        }

        public List<PolicyType> GetPoliciesPaged(int page, int size)
        {
            int offset = (page - 1) * size;

            string query = @"
                SELECT *
                FROM PolicyTypes
                ORDER BY PolicyTypeId
                OFFSET @Offset ROWS
                FETCH NEXT @Size ROWS ONLY";

            SqlParameter[] p =
            {
                new("@Offset", offset),
                new("@Size", size)
            };

            DataTable dt = _db.ExecuteQuery(query, p);

            List<PolicyType> list = new();

            foreach (DataRow r in dt.Rows)
            {
                list.Add(new PolicyType
                {
                    PolicyTypeId = (int)r["PolicyTypeId"],
                    PolicyName = r["PolicyName"].ToString(),
                    PolicyCategory = r["PolicyCategory"].ToString(),
                    BasePremium = (decimal)r["BasePremium"],
                    InterestRate = (decimal)r["InterestRate"],
                    MinAge = (int)r["MinAge"],
                    MaxAge = (int)r["MaxAge"],
                    TermYears = (int)r["TermYears"],
                    IsActive = (bool)r["IsActive"]
                });
            }

            return list;
        }
    }
}
