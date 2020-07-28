using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mystik.Entities;
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

            Assert.Equal(_provider.InitialNumberOfUsers + 1, _provider.Context.Users.Count());
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
            await _provider.UserService.Update(
                MockUserService.User2.Id,
                MockUserService.NotExistingUser.Nickname,
                null);

            var actualUser = _provider.Context.Find<User>(MockUserService.User2.Id);

            Assert.Equal(MockUserService.NotExistingUser.Nickname, actualUser.Nickname);
        }

        [Fact]
        public async Task Delete_RemovesExactlyOneUserFromTheDatabase()
        {
            var id = MockUserService.User2.Id;
            await _provider.UserService.Delete(id);

            Assert.Equal(_provider.InitialNumberOfUsers - 1, _provider.Context.Users.Count());
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

        [Fact]
        public async Task AddFriend_AddsExactlyTwoEntities()
        {
            await _provider.UserService.AddFriend(MockUserService.Admin.Id, MockUserService.User2.Id);

            Assert.Equal(_provider.InitialNumberOfFriends + 2, _provider.Context.Friends.Count());
        }

        [Fact]
        public async Task AddFriend_AddsCorrectEntities()
        {
            await _provider.UserService.AddFriend(MockUserService.Admin.Id, MockUserService.User2.Id);

            Assert.True(_provider.Context.Friends.Any(cof => cof.Friend1Id == MockUserService.Admin.Id
                                                             && cof.Friend2Id == MockUserService.User2.Id));
            Assert.True(_provider.Context.Friends.Any(cof => cof.Friend2Id == MockUserService.Admin.Id
                                                             && cof.Friend1Id == MockUserService.User2.Id));
        }

        [Fact]
        public async Task DeleteFriends_RemovesExactlyTwoEntities()
        {
            _provider.AddFriend();
            var userId = MockUserService.Admin.Id;
            var friendsIds = new List<Guid> { MockUserService.User2.Id };

            await _provider.UserService.DeleteFriends(userId, friendsIds);

            Assert.Equal(_provider.InitialNumberOfFriends - 2, _provider.Context.Friends.Count());
        }

        public void Dispose()
        {
            _provider.Dispose();
        }
    }
}