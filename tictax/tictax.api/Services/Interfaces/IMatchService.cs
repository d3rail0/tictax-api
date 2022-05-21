using entities.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace tictax.api.Services.Interfaces
{
    public interface IMatchService
    {
        public Task<IEnumerable<Match>> GetAvailableMatches();

        public int GetTotalMatchCount();
    }
}
