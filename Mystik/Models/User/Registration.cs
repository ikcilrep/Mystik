using System.ComponentModel.DataAnnotations;

namespace Mystik.Models.User
{
    public class Registration
    {
        [Required]
        public string Nickname { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
