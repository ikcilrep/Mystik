using System;
using System.Threading.Tasks;
using Tests.Helpers;
using Xunit;

namespace Tests
{
    public class UserServiceTest : IDisposable
    {
        private UserServiceProvider _provider;

        public UserServiceTest()
        {
            _provider = new UserServiceProvider();
        }

        [Fact]
        public async Task Create_ReturnsCorrectUser()
        {
            var expectedUser = MockUserService.User1;

            var actualUser = await _provider.UserService.Create(
                expectedUser.Nickname,
                expectedUser.Username,
                expectedUser.Password);

            actualUser.Id = expectedUser.Id;
            actualUser.PasswordHash = expectedUser.PasswordHash;
            actualUser.PasswordSalt = expectedUser.PasswordSalt;


            Assert.Equal(expectedUser, actualUser);
        }

        public void Dispose()
        {
            _provider.Dispose();
        }
    }
}