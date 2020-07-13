namespace Mystik.Models.User
{
    public class Patch
    {
        public string Nickname { get; set; }
        public string Password { get; set; }

        public virtual Entities.User ToUser(Entities.User originalUser)
        {
            var user = new Entities.User
            {
                Id = originalUser.Id,
                Username = originalUser.Username,
                Nickname = Nickname == null ? originalUser.Nickname : Nickname,
                Role = originalUser.Role
            };
            if (Password == null)
            {
                user.PasswordHash = originalUser.PasswordHash;
                user.PasswordSalt = originalUser.PasswordSalt;
            }
            else
            {
                user.SetPassword(Password);
            }
            return user;
        }
    }
}