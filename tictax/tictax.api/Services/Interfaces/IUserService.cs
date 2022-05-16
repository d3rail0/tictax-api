using entities.Model;
using System;
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

        /// <summary>
        /// Extracts expiry date of JWT token from within current HttpContext
        /// </summary>
        /// <returns></returns>
        public DateTime GetExpiryDate();

        public bool IsTokenExpired();

        public Task<User> GetUserAsync(string username);
        public Task RegisterAccountAsync(User user);
    }
}
