using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

        [Fact]
        public async Task GetById_AsAuthorizedUser_ReturnsCorrectUser()
        {
            var service = new MockUserService();
            var id = Guid.Parse("4192105b-3256-40e2-9efb-eef265e5eaa6");
            var controller = new UsersController(service).WithIdentity(
                id.ToString(),
                Role.User);


            var expectedUser = await service.Retrieve(id);
            var response = await controller.Get(id);

            Assert.IsType<OkObjectResult>(response);
            var ok = response as OkObjectResult;
            Assert.Equal(expectedUser, ok.Value);
        }
    }
}
