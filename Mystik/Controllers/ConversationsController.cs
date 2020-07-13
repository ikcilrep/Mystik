using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystik.Entities;
using Mystik.Models;
using Mystik.Services;

namespace Mystik.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ConversationsController : Controller
    {
        private IConversationService _conversationService;

        protected override void Dispose(bool disposing)
        {
            _conversationService.Dispose();
            base.Dispose(disposing);
        }

        public ConversationsController(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        public async Task<IActionResult> Patch(Guid id, ConversationPatch model)
        {
            if (await _conversationService.Update(id, model))
            {
                return Ok();
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Post(ConversationPost model)
        {
            var currentUserId = Guid.Parse(User.Identity.Name);
            var conversation = await _conversationService.Create(model.Name, model.PasswordHashData, currentUserId);
            var usersIds = model.UsersIds.ToHashSet();

            usersIds.Add(currentUserId);

            await _conversationService.AddUsers(conversation.Id, usersIds);

            return Ok(new { Id = conversation.Id });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var currentUserId = Guid.Parse(User.Identity.Name);
            if (User.IsInRole(Role.Admin) || await _conversationService.IsTheConversationAdmin(id, currentUserId))
            {
                await _conversationService.Delete(id);
                return Ok();
            }
            return Forbid();
        }

        [Authorize(Roles = Role.Admin)]
        [HttpGet]
        public async Task<IEnumerable<object>> Get()
        {
            var conversations = await _conversationService.GetAll();
            return conversations.Select(c => c.ToJsonRepresentableObject());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var currentUserId = Guid.Parse(User.Identity.Name);
            var conversation = await _conversationService.Retrieve(id);

            if (conversation == null)
            {
                return NotFound();
            }
            else if (conversation.UserConversations.All(c => c.UserId != currentUserId))
            {
                return Forbid();
            }

            return Ok(conversation.ToJsonRepresentableObject());
        }
    }
}