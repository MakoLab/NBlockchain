
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBlockchain.P2PPrototocol.AgentAPI;
using NBlockchain.P2PPrototocol.Network;
using NBlockchain.P2PPrototocol.NodeJSAPI;
using NBlockchain.P2PPrototocol.Repository;
using System.Linq;
using System.Collections;

namespace NBlockchain.P2PPrototocol.lUnitTest.AgentAPI
{

  [TestClass]
  public class AgentServicesUnitTest
  {
    [TestMethod]
    public void ServiceResponseAcceptedTestMethod()
    {
      List<string> _log = new List<string>();
      using (AgentServices _services = new AgentServices(new TestRepository(), new NetworkAgent(), -1, (x) => _log.Add(x)))
      {
        Assert.AreEqual<int>(2, _log.Count);
        using (HttpClient _client = new HttpClient())
        {
          _client.BaseAddress = new Uri("http://localhost:3001");
          HttpResponseMessage _message = _client.GetAsync("/peers").Result;
          Assert.IsNotNull(_message);
          Assert.AreEqual<HttpStatusCode>(HttpStatusCode.Accepted, _message.StatusCode);
        }
      }
      Assert.AreEqual<int>(3, _log.Count);
    }
    [TestMethod]
    public void ServiceResponseNotFoundTestMethod()
    {
      List<string> _log = new List<string>();
      using (AgentServices _services = new AgentServices(new TestRepository(), new NetworkAgent(), -1, (x) => _log.Add(x)))
      {
        Assert.AreEqual<int>(2, _log.Count);
        using (HttpClient _client = new HttpClient())
        {
          _client.BaseAddress = new Uri("http://localhost:3001");
          HttpResponseMessage _message = _client.GetAsync("/wrong").Result;
          Assert.IsNotNull(_message);
          Assert.AreEqual<HttpStatusCode>(HttpStatusCode.BadRequest, _message.StatusCode);
        }
      }
      Assert.AreEqual<int>(2, _log.Count);
    }
    private class NetworkAgent : INetworkAgentAPI
    {

      public IEnumerable<string> Sockets { get { return Enumerable.Empty<string>(); } }
      public void ConnectToPeers(Uri[] peer)
      {
        throw new NotImplementedException();
      }
      public void initP2PServer()
      {
        throw new NotImplementedException();
      }

    }
    private class TestRepository : IRepositoryAgentInterface
    {

      public IBlock generateNextBlock(string data)
      {
        throw new NotImplementedException();
      }
      public string stringify()
      {
        throw new NotImplementedException();
      }

    }

  }

}
