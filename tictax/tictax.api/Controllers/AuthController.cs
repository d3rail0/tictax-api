using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using tictax.api.Data.Models;
using tictax.api.Helpers;
using tictax.api.Services.Interfaces;

namespace tictax.api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/auth")]
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

        [HttpPost]
        [AllowAnonymous]
        [Route("register")]
        public async Task<ActionResult<AuthResponse>> Register(AuthRequest request)
        {
            bool isSuccessful = await _authService.RegisterAccount(request);
            
            if(!isSuccessful)
            {
                return BadRequest(
                    new ErrorResponse(
                        Constants.ErrorCodes.UserAlreadyExists, 
                        "Account already exists or input data is invalid"));
            }

            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<ActionResult<AuthResponse>> Login(AuthRequest request)
        {
            bool isInputValid = await _authService.VerifyCredentials(request);
            
            if (!isInputValid)
            {
                return BadRequest(
                    new ErrorResponse(
                        Constants.ErrorCodes.WrongPassword, 
                        "Entered credentials are invalid"));
            }

            string token = _authService.CreateToken(request);

            return Ok(new AuthResponse() { Token = token });
        }

        [HttpGet]
        [Route("profile")]
        public IActionResult GetMyProfile()
        {
            var username = _userService.GetMyUsername();
            return Ok(username);
        }


    }
}
