using System.Runtime.Serialization;

namespace NBlockchain.P2PPrototocol.Network
{
  [DataContract]
  internal class Message
  {
    internal enum MessageType
    {
      QUERY_LATEST = 0,
      QUERY_ALL = 1,
      RESPONSE_BLOCKCHAIN = 2
    };

    [DataMember]
    internal MessageType type { get; set; }
    [DataMember]
    internal string data { get; set; }

  }

}