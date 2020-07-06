using System;
using System.ComponentModel.DataAnnotations;

namespace Mystik.Models
{
    public class UserPatch
    {
        public string Nickname { get; set; }
        public string Password { get; set; }
    }
}