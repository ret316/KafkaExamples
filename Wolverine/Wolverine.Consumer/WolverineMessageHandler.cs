using Confluent.Kafka;
using Wolverine.Shared;

namespace Wolverine.Consumer;

public class WolverineMessageHandler(ILogger<WolverineMessageHandler> logger)
{
    public void Before(Envelope envelope)
    {

    }

    public async Task HandleAsync(WolverineMessage message, Envelope envelope, CancellationToken token)
    {
        await Task.Delay(1000, token);

        logger.LogInformation(message.ToString());

    }
}
