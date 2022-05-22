using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entities.Model
{
    [Table("ProfileActivity")]
    public class ProfileActivity
        // This model only tracks "thumbs-up" activity types
        // but it could be modified to support different activites
    {

        [Key]
        public int Id { get; set; }

        // Unix epoch
        public long ActivityTime { get; set; }

        [ForeignKey(nameof(Initiator))]
        public string UsernameFrom { get; set; }

        [ForeignKey(nameof(Recipient))]
        public string UsernameTo { get; set; }

        public User Initiator { get; set; }
        public User Recipient { get; set; }

    }
}
