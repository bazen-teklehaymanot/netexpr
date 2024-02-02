namespace ExperimentalWrapper.Interceptors;

public class ProducerInterceptor<T> : DispatchProxy
{
#nullable disable
    public T Target { get; set; }
#nullable restore

    private readonly List<string> targetMethodNames =
    [
        "Produce",
        "ProduceAsync",
        "Flush"
    ];

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        if (!IsTargetMethod(targetMethod))
        {
            return targetMethod?.Invoke(Target, args);
        }
        PreHook(targetMethod!, args);
        var result = targetMethod?.Invoke(Target, args);
        PostHook(targetMethod!, args, result);
        return result;
    }

    private void PreHook(MethodInfo method, object?[]? args)
    {
        Console.WriteLine("============== [PRE-HOOK] =================== ");
        Console.WriteLine($"Method: {method?.Name}");
        Console.WriteLine($"Arguments: {string.Join(", ", args)}");
        Console.WriteLine("=========================================== \n");
    }

    private void PostHook(MethodInfo method, object?[]? args, object? result)
    {
        Console.WriteLine("============== [POST-HOOK] =================== ");
        Console.WriteLine($"Method: {method?.Name}");
        Console.WriteLine($"Arguments: {string.Join(", ", args)}");
        Console.WriteLine($"Result: {result}");
        Console.WriteLine("=========================================== \n");
    }

    private bool IsTargetMethod(MethodInfo? targetMethod)
    {
        return targetMethod != null &&
        targetMethod.DeclaringType == typeof(T) &&
        targetMethodNames.Contains(targetMethod.Name);
    }

    public static IProducer<TKey, TValue> Init<TKey, TValue>(IProducer<TKey, TValue> target)
    {
        var proxy = Create<IProducer<TKey, TValue>, ProducerInterceptor<IProducer<TKey, TValue>>>()
            as ProducerInterceptor<IProducer<TKey, TValue>>
            ?? throw new InvalidOperationException(typeof(IProducer<TKey, TValue>).Name);

        proxy.Target = target;

        return proxy as IProducer<TKey, TValue> ?? throw new InvalidOperationException(typeof(IProducer<TKey, TValue>).Name);
    }
}