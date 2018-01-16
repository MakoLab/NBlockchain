using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using NBlockchain.P2PPrototocol.NodeJSAPI;

namespace NBlockchain.P2PPrototocol.Network
{
  internal interface INetworkAgentAPI
  {
    List<JavaWebSocket> sockets { get; }

    void connectToPeers(IPAddress[] peer);
    void initP2PServer();
  }
}
