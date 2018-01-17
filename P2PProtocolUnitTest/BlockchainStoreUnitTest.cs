
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBlockchain.P2PPrototocol.Repository;

namespace NBlockchain.P2PPrototocol.lUnitTest
{
  [TestClass]
  public class BlockchainStoreUnitTest
  {
    [TestMethod]
    public void ConstructoTestMethod()
    {
      IRepositoryAgentInterface _newBlockchainStore = new BlockchainStore();
      Assert.IsNotNull(_newBlockchainStore);

    }
  }
}
