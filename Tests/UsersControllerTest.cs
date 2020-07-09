using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mystik.Controllers;
using Mystik.Models;
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
            var controller = new UsersController(service).WithUser1Identity();

            var id = MockUserService.User1.Id;

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
            var controller = new UsersController(service).WithAdminIdentity();

            var id = MockUserService.User1.Id;

            var expectedUser = await service.Retrieve(id);
            var response = await controller.Get(id);

            Assert.IsType<OkObjectResult>(response);
            var ok = response as OkObjectResult;
            Assert.Equal(expectedUser, ok.Value);
        }

        [Fact]
        public async Task GetById_AsUnauthorizedUser_ForbidsAccess()
        {
            var service = new MockUserService();
            var controller = new UsersController(service).WithUser1Identity();

            var id = MockUserService.User2.Id;

            var response = await controller.Get(id);

            Assert.IsType<ForbidResult>(response);
        }

        [Fact]
        public async Task Delete_AsAuthorizedUser_ReturnsOk()
        {
            var service = new MockUserService();
            var controller = new UsersController(service).WithUser1Identity();

            var id = MockUserService.User1.Id;
            var response = await controller.Delete(id);

            Assert.IsType<OkResult>(response);
        }

        [Fact]
        public async Task Delete_AsAdmin_ReturnsOk()
        {
            var service = new MockUserService();
            var controller = new UsersController(service).WithAdminIdentity();

            var id = MockUserService.User1.Id;

            var response = await controller.Delete(id);

            Assert.IsType<OkResult>(response);
        }

        [Fact]
        public async Task Delete_AsUnauthorizedUser_ForbidsAccess()
        {
            var service = new MockUserService();
            var controller = new UsersController(service).WithUser1Identity();

            var id = MockUserService.User2.Id;

            var response = await controller.Delete(id);

            Assert.IsType<ForbidResult>(response);
        }

        [Fact]
        public async Task Patch_AsAuthorizedUser_ReturnsOk()
        {
            var service = new MockUserService();
            var controller = new UsersController(service).WithUser1Identity();

            var id = MockUserService.User1.Id;
            var model = new UserPatch { Nickname = MockUserService.User2.Nickname };
            var response = await controller.Patch(id, model);

            Assert.IsType<OkResult>(response);
        }

        [Fact]
        public async Task Patch_AsAdmin_ReturnsOk()
        {
            var service = new MockUserService();
            var controller = new UsersController(service).WithAdminIdentity();

            var id = MockUserService.User1.Id;
            var model = new UserPatch { Nickname = MockUserService.User2.Nickname };
            var response = await controller.Patch(id, model);

            Assert.IsType<OkResult>(response);
        }

        [Fact]
        public async Task Patch_AsUnauthorizedUser_ForbidsAccess()
        {
            var service = new MockUserService();
            var controller = new UsersController(service).WithUser1Identity();

            var id = MockUserService.User2.Id;
            var model = new UserPatch { Nickname = MockUserService.User1.Nickname };
            var response = await controller.Patch(id, model);

            Assert.IsType<ForbidResult>(response);
        }

    }
}
