
using ExperimentalWrapper.Extensions;

namespace ExperimentalWrapper.Examples;

public class ProducerExample
{
  public static void RunProduce(CancellationToken cancellationToken = default)
  {
    using var producer = new ProducerBuilder<string, string>(Constants.ProducerConfig)
        .BuildWithInterceptor();

    producer.Produce(Constants.Topic, new Message<string, string> { Value = "test" }, HandleDeliveryReport);
    producer.Flush(TimeSpan.FromSeconds(10));
  }


  public static async Task RunProduceAsync(CancellationToken cancellationToken = default)
  {
    using var producer = new ProducerBuilder<string, string>(Constants.ProducerConfig)
        .BuildWithInterceptor();
    var dr = await producer.ProduceAsync(Constants.Topic, new Message<string, string> { Value = "test" }, cancellationToken);
    Console.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
  }



  private static void HandleDeliveryReport(DeliveryReport<string, string> report)
  {
    Console.WriteLine($"Delivered '{report.Value}' to '{report.TopicPartitionOffset}'");
  }
}