
using System;
using System.Collections.Generic;
using NBlockchain.P2PPrototocol.NodeJSAPI;

namespace NBlockchain.P2PPrototocol.Network
{

  internal interface INetworkAgentAPI
  {
    List<WebSocketClient> sockets { get; }
    void connectToPeers(Uri[] peer);

  }
}
