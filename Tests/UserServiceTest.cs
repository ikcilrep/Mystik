using System;
using System.Linq;
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

        [Fact]
        public async Task Create_ReturnedUserHasAnId()
        {
            var user = await _provider.UserService.Create(
                MockUserService.User1.Nickname,
                MockUserService.User1.Username,
                MockUserService.User1.Password);

            Assert.True(user.Id != null && user.Id != Guid.Empty);
        }

        [Fact]
        public async Task Create_AddsExactlyOneUserToTheDatabase()
        {
            var user = await _provider.UserService.Create(
                MockUserService.User1.Nickname,
                MockUserService.User1.Username,
                MockUserService.User1.Password);

            Assert.Equal(2, _provider.Context.Users.Count());
        }

        [Fact]
        public async Task Retrieve_ReturnsCorrectUser()
        {
            var actualUser = await _provider.UserService.Retrieve(MockUserService.User2.Id);

            Assert.Equal(MockUserService.User2, actualUser);
        }

        public void Dispose()
        {
            _provider.Dispose();
        }
    }
}