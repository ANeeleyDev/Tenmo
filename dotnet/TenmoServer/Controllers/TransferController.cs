using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.DAO;
using TenmoServer.Models;
using TenmoServer.Security;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TransferController : ControllerBase
    {
        //Properties
        private readonly ITransferDao transferDao;
        private readonly IAccountDao accountDao;

        //Method Case 1
        [HttpGet("users")]
        public ActionResult<IList<User>> GetUsers()
        {
            return Ok(transferDao.getUsers());
        }

        //Method case 2
        [HttpPost]
        public void CreateTransaction(TransferRequest transferRequest)
        {
            //Getting current logged in user's ID
            string userIdString = User.FindFirst("sub")?.Value;
            int fromUserId = Convert.ToInt32(userIdString);

            //these two lines are getting the whole account and assigning it to that variable
            Account fromAccount = accountDao.GetAccount(transferRequest.UserFrom);
            Account toAccount = accountDao.GetAccount(transferRequest.UserTo);

            //These two lines are asigning the account id.
            transferRequest.AccountFrom = fromAccount.AccountId;
            transferRequest.AccountTo = toAccount.AccountId;

            //defining the parameters we need to call on our Dao method for a transfer.
            int toUserId = transferRequest.UserTo;
            decimal amount = transferRequest.Amount;

            transferDao.transfer(fromUserId, toUserId, amount);

            transferDao.createTransferReceipt(transferRequest);         
            
        }

        

        //CTOR
        public TransferController(ITransferDao _transferDao, IAccountDao accountDao)
        {
            transferDao = _transferDao;
            this.accountDao = accountDao;
        }
    }
    //DANGER ZONE
}
