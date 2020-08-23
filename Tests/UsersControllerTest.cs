using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mystik.Entities;
using Mystik.Helpers;
using Mystik.Models.User;
using Tests.Helpers;
using Xunit;

namespace Tests
{
    public class UsersControllerTest : UsersControllerProvider
    {
        [Fact]
        public async Task GetRemoved_AsAuthorizedUser_ReturnsCorrectEntities()
        {
            UsersController = UsersController.WithUserIdentity(Admin);

            AddFriend();
            AddInvitation();
            AddConversation();

            var usersIds = Context.Users.Select(u => u.Id).ToList();

            Context.Remove(Friends1);
            Context.Remove(Friends2);
            Context.Remove(Invitation);

            Context.SaveChanges();

            var userRelatedEntities = new UserRelatedEntities
            {
                FriendsIds = new List<Guid> { User2.Id },
                InvitedIds = new List<Guid> { User2.Id },
                InvitersIds = new List<Guid> { },
                ConversationIds = new List<Guid> { Conversation.Id },
                ConversationMembersIds = new List<Guid> { User2.Id },
                ConversationManagersIds = new List<Guid> { Admin.Id },
            };

            var expectedNotExistingUserRelatedEntities = new UserRelatedEntities
            {
                FriendsIds = new List<Guid> { User2.Id },
                InvitedIds = new List<Guid> { User2.Id },
                InvitersIds = new List<Guid> { },
                ConversationIds = new List<Guid> { },
                ConversationMembersIds = new List<Guid> { },
                ConversationManagersIds = new List<Guid> { },
            };

            var actualNotExistingUserRelatedEntitiesAction = await UsersController.GetRemoved(Admin.Id, userRelatedEntities);

            Assert.IsAssignableFrom<OkObjectResult>(actualNotExistingUserRelatedEntitiesAction);

            var actualNotExistingUserRelatedEntities = (actualNotExistingUserRelatedEntitiesAction
                                                        as OkObjectResult).Value;

            Assert.Equal(expectedNotExistingUserRelatedEntities, actualNotExistingUserRelatedEntities);
        }

        [Fact]
        public async Task GetRemove_AsUnauthorizedUser_Forbids()
        {
            UsersController = UsersController.WithUserIdentity(User1);

            var result = await UsersController.GetRemoved(Admin.Id, new UserRelatedEntities { });

            Assert.IsAssignableFrom<ForbidResult>(result);
        }

        [Fact]
        public async Task Get_ReturnsCorrectEntities()
        {
            UsersController = UsersController.WithUserIdentity(Admin);

            var model = new Get { };

            var jsonRepresentableUsers = await UsersController.Get(model);

            var expectedUsersIds = Context.Users.Select(u => u.Id).ToHashSet();
            var actualUsersIds = jsonRepresentableUsers.Select(jru => jru.Id);

            Assert.True(expectedUsersIds.SetEquals(actualUsersIds));
        }

        [Fact]
        public async Task GetById_AsAuthorizedUser_ReturnsCorrectEntity()
        {
            UsersController = UsersController.WithUserIdentity(User1);

            var model = new Get { };
            var jsonRepresentableUserResult = await UsersController.Get(User1.Id, model);

            Assert.IsAssignableFrom<OkObjectResult>(jsonRepresentableUserResult);

            var jsonRepresentableUser = (jsonRepresentableUserResult as OkObjectResult).Value as JsonRepresentableUser;

            Assert.Equal(User1.Id, jsonRepresentableUser.Id);
        }

        [Fact]
        public async Task GetById_AsUnauthorizedUser_Forbids()
        {
            UsersController = UsersController.WithUserIdentity(User1);

            var model = new Get { };
            var jsonRepresentableUserResult = await UsersController.Get(User2.Id, model);

            Assert.IsAssignableFrom<ForbidResult>(jsonRepresentableUserResult);
        }

        [Fact]
        public async Task GetPublicData_UserExists_ReturnsCorrectEntity()
        {
            UsersController = UsersController.WithUserIdentity(User1);

            var publicDataResult = await UsersController.GetPublicData(User2.Id);


            Assert.IsAssignableFrom<OkObjectResult>(publicDataResult);

            var publicData = (publicDataResult as OkObjectResult).Value as UserPublicData;
            Assert.Equal(User2.Id, publicData.Id);
        }

        [Fact]
        public async Task GetPublicData_UserDoesNotExist_ReturnsNotFound()
        {
            UsersController = UsersController.WithUserIdentity(User1);

            var publicDataResult = await UsersController.GetPublicData(NotExistingUser.Id);

            Assert.IsAssignableFrom<NotFoundResult>(publicDataResult);
        }

        [Fact]
        public async Task Delete_AsAuthorizedUser_ReturnsOk()
        {
            UsersController = UsersController.WithUserIdentity(User1);

            var result = await UsersController.Delete(User1.Id);

            Assert.IsAssignableFrom<OkResult>(result);
        }

        [Fact]
        public async Task Delete_AsUnauthorizedUser_Forbids()
        {
            UsersController = UsersController.WithUserIdentity(User1);

            var result = await UsersController.Delete(User2.Id);

            Assert.IsAssignableFrom<ForbidResult>(result);
        }

        [Fact]
        public async Task Delete_AsUnauthorizedUser_DoesNotDelete()
        {
            UsersController = UsersController.WithUserIdentity(User1);

            await UsersController.Delete(User2.Id);

            Assert.Equal(InitialNumberOfUsers, Context.Users.Count());
        }

        [Fact]
        public async Task Authenticate_WithCorrectCredentials_ReturnsCorrectEntity()
        {
            AppSettings.Secret = "12345678901234567890098765432";

            var model = new Authentication
            {
                Username = User1.Username,
                Password = User1.Password,
            };

            var result = await UsersController.Authenticate(model);

            Assert.IsAssignableFrom<OkObjectResult>(result);

            var resultValue = (result as OkObjectResult).Value;

            Assert.Equal(resultValue.GetProperty("Id"), User1.Id);
        }

        [Fact]
        public async Task Authenticate_WithIncorrectCredentials_ReturnsBadRequest()
        {
            AppSettings.Secret = "12345678901234567890098765432";

            var model = new Authentication
            {
                Username = User1.Username,
                Password = User2.Password,
            };

            var result = await UsersController.Authenticate(model);

            Assert.IsAssignableFrom<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Register_WithValidCredentials_ReturnsOk()
        {
            var model = new Registration
            {
                Nickname = NotExistingUser.Nickname,
                Username = NotExistingUser.Username,
                Password = NotExistingUser.Password,
            };

            var result = await UsersController.Register(model);

            Assert.IsAssignableFrom<OkResult>(result);
        }

        [Fact]
        public async Task Register_WithInvalidCredentials_ReturnsBadRequest()
        {
            var model = new Registration
            {
                Nickname = NotExistingUser.Nickname,
                Username = NotExistingUser.Username,
                Password = "",
            };

            var result = await UsersController.Register(model);

            Assert.IsAssignableFrom<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Search_WithNicknameFragment_ReturnsCorrectEntities()
        {
            var actualUsersPublicData = await UsersController.Search("er");

            var expectedUsersIds = new HashSet<Guid>
            {
                User1.Id,
                User2.Id,
            };

            var actualUsersIds = actualUsersPublicData.Select(upd => upd.Id);

            Assert.True(expectedUsersIds.SetEquals(actualUsersIds));
        }

        [Fact]
        public async Task Search_WithGuid_ReturnsCorrectEntity()
        {
            var query = User1.Id.ToString();
            var actualUsersPublicData = await UsersController.Search(query);

            var expectedUsersIds = new HashSet<Guid>
            {
                User1.Id,
            };

            var actualUsersIds = actualUsersPublicData.Select(upd => upd.Id);

            Assert.True(expectedUsersIds.SetEquals(actualUsersIds));
        }
    }
}