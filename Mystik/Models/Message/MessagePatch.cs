using System.ComponentModel.DataAnnotations;

namespace Mystik.Models.Message
{
    public class Patch
    {
        [Required]
        public byte[] EncryptedContent { get; set; }
    }
}