using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Mystik.Entities;
using Mystik.Helpers;
using Mystik.Models.User;
using Mystik.Services;

namespace Mystik.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : Controller
    {
        private IUserService _userService;

        protected override void Dispose(bool disposing)
        {
            _userService.Dispose();
            base.Dispose(disposing);
        }

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("removed/{id}")]
        public async Task<IActionResult> GetRemoved(Guid id, UserRelatedEntities model)
        {
            var currentUserId = Guid.Parse(User.Identity.Name);
            if (currentUserId == id)
            {
                var removedEntities = await _userService.GetNotExisting(id, model);
                return Ok(removedEntities);
            }
            return Forbid();
        }

        [Authorize(Roles = Role.Admin)]
        public async Task<IEnumerable<JsonRepresentableUser>> Get(Get model)
        {
            if (model.Since == null)
            {
                model.Since = DateTime.UnixEpoch;
            }

            var users = await _userService.GetAll();
            return await users.GetJsonRepresentableUsers(model.Since);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id, Get model)
        {
            if (model.Since == null)
            {
                model.Since = DateTime.UnixEpoch;
            }

            var currentUserId = Guid.Parse(User.Identity.Name);

            if (id != currentUserId && !User.IsInRole(Role.Admin))
                return Forbid();

            var user = await _userService.Retrieve(id);
            return Ok(await user.ToJsonRepresentableObject(model.Since));
        }

        [HttpGet("public/{id}")]
        public async Task<IActionResult> GetPublicData(Guid id)
        {
            var user = await _userService.Retrieve(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user.GetPublicData());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var currentUserId = Guid.Parse(User.Identity.Name);

            if (id != currentUserId && !User.IsInRole(Role.Admin))
                return Forbid();

            await _userService.Delete(id);
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate(Authentication model)
        {
            var user = await _userService.Authenticate(model.Username, model.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect." });
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(AppSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new
            {
                Id = user.Id,
                Token = tokenString
            });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(Registration model)
        {
            try
            {
                await _userService.Create(model.Nickname, model.Username, model.Password);
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
