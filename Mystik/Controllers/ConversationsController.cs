using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystik.Entities;
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
        public async Task<IEnumerable<JsonRepresentableConversation>> Get()
        {
            var conversations = await _conversationService.GetAll();
            var result = new List<JsonRepresentableConversation>();

            foreach (var conversation in conversations)
            {
                result.Add(await conversation.ToJsonRepresentableObject());
            }

            return result;
        }
    }
}