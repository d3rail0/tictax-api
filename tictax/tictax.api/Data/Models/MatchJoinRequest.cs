using System.ComponentModel.DataAnnotations;

namespace tictax.api.Data.Models
{
    public class MatchJoinRequest
    {
        [Required]
        public int ? MatchId { get; set; }
    }
}
