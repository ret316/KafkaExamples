using ConfluentKafka.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ConfluentKafka.Producer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ValuesController : ControllerBase
{
    private ILogger<ValuesController> _logger;

    public ValuesController(ILogger<ValuesController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult> Get()
    {
        var producer = new ConfluentProducer();
        var message = new ConfluentMessage { Message = Guid.NewGuid().ToString() };
        await producer.ProduceAsync(message.Message);
        _logger.LogInformation(message.Message);
        return Ok();
    }
}
