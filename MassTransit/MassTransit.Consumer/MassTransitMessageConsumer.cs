using MassTransit.Shared;

namespace MassTransit.Consumer;

public class MassTransitMessageConsumer : IConsumer<MassTransitMessage>
{
    private readonly ILogger<MassTransitMessageConsumer> _logger;
    private readonly MessageCollection _collection;

    public MassTransitMessageConsumer(ILogger<MassTransitMessageConsumer> logger, MessageCollection collection)
    {
        _logger = logger;
        _collection = collection;
    }

    public Task Consume(ConsumeContext<MassTransitMessage> context)
    {
        _collection.Add(new MassTransitMessageWithId 
        { 
            Id = context.MessageId.ToString()!,
            Message = context.Message.Message
        });

        _logger.LogInformation(context.Message.Message);

        return Task.CompletedTask;
    }
}
