using Rebus.Handlers;
using Rebus.Shared;

namespace Rebus.Consumer;

public class RebusConsumer : IHandleMessages<RebusMessage>
{
    private readonly ILogger<RebusConsumer> _logger;
    private readonly MessageCollection _messageCollection;

    public RebusConsumer(ILogger<RebusConsumer> logger, MessageCollection messageCollection)
    {
        _logger = logger;
        _messageCollection = messageCollection;
    }

    public Task Handle(RebusMessage message)
    {
        _logger.LogInformation(message.Message);

        _messageCollection.Add(new RebusMessageWithId
        {
            Id = "",
            Message = message.Message
        });

        return Task.CompletedTask;
    }
}
