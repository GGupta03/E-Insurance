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
        public PolicyType GetPolicyById(int id)
        {
            string query = "SELECT * FROM PolicyTypes WHERE PolicyTypeId=@Id";

            SqlParameter[] p = { new("@Id", id) };

            var dt = _db.ExecuteQuery(query, p);

            if (dt.Rows.Count == 0) return null;

            var r = dt.Rows[0];

            return new PolicyType
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
            };
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

        public bool CreatePolicy(PolicyType policy)
        {
            string query = @"
                INSERT INTO PolicyTypes
                (PolicyName, PolicyCategory, BasePremium,
                 InterestRate, MinAge, MaxAge, TermYears, IsActive)
                VALUES
                (@Name, @Category, @Premium,
                 @Rate, @MinAge, @MaxAge, @Term, 1)";

            SqlParameter[] p =
            {
                new("@Name", policy.PolicyName),
                new("@Category", policy.PolicyCategory),
                new("@Premium", policy.BasePremium),
                new("@Rate", policy.InterestRate),
                new("@MinAge", policy.MinAge),
                new("@MaxAge", policy.MaxAge),
                new("@Term", policy.TermYears)
            };

            return _db.ExecuteNonQuery(query, p) > 0;
        }

        public bool UpdatePolicy(PolicyType p)
        {
            string query = @"
                UPDATE PolicyTypes SET
                PolicyName=@Name,
                PolicyCategory=@Cat,
                BasePremium=@Premium,
                InterestRate=@Rate,
                MinAge=@Min,
                MaxAge=@Max,
                TermYears=@Term,
                IsActive=@Active
                WHERE PolicyTypeId=@Id";

            SqlParameter[] prm =
            {
                new("@Name", p.PolicyName),
                new("@Cat", p.PolicyCategory),
                new("@Premium", p.BasePremium),
                new("@Rate", p.InterestRate),
                new("@Min", p.MinAge),
                new("@Max", p.MaxAge),
                new("@Term", p.TermYears),
                new("@Active", p.IsActive),
                new("@Id", p.PolicyTypeId)
            };

            return _db.ExecuteNonQuery(query, prm) > 0;
        }

        public List<PolicyType> GetActivePolicies()
        {
            string query = @"
                SELECT *
                FROM PolicyTypes
                WHERE IsActive = 1
                ORDER BY PolicyTypeId";

            var dt = _db.ExecuteQuery(query);

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
        public bool PurchasePolicy(int userId, PolicyType policy)
        {
            DateTime start = DateTime.Now;
            DateTime end = start.AddYears(policy.TermYears);

            string query = @"
                INSERT INTO CustomerPolicies
                (UserId, PolicyTypeId, StartDate,
                 EndDate, PremiumAmount, PolicyStatus)
                VALUES
                (@UserId, @PolicyId, @Start,
                 @End, @Premium, 'Active')";

            SqlParameter[] p =
            {
                new("@UserId", userId),
                new("@PolicyId", policy.PolicyTypeId),
                new("@Start", start),
                new("@End", end),
                new("@Premium", policy.BasePremium)
            };

            return _db.ExecuteNonQuery(query, p) > 0;
        }

    }
}
