using System;
using System.Linq;
using System.Threading.Tasks;
using Mystik.Models;
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

        [Fact]
        public async Task Update_ChangesValues()
        {
            var model = new UserPut
            {
                Nickname = MockUserService.NotExistingUser.Nickname,
                Username = MockUserService.NotExistingUser.Username
            };

            await _provider.UserService.Update(MockUserService.User2.Id, model);

            var actualUser = _provider.Context.Users.Single();

            Assert.Equal(MockUserService.NotExistingUser.Nickname, actualUser.Nickname);
            Assert.Equal(MockUserService.NotExistingUser.Username, actualUser.Username);
        }

        [Fact]
        public async Task Delete_RemovesExactlyOneUserFromTheDatabase()
        {
            var id = MockUserService.User2.Id;
            await _provider.UserService.Delete(id);

            Assert.Equal(0, _provider.Context.Users.Count());
        }

        [Fact]
        public async Task Authenticate_WithValidCredentials_ReturnsCorrectUser()
        {
            var expectedUser = MockUserService.User2;
            var actualUser = await _provider.UserService.Authenticate(
                expectedUser.Username,
                expectedUser.Password);

            Assert.Equal(expectedUser, actualUser);
        }

        [Fact]
        public async Task Authenticate_WithInvalidCredentials_ReturnsNull()
        {
            var user = await _provider.UserService.Authenticate(
                MockUserService.User2.Username,
                MockUserService.NotExistingUser.Password);

            Assert.Null(user);
        }

        public void Dispose()
        {
            _provider.Dispose();
        }
    }
}