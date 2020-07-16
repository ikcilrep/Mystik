using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Mystik.Models.Conversation
{
    public class Post
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public byte[] PasswordHashData { get; set; }
        public IEnumerable<Guid> UsersIds { get; set; }

    }
}