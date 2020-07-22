using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystik.Helpers;
using Mystik.Models.Message;
using Mystik.Services;

namespace Mystik.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class MessagesController : Controller
    {
        private IMessageService _messageService;

        protected override void Dispose(bool disposing)
        {
            _messageService.Dispose();
            base.Dispose(disposing);
        }

        public MessagesController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var message = await _messageService.Retrieve(id);

            if (message == null)
            {
                return NotFound();
            }

            var currentUserId = Guid.Parse(User.Identity.Name);

            if (await _messageService.IsTheConversationMember(message.ConversationId, currentUserId))
            {
                return Ok(await message.ToJsonRepresentableObject());
            }

            return Forbid();
        }

        [HttpGet("conversations/{conversationId}")]
        public async Task<object> Get(Guid conversationId, Get model)
        {
            var messages = _messageService.GetMessagesFromConversation(conversationId);

            if (model.Since != null)
            {
                messages = messages.Where(m => m.CreatedDate > model.Since);
            }

            return new
            {
                Messages = await messages.GetEncryptedContent(),
            };
        }
    }
}