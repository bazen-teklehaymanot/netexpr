using ExperimentalWrapper.Serializers;

namespace ExperimentalWrapper.Examples;

public class SerializationExample
{
    private static readonly string _descriptor = "CmwKEnRlc3RzY2hlbWFfMS5wcm90byJOCgRUZXN0EhYKBmZpZWxkMRgBIAEoCVIGZmllbGQxEhYKBmZpZWxkMhgCIAEoCVIGZmllbGQyEhYKBmZpZWxkMxgDIAEoBVIGZmllbGQzYgZwcm90bzM=";
    private static readonly string _structName = "testschema_1";
    private static readonly string _messageName = "Test";

    public static async Task RunAsync()
    {
        var data = new Dictionary<string, object>
        {
            ["field1"] = "AwesomeFirst",
            ["field2"] = "SecondField",
            ["field3"] = 333,
        };

        await Serializer.ToProto(data, _descriptor, _structName, _messageName);
    }
}