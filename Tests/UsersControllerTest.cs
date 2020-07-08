using System;
using System.Linq;
using System.Threading.Tasks;
using Mystik.Controllers;
using Mystik.Entities;
using Tests.Helpers;
using Xunit;

namespace Tests
{
    public class UsersControllerTest
    {
        [Fact]
        public async Task Get_ReturnsAllUsers()
        {
            var service = new MockUserService();
            var controller = new UsersController(service);
            var result = await controller.Get();
            Assert.True(service.Users.SetEquals(result.ToHashSet()));
        }
    }
}
