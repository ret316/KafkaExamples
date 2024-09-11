using Rebus.Handlers;
using Rebus.Shared;

namespace Rebus.Consumer;

public class RebusConsumer : IHandleMessages<RebusMessage>
{
    private readonly ILogger<RebusConsumer> _logger;

    public RebusConsumer(ILogger<RebusConsumer> logger)
    {
        _logger = logger;
    }

    public Task Handle(RebusMessage message)
    {
        _logger.LogInformation(message.Message);

        return Task.CompletedTask;
    }
}
