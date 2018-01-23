
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBlockchain.P2PPrototocol.Network;
using NBlockchain.P2PPrototocol.NodeJSAPI;
using NBlockchain.P2PPrototocol.Repository;

namespace NBlockchain.P2PPrototocol.lUnitTest.Network
{

  [TestClass]
  public class CommunicationEngineUnitTest
  {

    [TestMethod]
    public void ConstructorTestMethod()
    {
      List<string> _log = new List<string>();
      TestRepositoryNetwork _repository = new TestRepositoryNetwork();
      using (CommunicationEngine _new = new CommunicationEngine(_repository, -1, x => _log.Add(x)))
      {
        Assert.AreEqual<int>(2, _log.Count);
        Assert.IsTrue(_repository.IsCosistent);
      }
    }
    [TestMethod]
    public void ConnectToPeersTestMethod()
    {

      WebSocketConnection _connection = null;
      Task _server = WebSocketServer.Server(3001, x => _connection = x);
      List<string> _log = new List<string>();
      TestRepositoryNetwork _repository = new TestRepositoryNetwork();
      using (CommunicationEngine _new = new CommunicationEngine(_repository, -1, x => _log.Add(x)))
      {
        Assert.AreEqual<int>(2, _log.Count);
        Assert.IsTrue(_repository.IsCosistent);
        Uri[] _peers = new Uri[] { new Uri("ws://localhost:3001") };
        _new.connectToPeers(_peers);
        Assert.IsNotNull(_connection);
        string _onMesage = null;
        _connection.onMessage = x => _onMesage = x;
        Assert.AreEqual<int>(3, _log.Count);
      }
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
