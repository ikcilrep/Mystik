using Mystik.Controllers;

namespace Tests.Helpers
{
    public class UsersControllerProvider : UserServiceProvider
    {
        protected UsersController UsersController { get; set; }

        protected UsersControllerProvider() : base()
        {
            UsersController = new UsersController(UserService);
        }
    }
}