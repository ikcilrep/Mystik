using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Mystik.Entities;

namespace Tests.Helpers
{
    public static class HubsTestExtension
    {
        private static T WithIdentity<T>(this T hub, string id, string role) where T : Hub
        {

            var principal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                            {
                                new Claim(ClaimTypes.Name, id),
                                new Claim(ClaimTypes.Role, role)
                            }, "TestAuthentication"));
            var mockContext = new Mock<HubCallerContext>();
            mockContext.Setup(hcc => hcc.User).Returns(principal);
            hub.Context = mockContext.Object;
            return hub;
        }
        public static T WithAdminIdentity<T>(this T hub) where T : Hub
        {
            return hub.WithIdentity(MockUserService.Admin.Id.ToString(), Role.Admin);
        }
    }
}