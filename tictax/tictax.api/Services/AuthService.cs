using entities.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using tictax.api.Services.Interfaces;

namespace tictax.api.Services
{
    public class AuthService : IAuthService
    {

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IUnitOfWork _unitOfWork;

        public AuthService(IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }

    }
}
