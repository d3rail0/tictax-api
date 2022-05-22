using entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entities.Repositories.Interfaces
{
    public interface IProfileActivityRepository: IGenericRepository<ProfileActivity>
    {

        public Task<IEnumerable<ProfileActivity>> GetProfileActivitiesForUser(string username);
        public Task<ProfileActivity> GetProfileActivity(string initiator, string recipient);

    }
}
