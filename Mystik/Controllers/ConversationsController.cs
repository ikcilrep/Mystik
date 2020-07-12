using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public ConversationsController(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }


        [HttpPost]
        public async Task<IActionResult> Post(ConversationPost model)
        {
            var currentUserId = Guid.Parse(User.Identity.Name);
            var conversation = await _conversationService.Create(model.Name, currentUserId);

            if (conversation == null)
            {
                return BadRequest();
            }

            var usersIds = model.UsersIds.ToHashSet();
            usersIds.Add(currentUserId);

            await _conversationService.AddUsers(conversation.Id, usersIds);

            return Ok();
        }
    }
}