using System.Collections.Generic;
using tictax.api.Data.Models.Profile;

namespace tictax.api.Data.Models
{
    public class ProfileDataResponse
    {
        public string Username { get; set; }
        public int TotalWins { get; set; }
        public int TotalLosses { get; set; }
        public int TotalDraws { get; set; }
        public bool IsAvailable { get; set; }
        public List<ProfileActivityFeedResponse> ActivityFeed { get; set; } = new List<ProfileActivityFeedResponse>();
    }
}
