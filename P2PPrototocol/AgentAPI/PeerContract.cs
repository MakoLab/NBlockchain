using System;
using System.Runtime.Serialization;
using NBlockchain.P2PPrototocol.NodeJSAPI;

namespace NBlockchain.P2PPrototocol.AgentAPI
{
  [DataContract]
  public class PeerContract
  {
    [DataMember]
    public string peer { get; set; }
    internal Uri PeerUri { get; private set; }
    public static PeerContract Parse(string data)
    {
      PeerContract _peer = data.parse<PeerContract>();
      _peer.PeerUri = new Uri(_peer.peer);
      return _peer;
;    }
  }
}
