using System.ComponentModel.DataAnnotations;

namespace Mystik.Models
{
    public class MessagePost
    {
        [Required]
        public byte[] EncryptedContent { get; set; }
    }
}