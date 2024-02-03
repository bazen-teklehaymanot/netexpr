using System.Text;
using System.Text.Json;

namespace ExperimentalWrapper.Serializers;

file class ErrorCodes
{
    public const int InvalidInput = 1;

    public static void ThrowIfError(BufferedCommandResult result)
    {
        if (result.ExitCode != 0)
            throw new Exception($"Something went wrong: {result.StandardError}");
    }
}

file class ExecArgs
{
    public required string Payload { get; set; }
    public required string Descriptor { get; set; }
    public required string StructName { get; set; }
    public required string MessageName { get; set; }

    public string[] ToP2J()
    {
        return [
            "p2j",
            "--payload", Payload,
            "--desc", Descriptor,
            "--name", MessageName,
            "--sn", StructName
        ];
    }

    public string[] ToJ2P()
    {
        return [
            "j2p",
            "--payload", Payload,
            "--desc", Descriptor,
            "--name", MessageName,
            "--sn", StructName
        ];
    }
}

public class ProtoSerializer
{
    private static readonly string _bin;

    static ProtoSerializer()
    {
        _bin = Env.Bin;
    }

    private static async Task<string?> Exec(string[] args)
    {
        var result = await Cli.Wrap(_bin)
            .WithArguments(args)
            .WithValidation(CommandResultValidation.None)
            .ExecuteBufferedAsync();

        ErrorCodes.ThrowIfError(result);
        return result.StandardOutput;
    }

    public static async Task<string?> ProtoToJson(
        byte[] bytes,
        string descriptor,
        string structName,
        string messageName
    )
    {
        var p64 = Convert.ToBase64String(bytes);
        var args = new ExecArgs
        {
            Payload = p64,
            Descriptor = descriptor,
            StructName = structName,
            MessageName = messageName
        };
        return await Exec(args.ToP2J());
    }

    public static async Task<byte[]?> ToProto<T>(
        T obj,
        string descriptor,
        string structName,
        string messageName
    ) where T : class
    {
        return await JsonToProto(
            JsonSerializer.Serialize(obj),
            descriptor,
            structName,
            messageName
        );
    }

    public static async Task<byte[]?> JsonToProto(
        string json,
        string descriptor,
        string structName,
        string messageName
    )
    {
        var j64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
        var args = new ExecArgs
        {
            Payload = j64,
            Descriptor = descriptor,
            StructName = structName,
            MessageName = messageName
        };
        var result = await Exec(args.ToJ2P());
        if (string.IsNullOrEmpty(result))
            return null;
        return Convert.FromBase64String(result);
    }

    public static async Task<T?> DeserializeProto<T>(
        byte[] bytes,
        string descriptor,
        string structName,
        string messageName
    ) where T : class
    {
        var res = await ProtoToJson(bytes, descriptor, structName, messageName);
        return string.IsNullOrWhiteSpace(res)
        ? null
        : JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(Convert.FromBase64String(res)));
    }
}