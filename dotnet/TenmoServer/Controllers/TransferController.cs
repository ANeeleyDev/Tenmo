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

        [HttpGet("users")]
        public ActionResult<IList<User>> GetUsers()
        {
            return Ok(transferDao.getUsers());
        }

        //CTOR
        public TransferController(ITransferDao _transferDao)
        {
            transferDao = _transferDao;
        }
    }


}
