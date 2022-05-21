using entities.Model;
using entities.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entities.Repositories
{
    public class MatchRepository : GenericRepository<Match>, IMatchRepository
    {

        public MatchRepository(AppDbContext appDbContext) :
           base(appDbContext)
        {

        }

        public async Task<IEnumerable<Match>> GetAvailableMatches()
        {
            return await Find(m => m.OpponentUsername == null);
        }

        public int GetTotalMatchCount()
        {
            return  _dbSet.Count();
        }
    }
}
