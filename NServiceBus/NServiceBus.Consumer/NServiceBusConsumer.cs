using NServiceBus.Shared;

namespace NServiceBus.Consumer;

public class NServiceBusConsumer : IHandleMessages<NServiceBusMessage>
{
    private readonly ILogger<NServiceBusConsumer> _logger;

    public NServiceBusConsumer(ILogger<NServiceBusConsumer> logger)
    {
        _logger = logger;
    }

    public Task Handle(NServiceBusMessage message, IMessageHandlerContext context)
    {
        _logger.LogInformation(message.Message);

        return Task.CompletedTask;
    }
}
