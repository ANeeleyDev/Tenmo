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
    [Authorize]
    public class AccountController : ControllerBase
    {
        //Properties
        private readonly IAccountDao accountDao;

        [HttpGet("balance")]
        public decimal GetBalance(int userId)
        {
            string userIdString = User.FindFirst("sub")?.Value;
            userId = Convert.ToInt32(userIdString);

            Account acc = accountDao.GetAccount(userId);
            return acc.Balance;
        }

        [HttpGet("{userId}")]
        [AllowAnonymous]
        public int GetAccountId(int userId)
        {
            //Account acc = new Account();
            //string userIdString = User.FindFirst("sub")?.Value;
            //userId = Convert.ToInt32(userIdString);

            //Account userIdString = acc.UserId;

            Account acc = accountDao.GetAccount(userId);
            return acc.AccountId;
        }




        //CTOR
        public AccountController(IAccountDao _accountDao)
        {
            accountDao = _accountDao;
        }

    }
    //DANGER ZONE
}
