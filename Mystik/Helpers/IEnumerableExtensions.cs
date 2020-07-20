using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mystik.Entities;

namespace Mystik.Helpers
{
    public static class IEnumerableExtensions
    {
        public static async Task<List<byte[]>> GetEncryptedContent(this IEnumerable<Message> messages)
        {
            var result = new List<byte[]>();

            foreach (var message in messages.OrderBy(m => m.SentTime))
            {
                result.Add(await message.GetEncryptedContent());
            }

            return result;
        }

        public static List<string> ToStringList<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Select(e => e.ToString()).ToList();
        }
    }
}