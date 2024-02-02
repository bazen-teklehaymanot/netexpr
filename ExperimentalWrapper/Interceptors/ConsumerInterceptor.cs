namespace ExperimentalWrapper.Interceptors;

public class ConsumerInterceptor<T> : DispatchProxy
{
#nullable disable
    public T Target { get; set; }
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

    public static IConsumer<TKey, TValue> Init<TKey, TValue>(IConsumer<TKey, TValue> target)
    {
        var proxy = Create<IConsumer<TKey, TValue>, ConsumerInterceptor<IConsumer<TKey, TValue>>>()
            as ConsumerInterceptor<IConsumer<TKey, TValue>>
            ?? throw new InvalidOperationException(typeof(IConsumer<TKey, TValue>).Name);

        proxy.Target = target;

        return proxy as IConsumer<TKey, TValue> ?? throw new InvalidOperationException(typeof(IConsumer<TKey, TValue>).Name);
    }
}