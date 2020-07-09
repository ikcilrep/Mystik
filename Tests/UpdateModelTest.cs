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

        [Fact]
        public void PutToUser_NotEmptyModel_ReturnsDifferentUser()
        {
            var originalUser = MockUserService.User1;
            var model = new UserPut
            {
                Nickname = MockUserService.User2.Nickname,
                Username = MockUserService.User2.Username,
                Role = MockUserService.User2.Role,
                Password = MockUserService.User2.Password
            };

            var actualUser = model.ToUser(originalUser);
            Assert.NotEqual(originalUser, actualUser);

        }
    }
}