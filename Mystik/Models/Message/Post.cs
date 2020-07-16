using System.ComponentModel.DataAnnotations;

namespace Mystik.Models.Message
{
    public class Post
    {
        [Required]
        public byte[] EncryptedContent { get; set; }
    }
}