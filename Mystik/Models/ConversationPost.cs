using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Mystik.Models
{
    public class ConversationPost
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public byte[] PasswordHashData { get; set; }
        public IEnumerable<Guid> UsersIds { get; set; }

    }
}