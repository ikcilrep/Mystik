using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    }
}