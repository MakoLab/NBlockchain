
using System;
using System.Runtime.Serialization;
using NBlockchain.P2PPrototocol.NodeJSAPI;

namespace NBlockchain.P2PPrototocol.AgentAPI
{
  /// <summary>
  /// class PeerContract
  /// </summary>
  [DataContract]
  public class PeerContract
  {
    /// <summary>
    /// represnt the peer network addre3ss as <see cref="string"/>
    /// </summary>
    [DataMember]
    public string peer { get; set; }
    internal Uri PeerUri { get; private set; }
    /// <summary>
    /// Parses the JSON string to recover an instance of <see cref="PeerContract"/>
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static PeerContract Parse(string data)
    {
      PeerContract _peer = data.Parse<PeerContract>();
      _peer.PeerUri = new Uri(_peer.peer);
      return _peer;
;    }
  }
}
