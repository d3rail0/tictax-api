using entities.Model;
using entities.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entities.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {

        public UserRepository(AppDbContext appDbContext, ILogger logger) :
            base(appDbContext, logger)  
        {

        }

        public async Task<User> GetByUsername(string username)
        {
            return await _dbSet.FindAsync(username);   
        }
    }
}
