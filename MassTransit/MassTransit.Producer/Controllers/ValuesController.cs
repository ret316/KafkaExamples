using MassTransit.Shared;
using Microsoft.AspNetCore.Mvc;

namespace MassTransit.Producer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ValuesController : ControllerBase
{
    private readonly ILogger<ValuesController> _logger;
    private readonly ITopicProducer<string, MassTransitMessage> _producer;

    public ValuesController(ILogger<ValuesController> logger, ITopicProducer<string, MassTransitMessage> producer)
    {
        _logger = logger;
        _producer = producer;
    }

    [HttpGet]
    public async Task<ActionResult> Get()
    {
        var guid = Guid.NewGuid().ToString();
        var message = new MassTransitMessage
        {
            Message = Guid.NewGuid().ToString()
        };

        await _producer.Produce(guid, message);
        _logger.LogInformation(guid + " " + " " + message.Message);

        return Ok("Test");
    }
}
