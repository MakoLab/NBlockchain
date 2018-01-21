
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBlockchain.P2PPrototocol.Network;
using NBlockchain.P2PPrototocol.NodeJSAPI;
using NBlockchain.P2PPrototocol.Repository;

namespace NBlockchain.P2PPrototocol.lUnitTest
{

  [TestClass]
  public class CommunicationEngineUnitTest
  {

    [TestMethod]
    public void ConstructorTestMethod()
    {
      List<string> _log = new List<string>();
      TestRepositoryNetwork _repository = new TestRepositoryNetwork();
      using (CommunicationEngine _new = new CommunicationEngine(_repository, x => _log.Add(x)))
      {
        Assert.AreEqual<int>(1, _log.Count);
        Assert.IsTrue(_repository.IsCosistent);
      }
    }
    [TestMethod]
    public void ConnectToPeersTestMethod()
    {
      bool _connected = false;
      Task _server = WebSocketServer.Server(3001, x => _connected = true);
      List<string> _log = new List<string>();
      TestRepositoryNetwork _repository = new TestRepositoryNetwork();
      using (CommunicationEngine _new = new CommunicationEngine(_repository, x => _log.Add(x)))
      {
        Assert.AreEqual<int>(1, _log.Count);
        Assert.IsTrue(_repository.IsCosistent);
        Uri[] _peers = new Uri[] { new Uri("ws://localhost:3001") };
        _new.connectToPeers(_peers);
      }
      Assert.IsTrue(_connected);
      Assert.AreEqual<int>(1, _log.Count);
    }

    private class TestRepositoryNetwork : IRepositoryNetwork
    {

      internal bool IsCosistent { get { return Broadcast != null; } }

      #region IRepositoryNetwork
      public int Count => throw new NotImplementedException();
      public event EventHandler<NewBlockEventArgs> Broadcast;
      public void Add(IBlock latestBlockReceived)
      {
        throw new NotImplementedException();
      }
      public IBlock getLatestBlock()
      {
        throw new NotImplementedException();
      }
      public bool isValidChain(IEnumerable<IBlock> newBlocks)
      {
        throw new NotImplementedException();
      }
      public string stringify()
      {
        throw new NotImplementedException();
      }
      public void handleBlockchainResponse(string data, Action queryAll)
      {
        throw new NotImplementedException();
      }
      #endregion

    }

  }

}
