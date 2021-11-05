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
        public void Transaction(int fromUserId, int toUserId, decimal amount);
    }
}
