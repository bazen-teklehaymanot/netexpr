namespace ExperimentalWrapper.Examples;

public static class Constants
{
    public static readonly string Topic = "test-topic";

    public static readonly ConsumerConfig ConsumerConfig = new()
    {
        BootstrapServers = "localhost:9092",
        GroupId = "test-consumer-group"
    };

    public static readonly ProducerConfig ProducerConfig = new()
    {
        BootstrapServers = "localhost:9092"
    };
}