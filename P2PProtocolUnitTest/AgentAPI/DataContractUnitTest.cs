using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBlockchain.P2PPrototocol.AgentAPI;

namespace NBlockchain.P2PPrototocol.lUnitTest.AgentAPI
{
  [TestClass]
  public class DataContractUnitTest
  {
    [TestMethod]
    public void TestMethod1()
    {
      DataContract _data = DataContract.Parse(m_TestString);
      Assert.IsNotNull(_data);
      Assert.AreEqual<string>("Some data to the first block", _data.data);
    }
    private const string m_TestString = "{\"data\" : \"Some data to the first block\"}";
  }
}
