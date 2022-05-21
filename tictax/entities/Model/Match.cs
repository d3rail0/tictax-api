using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entities.Model
{
    [Table("Match")]
    public class Match
    {
        [Key]
        public int Id { get; set; }

        // Unix epoch
        public int CreationTime { get; set; }

        [ForeignKey(nameof(Owner)), Required]
        public string OwnerUsername { get; set; }

        [ForeignKey(nameof(Opponent))]
        public string OpponentUsername { get; set; }

        public User Owner { get; set; }
        public User Opponent { get; set; }

    }
}
