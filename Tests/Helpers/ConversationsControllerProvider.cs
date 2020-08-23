using Mystik.Controllers;

namespace Tests.Helpers
{
    public class ConversationsControllerProvider : ConversationServiceProvider
    {
        protected ConversationsController ConversationsController { get; set; }

        protected ConversationsControllerProvider() : base()
        {
            ConversationsController = new ConversationsController(ConversationService);
        }
    }
}