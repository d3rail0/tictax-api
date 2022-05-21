using entities.Model;
using entities.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using tictax.api.Services.Interfaces;

namespace tictax.api.Services
{
    public class MatchService : IMatchService
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;

        public MatchService(
            IHttpContextAccessor httpContextAccessor,
            IUnitOfWork unitOfWork)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Match>> GetAvailableMatches()
        {
            return await _unitOfWork.Matches.GetAvailableMatches();
        }

        public int GetTotalMatchCount()
        {
            return _unitOfWork.Matches.GetTotalMatchCount();
        }

    }
}
