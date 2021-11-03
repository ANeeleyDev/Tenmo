using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.DAO;
using TenmoServer.Security;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        //Properties
        private readonly ITokenGenerator tokenGenerator;
        private readonly IAccountDao accountDao;

        [HttpGet("")]





        //CTOR
        public AccountController(ITokenGenerator _tokenGenerator, IAccountDao _accountDao)
        {
            tokenGenerator = _tokenGenerator;
            accountDao = _accountDao;
        }

    }
    //DANGER ZONE
}
