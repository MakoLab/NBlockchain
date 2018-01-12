
using System;
using System.Collections.Generic;
using NBlockchain.P2PPrototocol.Repository;

namespace NBlockchain.P2PPrototocol
{

  internal class Message
  {
    internal enum MessageType
    {
      QUERY_LATEST = 0,
      QUERY_ALL = 1,
      RESPONSE_BLOCKCHAIN = 2
    };

    internal MessageType type { get; set; }
    internal string data { get; set; }

    internal List<Block> Parse()
    {
      //JSON.parse(message.data).sort((b1, b2) => (b1.index - b2.index));
      throw new NotImplementedException();
    }
    internal string stringify()
    {
      throw new NotImplementedException();
    }
    internal static Message parse(string data)
    {
      throw new NotImplementedException();
    }
  }

}