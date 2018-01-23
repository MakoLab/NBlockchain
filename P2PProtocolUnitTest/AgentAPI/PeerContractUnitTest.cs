using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBlockchain.P2PPrototocol.AgentAPI;

namespace NBlockchain.P2PPrototocol.lUnitTest.AgentAPI
{
  [TestClass]
  public class PeerContractUnitTest
  {

    [TestMethod]
    public void ParseTestMethod()
    {
      PeerContract _peer = PeerContract.Parse(m_TestString);
      Assert.IsNotNull(_peer);
      Assert.IsNotNull(_peer.PeerUri);
      Assert.AreEqual<string>(_peer.peer, _peer.PeerUri.ToString());
      Assert.AreEqual<string>("ws://localhost:6001/", _peer.PeerUri.ToString());
    }
    private const string m_TestString = "{\"peer\" : \"ws://localhost:6001/\"}";

  }
}
