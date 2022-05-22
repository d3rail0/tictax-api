using entities.Model;
using entities.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entities.Repositories
{
    public class ProfileActivityRepository : GenericRepository<ProfileActivity>, IProfileActivityRepository
    {

        public ProfileActivityRepository(AppDbContext appDbContext) :
            base(appDbContext)
        {

        }

        public async Task<IEnumerable<ProfileActivity>> GetProfileActivitiesForUser(string username)
        {
            return await Find(activity => activity.UsernameTo == username);
        }

        public async Task<ProfileActivity> GetProfileActivity(string initiator, string recipient)
        {
            return await _dbSet.Where(activity => activity.UsernameFrom == initiator && activity.UsernameTo == recipient).FirstOrDefaultAsync();
        }

    }
}
