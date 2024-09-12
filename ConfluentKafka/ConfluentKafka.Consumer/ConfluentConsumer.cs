using Confluent.Kafka;
using Rebus.Shared;

namespace ConfluentKafka.Consumer;

public class ConfluentConsumer
{
    private readonly string _bootstrapServers = "localhost:29091";
    private readonly string _groupId = "confluent-group-id";
    private readonly string _topic = "confluent-topic";

    private readonly MessageCollection _messageCollection;

    public ConfluentConsumer(MessageCollection messageCollection)
    {
        _messageCollection = messageCollection;
    }

    public void Consume(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            GroupId = _groupId,
            BootstrapServers = _bootstrapServers,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using (var consumer = new ConsumerBuilder<string, string>(config).Build())
        {
            consumer.Subscribe(_topic);

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var consumeResult = consumer.Consume(stoppingToken);

                    _messageCollection.Add(new RebusMessageWithId 
                    { 
                        Id = consumeResult.Message.Key, 
                        Message = consumeResult.Message.Value 
                    });

                    Console.WriteLine($"Received message: {consumeResult.Message.Value}");
                }
            }
            catch (ConsumeException e)
            {
                Console.WriteLine($"Error occurred: {e.Error.Reason}");
            }
            finally
            {
                consumer.Close();
            }
        }
    }
}
