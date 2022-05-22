using entities.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using tictax.api.Data.Models;
using tictax.api.Data.Models.Profile;
using tictax.api.Helpers;
using tictax.api.Services.Interfaces;

namespace tictax.api.Controllers
{

    [ApiController]
    [Authorize]
    [Route("api/profile")]
    public class ProfileController : ControllerBase
    {
        private readonly ILogger<ProfileController> _logger;

        private readonly IUserService _userService;
        private readonly IProfileService _profileService;

        public ProfileController(ILogger<ProfileController> logger,
            IUserService userService,
            IProfileService profileService)
        {
            this._logger = logger;
            _profileService = profileService;
            _userService = userService;
        }

        [HttpGet(), AllowAnonymous]
        public async Task<IActionResult> GetProfileData([FromQuery(Name = "player")] string player)
        {
            var profileDataResp = await _profileService.GetProfileData(player);

            if (profileDataResp == null)
            {
                return BadRequest(
                    new ErrorResponse(
                        Constants.ErrorCodes.UserAlreadyExists,
                        "Specified username doesn't exist"));
            }

            return Ok(profileDataResp);
        }

        [HttpPut()]
        public async Task<IActionResult> UpdateStatus(ProfileAvailableStatusChangeRequest req)
        {
            bool isSucc = await _profileService.UpdateUserStatus(req.NewProfileState);

            if (!isSucc)
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpPost("action/like")]
        public async Task<IActionResult> AddLike(ProfileActionRequest req)
        {
            bool isSucc = await _profileService.LikeUserProfile(req.Username);
            
            if(!isSucc)
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpDelete("action/like")]
        public async Task<IActionResult> RemoveLike(ProfileActionRequest req)
        {
            bool isSucc = await _profileService.RemoveLike(req.Username);

            if (!isSucc)
            {
                return BadRequest();
            }

            return Ok();
        }


    }

}
