using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mystik.Helpers;
using Mystik.Models;
using Mystik.Services;

namespace Mystik.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : Controller
    {
        private IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Register(Registration model)
        {
            try
            {
                await _userService.Create(model.Username, model.Password);
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
