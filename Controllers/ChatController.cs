using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;

namespace lightsmeai.Controllers
{
    public class ChatRequest
{
    public string UserMessage { get; set; }
}

    [ApiController]
    [Route("apichat/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatClient _chatClient;
        private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;
        
        private readonly ChatOptions _chatOptions;
        public ChatController(
            IChatClient chatClient,
            IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
            ChatOptions chatOptions
            )
        {
            _chatClient = chatClient;
            _embeddingGenerator = embeddingGenerator;
            _chatOptions = chatOptions;
        }

        [HttpPost("chat")]
        public async Task<ActionResult<IEnumerable<string>>> Chat([FromBody] ChatRequest chatRequest)
        {
            var chatOptions = new ChatOptions
            {
                // Tools = new[]{}
            };

             var messages = new List<ChatMessage>
            {
               new(Microsoft.Extensions.AI.ChatRole.System, """
                You answer any question, Hey there, I'm Lumina, your friendly lighting assistant! 
                I can help you with all your lighting needs. 
                You can ask me to turn on the light, get the status of the light, 
                turn off all the lights, add a new light, or delete the light,get light.
                For update you should create an object like below.
                some time the user will pass all key values or one or two key value.
                { "id": 6, "name": "Chandelier", "Switched": false }
                Just let me know what you need and I'll do my best to help!
                """),
                new(Microsoft.Extensions.AI.ChatRole.User, chatRequest.UserMessage)
            };

            var response = await _chatClient.CompleteAsync(messages, _chatOptions);
            return Ok(response.Message.Text);
        }
    }
}
