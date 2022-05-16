using System.Threading.Tasks;
using tictax.api.Data.Models;

namespace tictax.api.Services.Interfaces
{
    public interface IAuthService
    {

        /// <summary>
        /// Registers account if there is no another user with the same username.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>true if account was successfully registered.</returns>
        public Task<bool> RegisterAccount(UserDto user);

        public string CreateTokenAsync(UserDto user);

        /// <summary>
        /// Verifies whether user credentials are valid
        /// against values in data storage.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>true is credentials are valid.</returns>
        public Task<bool> VerifyCredentials(UserDto user);

    }
}
