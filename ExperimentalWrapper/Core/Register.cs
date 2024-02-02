using System.Text.Json.Serialization;

namespace ExperimentalWrapper;

#nullable disable

internal class RegisterRequest
{
  [JsonPropertyName("natsConnectionId")]
  public string NatsConnectionId { get; set; }
  [JsonPropertyName("language")]
  public string Language { get; set; }
  [JsonPropertyName("version")]
  public string Version { get; set; }
}

internal class RegisterResp
{
  [JsonPropertyName("clientId")]
  public int ClientId { get; set; }
  [JsonPropertyName("accountName")]
  public string AccountName { get; set; }
  [JsonPropertyName("learningFactor")]
  public int LearningFactor { get; set; }
}