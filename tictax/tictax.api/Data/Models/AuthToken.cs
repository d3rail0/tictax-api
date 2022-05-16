using System;

namespace tictax.api.Data.Models
{
    public class AuthToken
    {
        public string Token { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Expires { get; set; }
    }
}
