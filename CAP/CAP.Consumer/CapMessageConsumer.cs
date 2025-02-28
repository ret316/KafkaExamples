using CAP.Shared;
using DotNetCore.CAP;

namespace CAP.Consumer;

public class CapMessageConsumer : ICapSubscribe
{
    private readonly ILogger<CapMessageConsumer> _logger;
    private readonly CapCollection _collection;

    public CapMessageConsumer(ILogger<CapMessageConsumer> logger, CapCollection collection)
    {
        _logger = logger;
        _collection = collection;
    }

    [CapSubscribe("cap-topic")]
    public Task Consume(CapMessage message, [FromCap] CapHeader headers, CancellationToken cancellationToken)
    {
        _collection.Add(new CapMessageWithId 
        { 
            Id = headers.GetValueOrDefault("key")!,
            Message = message.Message
        });

        _logger.LogInformation(message.Message);

        return Task.CompletedTask;
    }
}
