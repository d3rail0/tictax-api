using entities.Model;
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
    [Route("api/matches")]
    public class MatchController : ControllerBase
    {

        private readonly ILogger<MatchController> _logger;

        private readonly IUserService _userService;
        private readonly IMatchService _matchService;

        public MatchController(ILogger<MatchController> logger,
            IUserService userService,
            IMatchService matchService)
        {
            this._logger = logger;
            _matchService = matchService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var ms_response = new MatchesResponse();
            ms_response.TotalMatches = _matchService.GetTotalMatchCount();
            
            foreach (Match m in await _matchService.GetAvailableMatches())
            {
                ms_response.AvailableMatches.Add(new MatchResponse
                {
                    MatchId = m.Id,
                    CreationTime = m.CreationTime,
                    Owner = m.OwnerUsername,
                    Opponent = m.OpponentUsername
                });
            }

            return Ok(ms_response);
        }

    }
}
