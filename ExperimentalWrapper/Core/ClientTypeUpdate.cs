namespace ExperimentalWrapper;

public class ClientTypeUpdateRequest
{
  [JsonPropertyName("clientId")]
  public int ClientId { get; set; }
  [JsonPropertyName("type")]
  public string Type { get; set; }
}
