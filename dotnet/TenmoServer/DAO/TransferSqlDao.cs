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

        //Method Case 1
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

        public bool transfer(int fromUserId, int toUserId, decimal amount)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string Sql = "Begin Transaction; " +
                        "UPDATE accounts SET balance = balance - @amount " +
                        "WHERE user_id = @fromUserId; " +
                        "UPDATE accounts SET balance = balance + @amount " +
                        "WHERE user_id = @toUserId; " + 
                        "COMMIT;";
                    SqlCommand cmd = new SqlCommand(Sql, conn);
                    cmd.Parameters.AddWithValue("@amount", amount);
                    cmd.Parameters.AddWithValue("@fromUserId", fromUserId);
                    cmd.Parameters.AddWithValue("@toUserId", toUserId);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    //If 0 rows were affected, this will fail and return false
                    return (rowsAffected > 0);
                }             
            }
            catch (Exception)
            {
                throw;
            }
        }

        //populating transfer table
        public TransferRequest createTransferReceipt(TransferRequest transferRequest)
        {
            
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "INSERT INTO transfers (transfer_type_id, transfer_status_id, account_from, account_to, amount) " +
                                 "OUTPUT INSERTED.transfer_id " +
                                 "VALUES (@transfer_type_id, @transfer_status_id, @account_from, @account_to, @amount);";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                   
                    cmd.Parameters.AddWithValue("@transfer_status_id", transferRequest.TransferStatusId);
                    cmd.Parameters.AddWithValue("@transfer_type_id", transferRequest.TransferTypeId);
                    cmd.Parameters.AddWithValue("@account_from", transferRequest.AccountFrom);
                    cmd.Parameters.AddWithValue("@account_to", transferRequest.AccountTo);
                    cmd.Parameters.AddWithValue("@amount", transferRequest.Amount);

                    transferRequest.TransferId = Convert.ToInt32(cmd.ExecuteScalar());

                }
            }
            catch (Exception)
            {

                throw;
            }

            return transferRequest;
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

        public Transfer GetTransferFromReader(SqlDataReader reader)
        {
            Transfer transfer = new Transfer();
            transfer.TransferId = Convert.ToInt32(reader["transfer_id"]);
            transfer.TransferTypeId = Convert.ToInt32(reader["transfer_type_id"]);
            transfer.TransferStatusId = Convert.ToInt32(reader["transfer_status_id"]);
            transfer.AccountFrom = Convert.ToInt32(reader["account_from"]);
            transfer.AccountTo = Convert.ToInt32(reader["account_to"]);
            transfer.Amount = Convert.ToDecimal(reader["amount"]);

            return transfer;
        }
    }
}
