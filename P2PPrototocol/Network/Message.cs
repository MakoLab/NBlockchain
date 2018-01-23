using System;
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

    internal static Message queryChainLengthMsg => new Message() { type = MessageType.QUERY_LATEST, data = String.Empty };
    internal static Message queryAllMsg => new Message() { type = MessageType.QUERY_ALL, data = String.Empty };

  }

}