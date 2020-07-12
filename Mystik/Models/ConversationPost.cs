using System;
using System.Collections.Generic;

namespace Mystik.Models
{
    public class ConversationPost
    {
        public string Name { get; set; }
        public IEnumerable<Guid> UsersIds { get; set; }

    }
}