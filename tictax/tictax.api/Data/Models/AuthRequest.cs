using System.ComponentModel.DataAnnotations;

namespace tictax.api.Data.Models
{
    public class AuthRequest
    {
        [Required]
        [StringLength(32, MinimumLength = 3, ErrorMessage = "Username length must be in range [3, 32]")]
        public string Username { get; set; }
        [Required]
        [StringLength(32, MinimumLength = 5, ErrorMessage = "Password length must be in range [5, 32]")]
        public string Password { get; set; }
    }
}
