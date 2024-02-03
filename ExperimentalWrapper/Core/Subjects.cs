namespace ExperimentalWrapper;

internal class Subjects
{
  public const string ClientReconnectionUpdateSubject = "memphis.clientReconnectionUpdate";
  public const string ClientTypeUpdateSubject = "memphis.clientTypeUpdate";
  public const string ClientRegisterSubject = "memphis.registerClient";
  public const string MemphisLearningSubject = "memphis.schema.learnSchema.{0}";
  public const string MemphisRegisterSchemaSubject = "memphis.tasks.schema.registerSchema.{0}";
  public const string MemphisClientUpdatesSubject = "memphis.updates.{0}";
  public const string MemphisGetSchemaSubject = "memphis.schema.getSchema.{0}";
  public const string MemphisErrorSubject = "memphis.clientErrors";
}
