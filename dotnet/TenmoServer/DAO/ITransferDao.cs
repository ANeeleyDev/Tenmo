using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface ITransferDao
    {
        //Method Case 1
        public IList<User> getUsers();

        //Method case 2
        public bool transfer(int fromUserId, int toUserId, decimal amount);
        //getAccountId allows us to populate the transfer table with the account id

        //populating the transfer table
        public TransferRequest createTransferReceipt(TransferRequest transferRequest);
    }
}
