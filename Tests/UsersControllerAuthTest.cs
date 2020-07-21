using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mystik.Controllers;
using Mystik.Helpers;
using Mystik.Models.User;
using Tests.Helpers;
using Xunit;

namespace Tests
{
    public class UsersControllerAuthTest
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
        public async Task Authenticate_WithCorrectCredentials_ReturnsCorrectData()
        {
            AppSettings.Secret = "123456789012345678900987654321";

            var service = new MockUserService();
            var controller = new UsersController(service);

            var model = new Authentication
            {
                Username = MockUserService.User1.Username,
                Password = MockUserService.User1.Password
            };

            var response = await controller.Authenticate(model);
            Assert.IsType<OkObjectResult>(response);
            var ok = response as OkObjectResult;
            Assert.Equal(MockUserService.User1.Username, ok.Value.GetProperty("Username"));
            Assert.Equal(MockUserService.User1.Nickname, ok.Value.GetProperty("Nickname"));
            Assert.Equal(MockUserService.User1.Id, ok.Value.GetProperty("Id"));
        }

        [Fact]
        public async Task Authenticate_WithIncorrectCredentials_ReturnsBadRequest()
        {
            AppSettings.Secret = "123456789012345678900987654321";

            var service = new MockUserService();
            var controller = new UsersController(service);

            var model = new Authentication
            {
                Username = MockUserService.User1.Username,
                Password = MockUserService.User2.Password
            };

            var response = await controller.Authenticate(model);
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task Register_WithValidCredentials_ReturnsOk()
        {
            var service = new MockUserService();
            var controller = new UsersController(service);

            var model = new Registration
            {
                Nickname = MockUserService.NotExistingUser.Nickname,
                Username = MockUserService.NotExistingUser.Username,
                Password = MockUserService.NotExistingUser.Password
            };

            var response = await controller.Register(model);
            Assert.IsType<OkResult>(response);
        }
    }
}
