using entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tictax.api.Services.Interfaces;

namespace tictax.api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly ILogger<AuthController> _logger;

        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public AuthController(
            ILogger<AuthController> logger, 
            IAuthService authService, 
            IUserService userService)
        {
            this._logger = logger;
            _authService = authService;
            _userService = userService;
        }

    }
}
