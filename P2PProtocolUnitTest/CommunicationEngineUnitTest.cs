
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBlockchain.P2PPrototocol.Network;
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
    public void connectToPeersMyTestMethod()
    {
      List<string> _log = new List<string>();
      TestRepositoryNetwork _repository = new TestRepositoryNetwork();
      using (CommunicationEngine _new = new CommunicationEngine(_repository, x => _log.Add(x)))
      {
        Assert.AreEqual<int>(1, _log.Count);
        Assert.IsTrue(_repository.IsCosistent);
        Uri[] _peers = new Uri[] { new Uri("http://localhost:3001") };
        _new.connectToPeers(_peers);
      }
    }
  }

  internal class TestRepositoryNetwork : IRepositoryNetwork
  {

    internal bool IsCosistent { get { return Broadcast != null; } }
    #region IRepositoryNetwork
    public int Count => throw new NotImplementedException();
    public event EventHandler<BlockchainStore.NewBlockEventArgs> Broadcast;
    public void Add(Block latestBlockReceived)
    {
      throw new NotImplementedException();
    }
    public Block getLatestBlock()
    {
      throw new NotImplementedException();
    }
    public bool isValidChain(List<Block> newBlocks)
    {
      throw new NotImplementedException();
    }
    public void replaceChain(List<Block> newBlocks)
    {
      throw new NotImplementedException();
    }
    public string stringify()
    {
      throw new NotImplementedException();
    }
    #endregion

  }
}
