using NATS.Client;

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
        Console.WriteLine("============== [AfterConsume] =================== ");
        Console.WriteLine($"Result: {result}");
        Console.WriteLine("================================================ \n");
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


    public static IConsumer<TKey, TValue> InitV2<TKey, TValue>(IConsumer<TKey, TValue> target, Options options)
    {
        var proxy = Create<IConsumer<TKey, TValue>, ConsumerInterceptor<TKey, TValue>>()
            as ConsumerInterceptor<TKey, TValue>
            ?? throw new InvalidOperationException(typeof(IConsumer<TKey, TValue>).Name);

        proxy.Target = target;
        proxy.Client = Client.Setup(options, ClientType.Consumer);
        return proxy as IConsumer<TKey, TValue> ?? throw new InvalidOperationException(typeof(IConsumer<TKey, TValue>).Name);
    }
}