using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entities.Model
{
    [Table("User")]
    public class User
    {
        [Key]
        public string Username { get; set; }
        [Required]
        public byte[] PasswordHash { get; set; }
        [Required]
        public byte[] PasswordSalt { get; set; }

        public int TotalWins { get; set; } = 0;
        public int TotalLoses { get; set; } = 0;
        public int TotalGames { get; set; } = 0;

        [InverseProperty(nameof(Match.Owner))]
        public ICollection<Match> OwnerMatches { get; set; }

        [InverseProperty(nameof(Match.Opponent))]
        public ICollection<Match> OpponentMatches { get; set; }
    }

}
