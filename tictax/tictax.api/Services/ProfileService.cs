using entities.Model;
using entities.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using tictax.api.Data.Models;
using tictax.api.Data.Models.Profile;
using tictax.api.Helpers;
using tictax.api.Services.Interfaces;

namespace tictax.api.Services
{
    public class ProfileService: IProfileService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IUserService _userService;

        public ProfileService(
            IUnitOfWork unitOfWork, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
        }


        public async Task<IEnumerable<ProfileActivity>> GetActivitiesForUser(string username)
        {
            return await _unitOfWork.ProfileActivites.GetProfileActivitiesForUser(username);
        }

        public async Task<bool> LikeUserProfile(string username)
        {
            // Check if user is trying to like their own profile
            var initiatorUsername = _userService.GetMyUsername();

            if(initiatorUsername == username)
            {
                // Cannot like your own profile
                return false;
            }

            var user = await _userService.GetUserAsync(username);

            // Verify that user exists
            if (user == null)
            {
                return false;
            }

            // Check if the profile is already liked
            var pActivityDb = await _unitOfWork.ProfileActivites.GetProfileActivity(initiatorUsername, username);
            if (pActivityDb != null)
            {
                // The profile is already liked, no need to continue
                return true;
            }


            var profileActivity = new ProfileActivity();

            profileActivity.ActivityTime = DateHelper.DateTimeToUnixTimestamp(DateTime.UtcNow);
            profileActivity.UsernameFrom = initiatorUsername;
            profileActivity.UsernameTo = username;

            // Insert profile activity
            await _unitOfWork.ProfileActivites.Add(profileActivity);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<bool> RemoveLike(string username)
        {
            var initiatorUsername = _userService.GetMyUsername();

            var user = await _userService.GetUserAsync(username);

            // Verify that user exists
            if (user == null)
            {
                return false;
            }

            // Check if the like exists
            var profileActivity = await _unitOfWork.ProfileActivites.GetProfileActivity(initiatorUsername, username);

            if (profileActivity == null)
            {
                return false;
            }

            _unitOfWork.ProfileActivites.Remove(profileActivity);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<bool> UpdateUserStatus(bool newStatus)
        {
            var initiatorUsername = _userService.GetMyUsername();
            var user = await _userService.GetUserAsync(initiatorUsername);

            // Verify that user exists
            if (user == null)
            {
                return false;
            }

            // Just something basic for now.
            user.IsAvailable = newStatus;

            await _unitOfWork.CompleteAsync();

            return true;
        }

        /// <summary>
        /// Creates an object containing data about profile or returns
        /// null if user with username doesn't exist.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<ProfileDataResponse> GetProfileData(string username)
        {
            
            var user = await _userService.GetUserAsync(username);

            // Verify that user exists
            if (user == null)
            {
                return null;
            }

            var profileDataResponse = new ProfileDataResponse();

            profileDataResponse.Username = username;
            profileDataResponse.TotalWins = user.TotalWins;
            profileDataResponse.TotalLosses = user.TotalLoses;
            profileDataResponse.TotalDraws = user.TotalGames - (user.TotalWins + user.TotalLoses);

            // Fetch activities
            var feed = await GetActivitiesForUser(username);

            foreach (var activity in feed)
            {
                profileDataResponse.ActivityFeed.Add(
                    new ProfileActivityFeedResponse
                    {
                        Username = activity.UsernameFrom,
                        ActivityTime = activity.ActivityTime
                    });
            }

            return profileDataResponse;
        }


    }
}
