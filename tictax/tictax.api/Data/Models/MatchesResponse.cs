using System.Collections.Generic;

namespace tictax.api.Data.Models
{
    public class MatchesResponse
    {

        public int TotalMatches { get; set; } = 0;
        public List<MatchResponse> AvailableMatches { get; set; } = new List<MatchResponse>();

    }
}
