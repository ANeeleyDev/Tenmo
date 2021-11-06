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
        public ActionResult CreateTransaction(TransferRequest transferRequest)
        {
            //Getting current logged in user's ID
            string userIdString = User.FindFirst("sub")?.Value;
            int fromUserId = Convert.ToInt32(userIdString);

            Account fromAccount = accountDao.GetAccount(transferRequest.UserFrom);
            Account toAccount = accountDao.GetAccount(transferRequest.UserTo);

            transferRequest.AccountFrom = fromAccount.AccountId;
            transferRequest.AccountTo = toAccount.AccountId;

            int toUserId = transferRequest.UserTo;
            decimal amount = transferRequest.Amount;

            transferDao.transfer(fromUserId, toUserId, amount);

            transferDao.createTransferReceipt(transferRequest);         


            return Ok();
        }

        //[HttpPost("sendMoney")]
        //public ActionResult<Transfer> CreateTransactionReceipt(Transfer transfer)
        //{
        //    return transferDao.createTransferReceipt(transfer);
        //}

        //CTOR
        public TransferController(ITransferDao _transferDao, IAccountDao accountDao)
        {
            transferDao = _transferDao;
            this.accountDao = accountDao;
        }
    }
    //DANGER ZONE
}
