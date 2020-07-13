using System;
using System.ComponentModel.DataAnnotations;

namespace Mystik.Models.User
{
    public class Authentication
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
