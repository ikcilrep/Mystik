using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mystik.Entities;
using Tests.Helpers;
using Xunit;

namespace Tests
{
    public class UsersControllerTest : UsersControllerProvider
    {
        [Fact]
        public async Task GetRemoved_AsAuthorizedUser_ReturnsCorrectEntities()
        {
            UsersController = UsersController.WithAdminIdentity();

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
                FriendsIds = new List<Guid> { MockUserService.User2.Id },
                InvitedIds = new List<Guid> { MockUserService.User2.Id },
                InvitersIds = new List<Guid> { },
                ConversationIds = new List<Guid> { Conversation.Id },
                ConversationMembersIds = new List<Guid> { MockUserService.User2.Id },
                ConversationManagersIds = new List<Guid> { MockUserService.Admin.Id },
            };

            var expectedNotExistingUserRelatedEntities = new UserRelatedEntities
            {
                FriendsIds = new List<Guid> { MockUserService.User2.Id },
                InvitedIds = new List<Guid> { MockUserService.User2.Id },
                InvitersIds = new List<Guid> { },
                ConversationIds = new List<Guid> { },
                ConversationMembersIds = new List<Guid> { },
                ConversationManagersIds = new List<Guid> { },
            };

            var actualNotExistingUserRelatedEntitiesAction = await UsersController.GetRemoved(MockUserService.Admin.Id, userRelatedEntities);

            Assert.IsAssignableFrom<OkObjectResult>(actualNotExistingUserRelatedEntitiesAction);

            var actualNotExistingUserRelatedEntities = (actualNotExistingUserRelatedEntitiesAction
                                                        as OkObjectResult).Value;

            Assert.Equal(expectedNotExistingUserRelatedEntities, actualNotExistingUserRelatedEntities);
        }
    }
}