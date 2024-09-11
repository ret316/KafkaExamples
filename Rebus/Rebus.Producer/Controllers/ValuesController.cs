using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rebus.Bus;
using Rebus.Shared;

namespace Rebus.Producer.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ValuesController : ControllerBase
{
    private readonly ILogger<ValuesController> _logger;
    private readonly IBus _bus;

    public ValuesController(ILogger<ValuesController> logger, IBus bus)
    {
        _logger = logger;
        _bus = bus;
    }

    [HttpGet]
    public async Task<ActionResult> Get()
    {
        var message = new RebusMessage { Message = Guid.NewGuid().ToString() };
        _logger.LogInformation(message.Message);
        await _bus.Send(message);

        return Ok();
    }
}
