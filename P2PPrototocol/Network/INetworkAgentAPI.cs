
using System;
using System.Collections.Generic;
using NBlockchain.P2PPrototocol.NodeJSAPI;

namespace NBlockchain.P2PPrototocol.Network
{

  internal interface INetworkAgentAPI
  {

    List<WebSocketConnection> sockets { get; }
    void connectToPeers(Uri[] peer);

  }
}
