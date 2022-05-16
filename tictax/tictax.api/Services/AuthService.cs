using entities.Model;
using entities.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using tictax.api.Data.Models;
using tictax.api.Services.Interfaces;

namespace tictax.api.Services
{
    public class AuthService : IAuthService
    {

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IUnitOfWork _unitOfWork;

        private readonly IUserService _userService;

        public AuthService(
            IHttpContextAccessor httpContextAccessor,
            IUnitOfWork unitOfWork, IUserService userService)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

 
        /// <summary>
        /// Creates a JWT token with a username claim.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public string CreateToken(AuthRequest user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                Environment.GetEnvironmentVariable("TICTAX_JWT_KEY"))
                );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
        {
            using (var hmac = new HMACSHA512())
            {
                salt = hmac.Key;
                hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] hash, byte[] salt)
        {
            using (var hmac = new HMACSHA512(salt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(hash);
            }
        }

        public async Task<bool> RegisterAccount(AuthRequest user)
        {
            // Get user from user repo
            User modUser = await _userService.GetUserAsync(user.Username);

            // Check if user exists
            if (modUser != null)
            {
                // User already exists, cannot register
                return false;
            }

            // Create hash and salt 
            CreatePasswordHash(user.Password, out byte[] pHash, out byte[] pSalt);

            // Apply hash and salt to selected user
            modUser = new User();
            modUser.Username = user.Username;
            modUser.PasswordSalt = pSalt;
            modUser.PasswordHash = pHash;

            await _unitOfWork.Users.Add(modUser);

            // Save changes in unit of work
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<bool> VerifyCredentials(AuthRequest user)
        {
            // Get user from user repo
            User modUser = await _userService.GetUserAsync(user.Username);

            // Check if user exists
            if (modUser == null)
            {
                // User not found with entered Username
                return false;
            }

            // Compare input credentials with the credentials from repo
            if(!VerifyPasswordHash(user.Password, modUser.PasswordHash, modUser.PasswordSalt))
            {
                // User exists, but inserted password is wrong
                return false;
            }

            return true;
        }
    }
}
