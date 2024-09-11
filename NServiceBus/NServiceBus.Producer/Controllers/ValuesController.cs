using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NServiceBus.Shared;

namespace NServiceBus.Producer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ValuesController : ControllerBase
{
    private readonly ILogger<ValuesController> _logger;
    private readonly IEndpointInstance _endpointInstance;

    public ValuesController(ILogger<ValuesController> logger, IEndpointInstance endpointInstance)
    {
        _logger = logger;
        _endpointInstance = endpointInstance;
    }

    [HttpGet]
    public async Task<ActionResult> Get()
    {
        var message = new NServiceBusMessage { Message = Guid.NewGuid().ToString() };
        await _endpointInstance.Send(message);
        _logger.LogInformation(message.Message);
        return Ok();
    }
}
