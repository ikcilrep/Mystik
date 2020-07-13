namespace Mystik.Models.User
{
    public class Put : Patch
    {
        public string Username { get; set; }
        public string Role { get; set; }

        public override Entities.User ToUser(Entities.User originalUser)
        {
            var user = base.ToUser(originalUser);
            user.Username = Username == null ? originalUser.Username : Username;
            user.Role = Role == null ? originalUser.Role : Role;
            return user;
        }
    }
}