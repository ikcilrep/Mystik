using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Mystik.Services;

namespace Mystik.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private IMessageService _messageService;

        public ChatHub(IMessageService messageService)
        {
            _messageService = messageService;
        }
    }
}