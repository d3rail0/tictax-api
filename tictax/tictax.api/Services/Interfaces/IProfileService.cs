using entities.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using tictax.api.Data.Models;

namespace tictax.api.Services.Interfaces
{
    public interface IProfileService
    {

        public Task<IEnumerable<ProfileActivity>> GetActivitiesForUser(string username);
        public Task<bool> UpdateUserStatus(bool newStatus);
        public Task<bool> LikeUserProfile(string username);
        public Task<bool> RemoveLike(string username);

        public Task<ProfileDataResponse> GetProfileData(string username);
    }
}
