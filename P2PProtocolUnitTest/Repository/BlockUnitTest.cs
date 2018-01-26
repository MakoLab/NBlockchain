
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBlockchain.P2PPrototocol.Repository;

namespace NBlockchain.P2PPrototocol.UnitTest.Repository
{
  [TestClass]
  public class BlockUnitTest
  {
    [TestMethod]
    public void BlockTestMethod()
    {
      Block _newBlock = getGenesisBlock();
      string _newBlockString = _newBlock.stringify();
      string _expected = "[{\"data\":\"my genesis block!!\",\"hash\":\"816534932c2b7154836da6afc367695e6337db8a921823784c14378abed4f7d7\",\"index\":0,\"previousHash\":\"0\",\"timestamp\":1465154705}]";
      Assert.AreEqual<string>(_expected, _newBlockString);
    }
    private static Block getGenesisBlock()
    {
      return new Block(0, "0", 1465154705, "my genesis block!!", "816534932c2b7154836da6afc367695e6337db8a921823784c14378abed4f7d7");
    }
  }
}
