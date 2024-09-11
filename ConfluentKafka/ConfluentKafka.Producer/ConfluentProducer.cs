using Confluent.Kafka;

namespace ConfluentKafka.Producer;

public class ConfluentProducer
{
    private readonly string _bootstrapServers = "localhost:9092";
    private readonly string _topic = "confluent-topic";

    public async Task ProduceAsync(string message)
    {
        var config = new ProducerConfig { BootstrapServers = _bootstrapServers };

        using (var producer = new ProducerBuilder<Null, string>(config).Build())
        {
            try
            {
                var deliveryResult = await producer.ProduceAsync(_topic, new Message<Null, string> { Value = message });
                Console.WriteLine($"Message '{message}' delivered to '{deliveryResult.TopicPartitionOffset}'");
            }
            catch (ProduceException<Null, string> e)
            {
                Console.WriteLine($"Error occurred: {e.Error.Reason}");
            }
        }
    }
}
