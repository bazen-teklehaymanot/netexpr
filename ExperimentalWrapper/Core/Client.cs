﻿namespace ExperimentalWrapper;

public enum ClientType
{
  Producer,
  Consumer
}

public class Client
{
  public int ClientId { get; set; }
  public string? AccountName { get; set; }
  public string NatsConnectionId
  {
    get => $"{BrokerConnection.ServerInfo.ServerName}:{BrokerConnection.ConnectedId}";
  }
  public required ClientType ClientType { get; set; }
  public int LearningFactor { get; set; }
  public int LearningFactorCounter { get; set; }
  public bool LearningRequestSent { get; set; }
  public bool GetSchemaRequestSent { get; set; }
  public required IConnection BrokerConnection { get; set; }
  public required IJetStream JetStreamContext { get; set; }
  public int ProducerSchemaId { get; set; }
  public string? ProducerProtoDescriptor { get; set; }
  public Dictionary<int, string> ConsumerProtoDescriptors { get; set; } = [];

  public void Register()
  {
    var request = new RegisterRequest
    {
      NatsConnectionId = NatsConnectionId,
      Language = "C#",
      Version = "1.0.0"
    };
    var message = JsonSerializer.SerializeToUtf8Bytes(request);
    var res = Request(Subjects.ClientRegisterSubject, message, 3);
    var registerResp = JsonSerializer.Deserialize<RegisterResp>(res.Data)
      ?? throw new Exception("Failed to register client");

    ClientId = registerResp.ClientId;
    AccountName = registerResp.AccountName;
    LearningFactor = registerResp.LearningFactor;
    LearningFactorCounter = 0;
    LearningRequestSent = false;
    ConsumerProtoDescriptors = [];
  }

  public void SubscribeUpdates()
  {
    // TODO: Implement this method
    Console.WriteLine($"Subscribing to updates for client {ClientId}");
  }

  public void UpdateClientType()
  {
    var request = new ClientTypeUpdateRequest
    {
      ClientId = ClientId,
      Type = ClientType.ToString().ToLower()
    };
    var message = JsonSerializer.SerializeToUtf8Bytes(request);
    Request(Subjects.ClientTypeUpdateSubject, message, 3);
  }

  internal Msg Request(
      string subject,
      byte[] message,
      int timeoutRetry
  )
  {
    try
    {
      int timeoutMilliSeconds = (int)TimeSpan.FromSeconds(20).TotalMilliseconds;
      return BrokerConnection.Request(subject, message, timeoutMilliSeconds);
    }
    catch (NATSTimeoutException)
    {
      if (timeoutRetry <= 0)
        throw;
      return Request(subject, message, timeoutRetry - 1);
    }
  }

  internal void SentGetSchemaRequest(string schemaId)
  {
    throw new NotImplementedException();
  }

  internal void SendLearningMessage(byte[] messageBytes)
  {
    throw new NotImplementedException();
  }

  internal void SendRegisterSchemaRequest()
  {
    throw new NotImplementedException();
  }

  public static Client Setup(
    Options options,
    ClientType clientType)
  {
    DisableDefaultNatsEventHandlers(options);
    var connection = new ConnectionFactory().CreateConnection(options);
    var jetStreamContext = connection.CreateJetStreamContext();
    var client = new Client
    {
      BrokerConnection = connection,
      JetStreamContext = jetStreamContext,
      ClientType = clientType
    };
    client.Register();
    client.SubscribeUpdates();
    client.UpdateClientType();
    return client;

    static void DisableDefaultNatsEventHandlers(Options options)
    {
      options.ClosedEventHandler += (_, _) => { };
      options.ServerDiscoveredEventHandler += (_, _) => { };
      options.DisconnectedEventHandler += (_, _) => { };
      options.ReconnectedEventHandler += (_, _) => { };
      options.LameDuckModeEventHandler += (_, _) => { };
      options.AsyncErrorEventHandler += (_, _) => { };
      options.HeartbeatAlarmEventHandler += (_, _) => { };
      options.UnhandledStatusEventHandler += (_, _) => { };
      options.FlowControlProcessedEventHandler += (_, _) => { };
      options.PullStatusErrorEventHandler += (_, _) => { };
      options.PullStatusWarningEventHandler += (_, _) => { };
    }
  }
}