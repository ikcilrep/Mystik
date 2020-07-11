using System.ComponentModel.DataAnnotations;
using Mystik.Entities;

namespace Mystik.Models
{
    public class MessagePatch
    {
        [Required]
        public byte[] EncryptedContent { get; set; }
    }
}