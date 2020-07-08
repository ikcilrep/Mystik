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

        [Fact]
        public async Task GetById_AsAdmin_ReturnsCorrectUser()
        {
            var service = new MockUserService();
            var adminId = "6c554aa4-3fd8-48d4-a0d8-13164f172d0c";
            var controller = new UsersController(service).WithIdentity(
                adminId,
                Role.Admin);

            var userId = Guid.Parse("4192105b-3256-40e2-9efb-eef265e5eaa6");

            var expectedUser = await service.Retrieve(userId);
            var response = await controller.Get(userId);

            Assert.IsType<OkObjectResult>(response);
            var ok = response as OkObjectResult;
            Assert.Equal(expectedUser, ok.Value);
        }

        [Fact]
        public async Task GetById_AsUnauthorizedUser_ForbidsAccess()
        {
            var service = new MockUserService();
            var unauthorizedId = "60398e2a-4439-46bf-9101-e26ea63d5326";
            var controller = new UsersController(service).WithIdentity(
                unauthorizedId,
                Role.User);

            var userId = Guid.Parse("4192105b-3256-40e2-9efb-eef265e5eaa6");

            var expectedUser = await service.Retrieve(userId);
            var response = await controller.Get(userId);

            Assert.IsType<ForbidResult>(response);
        }
    }
}
