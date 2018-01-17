
using System;
using System.Collections.Generic;
using NBlockchain.P2PPrototocol.NodeJSAPI;

namespace NBlockchain.P2PPrototocol.Network
{
  internal interface INetworkAgentAPI
  {
    List<JavaWebSocket> sockets { get; }
    void connectToPeers(Uri[] peer);
    void initP2PServer();
  }
}
