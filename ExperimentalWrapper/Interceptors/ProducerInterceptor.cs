namespace ExperimentalWrapper.Interceptors;

public class ProducerInterceptor<TKey, TValue> : DispatchProxy
{
#nullable disable
    public IProducer<TKey, TValue> Target { get; set; }
    public Client Client { get; set; }
#nullable restore

    private readonly List<string> targetMethodNames =
    [
        "Produce",
        "ProduceAsync"
    ];

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        if (!IsTargetMethod(targetMethod))
        {
            return targetMethod?.Invoke(Target, args);
        }
        BeforeProduce(args[0], args[1] as Message<TKey, TValue>);
        var result = targetMethod?.Invoke(Target, args);
        AfterProduce(args[0], args[1] as Message<TKey, TValue>);
        return result;
    }

    private void BeforeProduce(
        object topic,
        Message<TKey, TValue> message
    )
    {
        if (string.IsNullOrWhiteSpace(Client.ProducerProtoDescriptor))
        {
            if (Client.LearningFactorCounter <= Client.LearningFactor)
            {
                var messageBytes = JsonSerializer.SerializeToUtf8Bytes(message.Value);
                Client.SendLearningMessage(messageBytes);
                Client.LearningFactorCounter++;
            }

            else if (!Client.LearningRequestSent &&
                Client.LearningFactorCounter >= Client.LearningFactor &&
                string.IsNullOrWhiteSpace(Client.ProducerProtoDescriptor)
            )
            {
                Client.SendRegisterSchemaRequest();
            }
            return;
        }

        if (message.Value is ValueType)
            return;
        var protoMessage = ProtoSerializer.ToProto(message.Value as object, Client.ProducerProtoDescriptor, "", "");
        byte[] buf = GetBytesBigEndian(Client.ProducerSchemaId);
        message.Headers.Add("memphis_schema", buf);
    }

    private static byte[] GetBytesBigEndian(long value)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }
        return bytes;
    }

    private void AfterProduce(
        object topic,
        Message<TKey, TValue> message)
    {
        Console.WriteLine("============== [AfterProduce] =================== ");
        Console.WriteLine($"Topic: {topic}");
        Console.WriteLine($"Message: {message.Value}");
        Console.WriteLine("=========================================== \n");
    }

    private bool IsTargetMethod(MethodInfo? targetMethod)
    {
        return targetMethod != null &&
        targetMethodNames.Contains(targetMethod.Name);
    }

    public static IProducer<K, V> Init<K, V>(IProducer<K, V> target)
    {
        var proxy = Create<IProducer<K, V>, ProducerInterceptor<K, V>>()
            as ProducerInterceptor<K, V>
            ?? throw new InvalidOperationException(typeof(IProducer<K, V>).Name);

        proxy.Target = target;

        return proxy as IProducer<K, V> ?? throw new InvalidOperationException(typeof(IProducer<K, V>).Name);
    }

    public static IProducer<K, V> Init<K, V>(IProducer<K, V> target, Options options)
    {
        var proxy = Create<IProducer<K, V>, ProducerInterceptor<K, V>>()
            as ProducerInterceptor<K, V>
            ?? throw new InvalidOperationException(typeof(IProducer<K, V>).Name);

        proxy.Target = target;
        proxy.Client = Client.Setup(options, ClientType.Producer);
        return proxy as IProducer<K, V> ?? throw new InvalidOperationException(typeof(IProducer<K, V>).Name);
    }
}