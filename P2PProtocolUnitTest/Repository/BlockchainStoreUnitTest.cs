﻿
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBlockchain.P2PPrototocol.Repository;

namespace NBlockchain.P2PPrototocol.UnitTest.Repository
{
  [TestClass]
  public class BlockchainStoreUnitTest
  {
    [TestMethod]
    public void ConstructoTestMethod()
    {
      IRepositoryAgentInterface _newBlockchainStore = new BlockchainStore(message => { });
      Assert.IsNotNull(_newBlockchainStore);
    }
    [TestMethod]
    public void stringifyTestMethod()
    {
      IRepositoryAgentInterface _newBlockchainStore = new BlockchainStore(message => { });
      string _newBlockString = _newBlockchainStore.stringify();
      string _expected = "[{\"data\":\"my genesis block!!\",\"hash\":\"816534932c2b7154836da6afc367695e6337db8a921823784c14378abed4f7d7\",\"index\":0,\"previousHash\":\"0\",\"timestamp\":1465154705}]";
      Assert.AreEqual<string>(_expected, _newBlockString);
    }
    [TestMethod]
    public void generateNextBlockTestMethod()
    {
      List<string> _log = new List<string>();
      IRepositoryAgentInterface _newBlockchainStore = new BlockchainStore(message => _log.Add(message));
      const string _testString = "generate Next Block";
      IBlock _newBlock = _newBlockchainStore.generateNextBlock(_testString);
      string _newBlockString = _newBlock.stringify();
      string _chain = _newBlockchainStore.stringify();
      Assert.IsTrue(_chain.Contains(_testString));
    }

  }
}
