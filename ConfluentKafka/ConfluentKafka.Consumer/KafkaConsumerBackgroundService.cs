namespace ConfluentKafka.Consumer;

public class KafkaConsumerBackgroundService : BackgroundService
{
    private readonly ConfluentConsumer _consumer;

    public KafkaConsumerBackgroundService(ConfluentConsumer consumer)
    {
        _consumer = consumer;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() => _consumer.Consume(stoppingToken), stoppingToken);
    }
}
