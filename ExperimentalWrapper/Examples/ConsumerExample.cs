namespace ExperimentalWrapper.Examples;

public class ConsumerExample
{
    public static void RunConsume(CancellationToken cancellationToken)
    {
        using var consumer = new ConsumerBuilder<Ignore, string>(Constants.ConsumerConfig)
            .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
            .BuildWithInterceptor();
        consumer.Subscribe(new List<string> { Constants.Topic });

        try
        {
            while (true)
            {
                try
                {
                    var consumeResult = consumer.Consume(cancellationToken);

                    if (consumeResult.IsPartitionEOF)
                    {
                        Console.WriteLine(
                            $"Reached end of topic {consumeResult.Topic}, partition {consumeResult.Partition}, offset {consumeResult.Offset}.");

                        continue;
                    }

                    Console.WriteLine($"Received message at {consumeResult.TopicPartitionOffset}: {consumeResult.Message.Value}");
                    try
                    {
                        consumer.StoreOffset(consumeResult);
                    }
                    catch (KafkaException e)
                    {
                        Console.WriteLine($"Store Offset error: {e.Error.Reason}");
                    }
                }
                catch (ConsumeException e)
                {
                    Console.WriteLine($"Consume error: {e.Error.Reason}");
                }
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Closing consumer.");
            consumer.Close();
        }
    }
}