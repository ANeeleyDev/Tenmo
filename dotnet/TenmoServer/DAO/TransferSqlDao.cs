using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public class TransferSqlDao : ITransferDao
    {
        private readonly string connectionString;

        public TransferSqlDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public IList<User> getUsers()
        {
            IList<User> listOfUsers = new List<User>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT * FROM users;";
                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    User user = GetUserFromReader(reader);
                    listOfUsers.Add(user);
                }
            }

            return listOfUsers;
        }

        public User GetUserFromReader(SqlDataReader reader)
        {
            User user = new User();
            user.Username = Convert.ToString(reader["username"]);
            user.UserId = Convert.ToInt32(reader["user_id"]);
            user.PasswordHash = Convert.ToString(reader["password_hash"]);
            user.Salt = Convert.ToString(reader["salt"]);

            return user;
        }
    }
}
