using Mystik.Entities;

namespace Mystik.Models
{
    public class UserPut : UserPatch
    {
        public string Username { get; set; }
        public string Role { get; set; }

        public override User ToUser(User originalUser)
        {
            var user = base.ToUser(originalUser);
            user.Username = Username == null ? originalUser.Username : Username;
            user.Role = Role == null ? originalUser.Role : Role;
            return user;
        }
    }
}