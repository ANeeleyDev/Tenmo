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

        //Method Case 1
        [HttpGet("users")]
        public ActionResult<IList<User>> GetUsers()
        {
            return Ok(transferDao.getUsers());
        }

        //Method case 2
        [HttpPut("sendMoney")]
        public ActionResult CreateTransaction(int toUserId, decimal amount)
        {
            string userIdString = User.FindFirst("sub")?.Value;
            int fromUserId = Convert.ToInt32(userIdString);

            //toUserId = 0;
            //amount = 0m;

            transferDao.Transaction(fromUserId, toUserId, amount);

            return Ok();
        }



        //CTOR
        public TransferController(ITransferDao _transferDao)
        {
            transferDao = _transferDao;
        }
    }
    //DANGER ZONE
}
