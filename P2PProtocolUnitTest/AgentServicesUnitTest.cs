using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBlockchain.P2PPrototocol.AgentAPI;
using NBlockchain.P2PPrototocol.Network;
using NBlockchain.P2PPrototocol.NodeJSAPI;
using NBlockchain.P2PPrototocol.Repository;

namespace NBlockchain.P2PPrototocol.lUnitTest
{
  [TestClass]
  public class AgentServicesUnitTest
  {
    [TestMethod]
    public void ConstructorTestMethod()
    {
      List<string> _log = new List<string>();
      using (AgentServices _services = new AgentServices(new TestRepository(), new NetworkAgent(), (x) => _log.Add(x)))
      {

      }
    }
  }

  internal class NetworkAgent : INetworkAgentAPI
  {
    public List<JavaWebSocket> sockets => throw new NotImplementedException();

    public void connectToPeers(IPAddress[] peer)
    {
      throw new NotImplementedException();
    }

    public void initP2PServer()
    {
      throw new NotImplementedException();
    }
  }

  internal class TestRepository : IRepositoryAgentInterface
  {
    public Block generateNextBlock(string data)
    {
      throw new NotImplementedException();
    }

    public string stringify()
    {
      throw new NotImplementedException();
    }
  }
}
