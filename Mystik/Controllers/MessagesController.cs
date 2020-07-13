using System;
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

        [HttpPost("conversations/{id}")]
        public async Task<IActionResult> Post(Guid conversationId, MessagePost model)
        {
            var currentUserId = Guid.Parse(User.Identity.Name);
            if (await _messageService.IsTheConversationMember(conversationId, currentUserId))
            {
                var message = await _messageService.Create(model.EncryptedContent, currentUserId, conversationId);
                return Ok(new { Id = message.Id, SentTime = message.SentTime });
            }

            return Forbid();
        }
    }
}