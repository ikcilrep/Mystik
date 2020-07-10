using System.Collections.Generic;
using System.Linq;

namespace Mystik.Entities
{
    public class Conversation
    {
        public string Name { get; set; }

        public ICollection<User> Admins { get; set; }

        public ICollection<User> Users { get; set; }

        public ICollection<Message> Messages { get; set; }

        public byte[] PasswordHashData { get; set; }
    }
}