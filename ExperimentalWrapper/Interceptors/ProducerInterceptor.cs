namespace ExperimentalWrapper.Interceptors;

// public class ProducerInterceptor<T> : DispatchProxy
public class ProducerInterceptor<TKey, TValue> : DispatchProxy
{
#nullable disable
    public IProducer<TKey, TValue> Target { get; set; }
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
        Message<TKey, TValue> message)
    {
        Console.WriteLine("============== [BeforeProduce] =================== ");
        Console.WriteLine($"Topic: {topic}");
        Console.WriteLine($"Message: {message.Value}");
        Console.WriteLine("=========================================== \n");
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
}