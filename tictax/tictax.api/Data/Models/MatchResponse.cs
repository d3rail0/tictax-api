namespace tictax.api.Data.Models
{
    public class MatchResponse
    {

        public int MatchId { get; set; }
        public int CreationTime { get; set; }
        public string Owner { get; set; }
        public string Opponent { get; set; }

    }
}
