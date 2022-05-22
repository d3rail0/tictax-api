using entities.Model;
using entities.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using tictax.api.Helpers;
using tictax.api.Services.Interfaces;

namespace tictax.api.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(
            IHttpContextAccessor httpContextAccessor,
            IUnitOfWork unitOfWork)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }

        public string GetMyUsername()
        {
            var result = string.Empty;
            if (_httpContextAccessor.HttpContext != null)
            {
                result = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            }
            return result;
        }

        public bool AmIAuthenticated()
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                return _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated && !IsTokenExpired();
            }
            return false;
        }

        public DateTime GetExpiryDate()
        {
            DateTime result = DateTime.UtcNow;
            if (_httpContextAccessor.HttpContext != null)
            {

                long unixTime = Convert.ToInt64(_httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "exp").Value);
                result = DateHelper.UnixTimestampToDateTime(unixTime);
            }
            return result;
        }

        public bool IsTokenExpired()
        {
            DateTime currTime = DateTime.UtcNow;
            DateTime expiryDate = GetExpiryDate();
            if (DateTime.Compare(currTime, expiryDate) <= 0)
            {
                return false;
            }
            return true;
        }

        public async Task<User> GetUserAsync(string username)
        {
            return await _unitOfWork.Users.GetByUsernameAsync(username);
        }

        public async Task RegisterAccountAsync(User user)
        {
            await _unitOfWork.Users.Add(user);
        }

    }
}
