using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public class AccountSqlDao : IAccountDao
    {
        private readonly string connectionString;

        public AccountSqlDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public decimal GetBalance(int userId)
        {
            Account returnAcc = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();


                    string sql = "SELECT balance, user_id, account_id " +
                                 "FROM accounts " +
                                 "WHERE user_id = @user_id;";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@user_id", userId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        returnAcc = GetAccountFromReader(reader);
                    }

                }
            }
            catch (Exception theseHands)
            {

                Console.WriteLine(theseHands.Message);
            }

            return returnAcc.Balance;

        }

        public Account GetAccountFromReader(SqlDataReader reader)
        {
            Account acc = new Account();
            {
                acc.AccountId = Convert.ToInt32(reader["account_id"]);
                acc.UserId = Convert.ToInt32(reader["user_id"]);
                acc.Balance = Convert.ToDecimal(reader["balance"]);
            }
            return acc;
        }


    }
    //DANGER ZONE
}
