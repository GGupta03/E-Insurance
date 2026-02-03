using System.Data;
using Microsoft.Data.SqlClient;
using E_Insurance_App.Helpers;
using E_Insurance_App.Models.Entities;

namespace E_Insurance_App.Repositories
{
    public class UserRepository : BaseRepository
    {
        public UserRepository(DbHelper db) : base(db) { }

        public User GetUserByEmail(string email)
        {
            string query = "SELECT * FROM Users WHERE Email = @Email";

            SqlParameter[] parameters =
            {
                new SqlParameter("@Email", email)
            };

            DataTable dt = _db.ExecuteQuery(query, parameters);

            if (dt.Rows.Count == 0)
                return null;

            DataRow row = dt.Rows[0];

            return new User
            {
                UserId = (int)row["UserId"],
                FirstName = row["FirstName"].ToString(),
                LastName = row["LastName"].ToString(),
                Email = row["Email"].ToString(),
                PasswordHash = row["PasswordHash"].ToString(),
                Role = row["Role"].ToString(),
                IsActive = (bool)row["IsActive"]
            };
        }
        public List<User> GetAllUsers()
        {
            string query = "SELECT UserId, FirstName, LastName, Email, Role, IsActive FROM Users";

            DataTable dt = _db.ExecuteQuery(query);

            List<User> users = new List<User>();

            foreach (DataRow row in dt.Rows)
            {
                users.Add(new User
                {
                    UserId = (int)row["UserId"],
                    FirstName = row["FirstName"].ToString(),
                    LastName = row["LastName"].ToString(),
                    Email = row["Email"].ToString(),
                    Role = row["Role"].ToString(),
                    IsActive = (bool)row["IsActive"]
                });
            }

            return users;
        }
        public bool CreateUser(User user)
        {
            string query = @"
            INSERT INTO Users 
            (FirstName, LastName, Email, PasswordHash, Role, IsActive)
            VALUES
            (@FirstName, @LastName, @Email, @PasswordHash, @Role, @IsActive)";

            SqlParameter[] parameters =
            {
                new SqlParameter("@FirstName", user.FirstName),
                new SqlParameter("@LastName", user.LastName),
                new SqlParameter("@Email", user.Email),
                new SqlParameter("@PasswordHash", user.PasswordHash),
                new SqlParameter("@Role", user.Role),
                new SqlParameter("@IsActive", user.IsActive)
            };

            return _db.ExecuteNonQuery(query, parameters) > 0;
        }

    }
}
