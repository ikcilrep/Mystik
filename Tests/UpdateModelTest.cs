using Mystik.Entities;
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
            var user2 = MockUserService.User2;
            var model = new UserPut
            {
                Nickname = user2.Nickname,
                Username = user2.Username,
                Role = user2.Role,
            };

            var actualUser = model.ToUser(originalUser);
            var expectedUser = new User
            {
                Nickname = user2.Nickname,
                Username = user2.Username,
                Role = user2.Role,
                Id = originalUser.Id,
                PasswordHash = originalUser.PasswordHash,
                PasswordSalt = originalUser.PasswordSalt,
            };
            Assert.Equal(expectedUser, actualUser);
        }
    }
}