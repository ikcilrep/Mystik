using System.ComponentModel.DataAnnotations;

namespace Mystik.Models
{
    public class ConversationPatch
    {
        [Required]
        public string Name { get; set; }
    }
}