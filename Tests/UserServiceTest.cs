using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mystik.Entities;
using Tests.Helpers;
using Xunit;

namespace Tests
{
    public class UserServiceTest : UserServiceProvider
    {
        [Fact]
        public async Task Create_ReturnsCorrectUser()
        {
            var expectedUser = MockUserService.User1;

            var actualUser = await UserService.Create(
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
            var user = await UserService.Create(
                MockUserService.User1.Nickname,
                MockUserService.User1.Username,
                MockUserService.User1.Password);

            Assert.True(user.Id != null && user.Id != Guid.Empty);
        }

        [Fact]
        public async Task Create_AddsExactlyOneEntity()
        {
            var user = await UserService.Create(
                MockUserService.User1.Nickname,
                MockUserService.User1.Username,
                MockUserService.User1.Password);

            Assert.Equal(InitialNumberOfUsers + 1, Context.Users.Count());
        }

        [Fact]
        public async Task Retrieve_ReturnsCorrectUser()
        {
            var actualUser = await UserService.Retrieve(MockUserService.User2.Id);

            Assert.Equal(MockUserService.User2, actualUser);
        }

        [Fact]
        public async Task Update_ChangesValues()
        {
            await UserService.Update(
                MockUserService.User2.Id,
                MockUserService.NotExistingUser.Nickname,
                null);

            var actualUser = Context.Find<User>(MockUserService.User2.Id);

            Assert.Equal(MockUserService.NotExistingUser.Nickname, actualUser.Nickname);
        }

        [Fact]
        public async Task Delete_RemovesExactlyOneEntity()
        {
            var id = MockUserService.User2.Id;
            await UserService.Delete(id);

            Assert.Equal(InitialNumberOfUsers - 1, Context.Users.Count());
        }

        [Fact]
        public async Task Authenticate_WithValidCredentials_ReturnsCorrectUser()
        {
            var expectedUser = MockUserService.User2;
            var actualUser = await UserService.Authenticate(
                expectedUser.Username,
                expectedUser.Password);

            Assert.Equal(expectedUser, actualUser);
        }

        [Fact]
        public async Task Authenticate_WithInvalidCredentials_ReturnsNull()
        {
            var user = await UserService.Authenticate(
                MockUserService.User2.Username,
                MockUserService.NotExistingUser.Password);

            Assert.Null(user);
        }

        [Fact]
        public async Task AddFriend_AddsExactlyTwoEntities()
        {
            await UserService.AddFriend(MockUserService.Admin.Id, MockUserService.User2.Id);

            Assert.Equal(InitialNumberOfFriends + 2, Context.Friends.Count());
        }

        [Fact]
        public async Task AddFriend_AddsCorrectEntities()
        {
            await UserService.AddFriend(MockUserService.Admin.Id, MockUserService.User2.Id);

            Assert.True(Context.Friends.Any(cof => cof.Friend1Id == MockUserService.Admin.Id
                                                             && cof.Friend2Id == MockUserService.User2.Id));
            Assert.True(Context.Friends.Any(cof => cof.Friend2Id == MockUserService.Admin.Id
                                                             && cof.Friend1Id == MockUserService.User2.Id));
        }

        [Fact]
        public async Task DeleteFriends_RemovesExactlyTwoEntities()
        {
            AddFriend();
            var userId = MockUserService.Admin.Id;
            var friendsIds = new List<Guid> { MockUserService.User2.Id };

            await UserService.DeleteFriends(userId, friendsIds);

            Assert.Equal(InitialNumberOfFriends - 2, Context.Friends.Count());
        }

        [Fact]
        public async Task DeleteFriends_RemovesCorrectEntities()
        {
            AddFriend();
            var userId = MockUserService.Admin.Id;
            var friendsIds = new List<Guid> { MockUserService.User2.Id };

            await UserService.DeleteFriends(userId, friendsIds);

            Assert.False(Context.Friends.Any(cof => cof.Friend1Id == MockUserService.Admin.Id
                                                             && cof.Friend2Id == MockUserService.User2.Id));
            Assert.False(Context.Friends.Any(cof => cof.Friend2Id == MockUserService.Admin.Id
                                                             && cof.Friend1Id == MockUserService.User2.Id));
        }

        [Fact]
        public async Task InviteFriends_AddsExactlyOneEntity()
        {
            var inviterId = MockUserService.Admin.Id;
            var invitedIds = new List<Guid> { MockUserService.User2.Id };

            await UserService.InviteFriends(inviterId, invitedIds);

            Assert.Equal(InitialNumberOfInvitations + 1, Context.Invitations.Count());
        }

        [Fact]
        public async Task InviteFriends_FriendAlreadyAdded_DoesNotAddAnyEntity()
        {
            AddFriend();
            var inviterId = MockUserService.Admin.Id;
            var invitedIds = new List<Guid> { MockUserService.User2.Id };

            await UserService.InviteFriends(inviterId, invitedIds);

            Assert.Equal(InitialNumberOfInvitations, Context.Invitations.Count());
        }

        [Fact]
        public async Task InviteFriends_AddsCorrectEntity()
        {
            var inviterId = MockUserService.Admin.Id;
            var invitedIds = new List<Guid> { MockUserService.User2.Id };

            await UserService.InviteFriends(inviterId, invitedIds);

            Assert.True(Context.Invitations.Any(i => i.InviterId == inviterId && i.InvitedId == MockUserService.User2.Id));
        }

        [Fact]
        public async Task GetNotExisting_ReturnsCorrectIds()
        {
            AddFriend();
            AddInvitation();
            AddConversation();

            var userRelatedEntities = new UserRelatedEntities
            {
                FriendsIds = new List<Guid> { MockUserService.Admin.Id, MockUserService.User1.Id },
                InvitedIds = new List<Guid> { MockUserService.Admin.Id, MockUserService.User1.Id },
                InvitersIds = new List<Guid> { MockUserService.Admin.Id, MockUserService.User1.Id },
                ConversationIds = new List<Guid> { ConversationId, MockUserService.User2.Id },
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

            var actualNotExistingUserRelatedEntities = await UserService.GetNotExisting(MockUserService.User2.Id,
                                                                                            userRelatedEntities);

            Assert.Equal(expectedNotExistingUserRelatedEntities, actualNotExistingUserRelatedEntities);
        }

        [Fact]
        public async Task DeleteInvitations_RemovesExactlyOneEntity()
        {
            AddInvitation();
            var inviterId = MockUserService.Admin.Id;
            var invitedIds = new List<Guid> { MockUserService.User2.Id };

            await UserService.DeleteInvitations(inviterId, invitedIds);

            Assert.Equal(InitialNumberOfInvitations - 1, Context.Invitations.Count());
        }

        [Fact]
        public async Task DeleteInvitations_RemovesCorrectEntity()
        {
            AddInvitation();
            var inviterId = MockUserService.Admin.Id;
            var invitedIds = new List<Guid> { MockUserService.User2.Id };

            await UserService.DeleteInvitations(inviterId, invitedIds);

            Assert.False(Context.Invitations.Any(cof => cof.InviterId == inviterId
                                                             && cof.InvitedId == MockUserService.User2.Id));
        }

        [Fact]
        public async Task IsUserInvited_UserIsInvited_ReturnsTrue()
        {
            AddInvitation();

            var userIsInvited = await UserService.IsUserInvited(MockUserService.Admin.Id, MockUserService.User2.Id);

            Assert.True(userIsInvited);
        }

        [Fact]
        public async Task IsUserInvited_UserIsNotInvited_ReturnsFalse()
        {
            var userIsInvited = await UserService.IsUserInvited(MockUserService.Admin.Id, MockUserService.User2.Id);

            Assert.False(userIsInvited);
        }

        [Fact]
        public async Task GetAll_ReturnsAllUsers()
        {
            var expectedUsers = Context.Users.ToHashSet();
            var actualUsers = await UserService.GetAll();

            Assert.True(expectedUsers.SetEquals(actualUsers));
        }
    }
}