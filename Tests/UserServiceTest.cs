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
            MockUserService.ReloadUsers();
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
        public async Task Create_AddsExactlyOneEntity()
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
        public async Task Delete_RemovesExactlyOneEntity()
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

        [Fact]
        public async Task DeleteFriends_RemovesCorrectEntities()
        {
            _provider.AddFriend();
            var userId = MockUserService.Admin.Id;
            var friendsIds = new List<Guid> { MockUserService.User2.Id };

            await _provider.UserService.DeleteFriends(userId, friendsIds);

            Assert.False(_provider.Context.Friends.Any(cof => cof.Friend1Id == MockUserService.Admin.Id
                                                             && cof.Friend2Id == MockUserService.User2.Id));
            Assert.False(_provider.Context.Friends.Any(cof => cof.Friend2Id == MockUserService.Admin.Id
                                                             && cof.Friend1Id == MockUserService.User2.Id));
        }

        [Fact]
        public async Task InviteFriends_AddsExactlyOneEntity()
        {
            var inviterId = MockUserService.Admin.Id;
            var invitedIds = new List<Guid> { MockUserService.User2.Id };

            await _provider.UserService.InviteFriends(inviterId, invitedIds);

            Assert.Equal(_provider.InitialNumberOfInvitations + 1, _provider.Context.Invitations.Count());
        }

        [Fact]
        public async Task InviteFriends_FriendAlreadyAdded_DoesNotAddAnyEntity()
        {
            _provider.AddFriend();
            var inviterId = MockUserService.Admin.Id;
            var invitedIds = new List<Guid> { MockUserService.User2.Id };

            await _provider.UserService.InviteFriends(inviterId, invitedIds);

            Assert.Equal(_provider.InitialNumberOfInvitations, _provider.Context.Invitations.Count());
        }

        [Fact]
        public async Task InviteFriends_AddsCorrectEntity()
        {
            var inviterId = MockUserService.Admin.Id;
            var invitedIds = new List<Guid> { MockUserService.User2.Id };

            await _provider.UserService.InviteFriends(inviterId, invitedIds);

            Assert.True(_provider.Context.Invitations.Any(i => i.InviterId == inviterId && i.InvitedId == MockUserService.User2.Id));
        }

        [Fact]
        public async Task GetNotExisting_ReturnsCorrectIds()
        {
            _provider.AddFriend();
            _provider.AddInvitation();
            _provider.AddConversation();

            var userRelatedEntities = new UserRelatedEntities
            {
                FriendsIds = new List<Guid> { MockUserService.Admin.Id, MockUserService.User1.Id },
                InvitedIds = new List<Guid> { MockUserService.Admin.Id, MockUserService.User1.Id },
                InvitersIds = new List<Guid> { MockUserService.Admin.Id, MockUserService.User1.Id },
                ConversationIds = new List<Guid> { _provider.ConversationId, MockUserService.User2.Id },
                ConversationMembersIds = new List<Guid> { MockUserService.Admin.Id,
                                                          MockUserService.User2.Id,
                                                          MockUserService.User1.Id },
                ConversationManagersIds = new List<Guid> { MockUserService.Admin.Id,
                                                           MockUserService.User2.Id,
                                                           MockUserService.User1.Id }
            };

            var expectedNotExistingUserRelatedEntities = new UserRelatedEntities
            {
                FriendsIds = new List<Guid> { MockUserService.User1.Id },
                InvitedIds = new List<Guid> { MockUserService.Admin.Id, MockUserService.User1.Id },
                InvitersIds = new List<Guid> { MockUserService.User1.Id },
                ConversationIds = new List<Guid> { MockUserService.User2.Id },
                ConversationMembersIds = new List<Guid> { MockUserService.User1.Id },
                ConversationManagersIds = new List<Guid> { MockUserService.User2.Id,
                                                           MockUserService.User1.Id }
            };

            var actualNotExistingUserRelatedEntities = await _provider.UserService.GetNotExisting(MockUserService.User2.Id,
                                                                                            userRelatedEntities);

            Assert.Equal(expectedNotExistingUserRelatedEntities, actualNotExistingUserRelatedEntities);
        }

        [Fact]
        public async Task DeleteInvitations_RemovesExactlyOneEntity()
        {
            _provider.AddInvitation();
            var inviterId = MockUserService.Admin.Id;
            var invitedIds = new List<Guid> { MockUserService.User2.Id };

            await _provider.UserService.DeleteInvitations(inviterId, invitedIds);

            Assert.Equal(_provider.InitialNumberOfInvitations - 1, _provider.Context.Invitations.Count());
        }

        [Fact]
        public async Task DeleteInvitations_RemovesCorrectEntity()
        {
            _provider.AddInvitation();
            var inviterId = MockUserService.Admin.Id;
            var invitedIds = new List<Guid> { MockUserService.User2.Id };

            await _provider.UserService.DeleteInvitations(inviterId, invitedIds);

            Assert.False(_provider.Context.Invitations.Any(cof => cof.InviterId == inviterId
                                                             && cof.InvitedId == MockUserService.User2.Id));
        }

        [Fact]
        public async Task IsUserInvited_UserIsInvited_ReturnsTrue()
        {
            _provider.AddInvitation();

            var userIsInvited = await _provider.UserService.IsUserInvited(MockUserService.Admin.Id, MockUserService.User2.Id);

            Assert.True(userIsInvited);
        }

        [Fact]
        public async Task IsUserInvited_UserIsNotInvited_ReturnsFalse()
        {
            var userIsInvited = await _provider.UserService.IsUserInvited(MockUserService.Admin.Id, MockUserService.User2.Id);

            Assert.False(userIsInvited);
        }

        [Fact]
        public async Task GetAll_ReturnsAllUsers()
        {
            var expectedUsers = _provider.Context.Users.ToHashSet();
            var actualUsers = await _provider.UserService.GetAll();

            Assert.True(expectedUsers.SetEquals(actualUsers));
        }

        public void Dispose()
        {
            _provider.Dispose();
        }
    }
}