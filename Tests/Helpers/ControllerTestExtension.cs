using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mystik.Entities;

namespace Tests.Helpers
{
    public static class ControllerTestExtensions
    {
        public static T WithUserIdentity<T>(this T controller, User user) where T : Controller
        {
            return controller.WithIdentity(user.Id.ToString(), user.Role);
        }
        private static T EnsureHttpContext<T>(this T controller) where T : Controller
        {
            if (controller.ControllerContext == null)
            {
                controller.ControllerContext = new ControllerContext();
            }

            if (controller.ControllerContext.HttpContext == null)
            {
                controller.ControllerContext.HttpContext = new DefaultHttpContext();
            }

            return controller;
        }
        private static T WithIdentity<T>(this T controller, string id, string role) where T : Controller
        {
            controller.EnsureHttpContext();

            var principal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                            {
                                new Claim(ClaimTypes.Name, id),
                                new Claim(ClaimTypes.Role, role)
                            }, "TestAuthentication"));

            controller.ControllerContext.HttpContext.User = principal;

            return controller;
        }

        /*  public static T WithAnonymousIdentity<T>(this T controller) where T : Controller
                {
                    controller.EnsureHttpContext();

                    var principal = new ClaimsPrincipal(new ClaimsIdentity());

                    controller.ControllerContext.HttpContext.User = principal;

                    return controller;
                }

         */

    }
}