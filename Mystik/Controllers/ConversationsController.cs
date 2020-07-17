using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystik.Entities;
using Mystik.Models.Conversation;
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

        [Authorize(Roles = Role.Admin)]
        [HttpGet]
        public async Task<IEnumerable<object>> Get()
        {
            var conversations = await _conversationService.GetAll();
            var result = new List<object>();

            foreach (var conversation in conversations)
            {
                result.Add(await conversation.ToJsonRepresentableObject());
            }

            return result;
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

            return Ok(await conversation.ToJsonRepresentableObject());
        }
    }
}