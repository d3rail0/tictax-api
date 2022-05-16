using entities.Model;
using System.Threading.Tasks;

namespace tictax.api.Services.Interfaces
{
    public interface IUserService
    {

        /// <summary>
        /// Extracts username from JWT used within current HttpContext
        /// </summary>
        /// <returns></returns>
        public string GetMyUsername();

        public Task<User> GetUserAsync(string username);
        public Task RegisterAccountAsync(User user);
    }
}
