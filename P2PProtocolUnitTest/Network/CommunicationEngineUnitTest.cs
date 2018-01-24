
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public async Task ConnectToPeersTestMethod()
    {

      WebSocketConnection _connection = null;
      List<string> _log = new List<string>();
      Task _server = WebSocketServer.Server(3001, x => { _connection = x; x.onMessage = m => _log.Add($"received message by test WS: {m}"); });
      TestRepositoryNetwork _repository = new TestRepositoryNetwork();
      using (CommunicationEngine _new = new CommunicationEngine(_repository, -1, x => _log.Add(x)))
      {
        Assert.AreEqual<int>(2, _log.Count);
        Assert.IsTrue(_repository.IsCosistent);
        Uri[] _peers = new Uri[] { new Uri("ws://localhost:3001") };
        _new.connectToPeers(_peers);
        await Task.Delay(200);
        Assert.IsNotNull(_connection);
        Assert.AreEqual<int>(5, _log.Count);
        Message _requestLast = new Message() { data = string.Empty, type = Message.MessageType.QUERY_LATEST };
        await _connection.SendAsync(_requestLast.Stringify<Message>());
        await Task.Delay(200);
        Assert.AreEqual<int>(8, _log.Count);
        Assert.IsTrue(_log[7].Contains("received message by test"));
      }
      foreach (string _message in _log)
        Debug.WriteLine(_message);
    }
    private class TestRepositoryNetwork : IRepositoryNetwork
    {

      internal bool IsCosistent { get { return Broadcast != null; } }

      #region IRepositoryNetwork
      public int Count => throw new NotImplementedException();
      public event EventHandler<NewBlockEventArgs> Broadcast;
      public string getLatestBlock()
      {
        List<Block> GenesisBlock = new List<Block>() { new Block(0, "0", 1465154705, "my genesis block!!", "816534932c2b7154836da6afc367695e6337db8a921823784c14378abed4f7d7") };
        return GenesisBlock.Stringify<List<Block>>();
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
