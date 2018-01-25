
using System;
using System.Collections.Generic;

namespace NBlockchain.P2PPrototocol.Network
{

  internal interface INetworkAgentAPI
  {

    IEnumerable<string> Sockets { get; }
    void ConnectToPeers(Uri[] peer);

  }
}
