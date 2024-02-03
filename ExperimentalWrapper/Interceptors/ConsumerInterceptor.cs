using System.Linq;

namespace ExperimentalWrapper.Interceptors;

public class ConsumerInterceptor<TKey, TValue> : DispatchProxy
{
#nullable disable
    public IConsumer<TKey, TValue> Target { get; set; }
    public Client Client { get; set; }
#nullable restore

    private readonly List<string> targetMethodNames =
    [
        "Consume",
        "ConsumeAsync"
    ];

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        if (!IsTargetMethod(targetMethod))
        {
            return targetMethod?.Invoke(Target, args);
        }
        BeforeConsume(args);
        var result = targetMethod?.Invoke(Target, args);
        AfterConsume(args, result as ConsumeResult<TKey, TValue>);
        return result;
    }

    private void BeforeConsume(object?[]? args)
    {
        Console.WriteLine("============== [BeforeConsume] =================== ");
        Console.WriteLine($"Arguments: {string.Join(", ", args)}");
        Console.WriteLine("================================================== \n");
    }

    private void AfterConsume(object?[]? args, ConsumeResult<TKey, TValue> result)
    {
        if (result.Message is { Headers: { } })
        {
            for (int i = 0; i < result.Message.Headers.Count; i++)
            {
                var header = result.Message.Headers[i];
                if (header.Key != "memphis_schema")
                    continue;
                var schemaExists = Client.ConsumerProtoDescriptors.ContainsKey((int)ToUInt64BigEndian(header.GetValueBytes()));
                if (!schemaExists)
                    Client.SentGetSchemaRequest(Encoding.UTF8.GetString(header.GetValueBytes()));

                if (Client.ConsumerProtoDescriptors.TryGetValue((int)ToUInt64BigEndian(header.GetValueBytes()), out var descriptor))
                {
                    result.Message.Headers.Remove(header.Key);
                }
            }
        }
    }

    private static ulong ToUInt64BigEndian(byte[] data)
    {
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(data);
        }
        return BitConverter.ToUInt64(data, 0);
    }

    private bool IsTargetMethod(MethodInfo? targetMethod)
    {
        return targetMethod != null &&
        targetMethodNames.Contains(targetMethod.Name);
    }

    public static IConsumer<K, V> Init<K, V>(IConsumer<K, V> target)
    {
        var proxy = Create<IConsumer<K, V>, ConsumerInterceptor<K, V>>()
            as ConsumerInterceptor<K, V>
            ?? throw new InvalidOperationException(typeof(IConsumer<K, V>).Name);

        proxy.Target = target;
        return proxy as IConsumer<K, V> ?? throw new InvalidOperationException(typeof(IConsumer<K, V>).Name);
    }


    public static IConsumer<K, V> Init<K, V>(IConsumer<K, V> target, Options options)
    {
        var proxy = Create<IConsumer<K, V>, ConsumerInterceptor<K, V>>()
            as ConsumerInterceptor<K, V>
            ?? throw new InvalidOperationException(typeof(IConsumer<K, V>).Name);

        proxy.Target = target;
        proxy.Client = Client.Setup(options, ClientType.Consumer);
        return proxy as IConsumer<K, V> ?? throw new InvalidOperationException(typeof(IConsumer<K, V>).Name);
    }
}