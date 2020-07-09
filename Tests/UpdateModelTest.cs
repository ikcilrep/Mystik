using Mystik.Models;
using Tests.Helpers;
using Xunit;

namespace Tests
{
    public class UpdateModelTest
    {
        [Fact]
        public void PutToUser_EmptyModel_ReturnsTheSameUser()
        {
            var originalUser = MockUserService.User1;
            var model = new UserPut
            {
                Nickname = null,
                Username = null,
                Role = null,
                Password = null
            };

            var actualUser = model.ToUser(originalUser);
            Assert.Equal(originalUser, actualUser);
        }
    }
}