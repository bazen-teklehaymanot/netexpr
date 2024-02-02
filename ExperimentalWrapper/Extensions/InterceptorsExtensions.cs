namespace ExperimentalWrapper.Extensions;

public static class InterceptorsExtensions
{
    public static IConsumer<TKey, TValue> WithInterceptor<TKey, TValue>(this IConsumer<TKey, TValue> consumer)
    {
        return ConsumerInterceptor<IConsumer<TKey, TValue>>
        .Init(consumer);
    }

    public static IProducer<TKey, TValue> WithInterceptor<TKey, TValue>(this IProducer<TKey, TValue> producer)
    {
        return ProducerInterceptor<IProducer<TKey, TValue>>
        .Init(producer);
    }

    public static IProducer<TKey, TValue> BuildWithInterceptor<TKey, TValue>(this ProducerBuilder<TKey, TValue> builder)
    {
        return builder.Build()
            .WithInterceptor();
    }

    public static IConsumer<TKey, TValue> BuildWithInterceptor<TKey, TValue>(this ConsumerBuilder<TKey, TValue> builder)
    {
        return builder.Build()
            .WithInterceptor();
    }
}