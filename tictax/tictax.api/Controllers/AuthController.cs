using entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tictax.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly ILogger<AuthController> _logger;
        private readonly AppDbContext _appDbContext;

        public AuthController(ILogger<AuthController> logger, AppDbContext appDbContext)
        {
            this._logger = logger;
            this._appDbContext = appDbContext;
        }
        
    }
}
