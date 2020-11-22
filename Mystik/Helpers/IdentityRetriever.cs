using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;

namespace Mystik.Helpers
{
    public static class IdentityRetriever
    {
        public static Guid GetCurrentUserId(this HubCallerContext context)
        {
            return Guid.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }

        public static Guid GetCurrentUserId(this TokenValidatedContext context)
        {
            return Guid.Parse(context.Principal.FindFirst(ClaimTypes.NameIdentifier).Value);
        }

        public static Guid GetCurrentUserId(this ClaimsPrincipal user)
        {
            return Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier).Value);
        }
    }
}