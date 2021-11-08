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

        //creates and begins a transfer from both userFrom and UserTo within a transaction block.
        //the parameter amount is used both for users sice this is a transaction happens to both of them. The only difference is
        //that one is adding while the other is subtracting that value from their balance.
        //This math only happens on both accounts if both can be done all at once.
        //return type is bool because we just want to confirm that this method worked or not for testing.
        public bool transfer(int fromUserId, int toUserId, decimal amount)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "Begin Transaction; " +
                                 "UPDATE accounts SET balance = balance - @amount " +
                                 "WHERE user_id = @fromUserId; " +

                                 "UPDATE accounts SET balance = balance + @amount " +
                                 "WHERE user_id = @toUserId; " + 
                                 "COMMIT;";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@amount", amount);
                    cmd.Parameters.AddWithValue("@fromUserId", fromUserId);
                    cmd.Parameters.AddWithValue("@toUserId", toUserId);

                    //Tells it to do what we have in our Sql string.
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
        //our parameter is a type TransferRequest so we can call on everything that it holds such as account and user info.
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


        public IList<TransferReceipt> GetTransfersForLoggedInUser(int accountId)
        {
            IList<TransferReceipt> transferReceipts = new List<TransferReceipt>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string sql = "SELECT * " +
                                 "FROM transfers " +
                                 "WHERE account_from = @account_id OR account_to = @account_id;";
                    SqlCommand cmd = new SqlCommand(sql, conn);

                    cmd.Parameters.AddWithValue("@account_id", accountId);
                    

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        TransferReceipt transfer = GetTransferFromReader(reader);
                        transferReceipts.Add(transfer);
                    }

                }
            }
            catch (Exception)
            {

                throw;
            }

            return transferReceipts;
        }

        public string GetUserNameFrom(int transferId)
        {
            string userNameFrom = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "SELECT u.username FROM users u " +
                                 "JOIN accounts a ON u.user_id = a.user_id " +
                                 "JOIN transfers t ON a.account_id = t.account_from " +
                                 "WHERE transfer_id = @transferId;";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@transferId", transferId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                         userNameFrom = Convert.ToString(reader["username"]);
                    }

                }
            }
            catch (Exception)
            {

                throw;
            }
            return userNameFrom;
        }

        public string GetUserNameTo(int transferId)
        {
            string userNameTo = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "SELECT u.username FROM users u " +
                                 "JOIN accounts a ON u.user_id = a.user_id " +
                                 "JOIN transfers t ON a.account_id = t.account_to " +
                                 "WHERE transfer_id = @transferId;";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@transferId", transferId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        userNameTo = Convert.ToString(reader["username"]);
                    }

                }
            }
            catch (Exception)
            {

                throw;
            }
            return userNameTo; 
        }

        //Reader helper for our get user.
        public User GetUserFromReader(SqlDataReader reader)
        {
            User user = new User();
            user.Username = Convert.ToString(reader["username"]);
            user.UserId = Convert.ToInt32(reader["user_id"]);
            user.PasswordHash = Convert.ToString(reader["password_hash"]);
            user.Salt = Convert.ToString(reader["salt"]);

            return user;
        }


        //Reader helper for our transfer.
        public TransferReceipt GetTransferFromReader(SqlDataReader reader)
        {
            TransferReceipt transferReceipt = new TransferReceipt();
            transferReceipt.TransferId = Convert.ToInt32(reader["transfer_id"]);
            transferReceipt.TransferTypeId = Convert.ToInt32(reader["transfer_type_id"]);
            transferReceipt.TransferStatusId = Convert.ToInt32(reader["transfer_status_id"]);
            transferReceipt.AccountFrom = Convert.ToInt32(reader["account_from"]);
            transferReceipt.AccountTo = Convert.ToInt32(reader["account_to"]);
            transferReceipt.Amount = Convert.ToDecimal(reader["amount"]);

            return transferReceipt;
        }

    }
    //DANGER ZONE
}
