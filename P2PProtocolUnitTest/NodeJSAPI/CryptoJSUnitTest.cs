using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NBlockchain.P2PPrototocol.lUnitTest.NodeJSAPI
{
  [TestClass]
  public class CryptoJSUnitTest
  {
    [TestMethod]
    public void SHA256FanyLettersTestMethod()
    {
      string _sha = NBlockchain.P2PPrototocol.NodeJSAPI.CryptoJS.SHA256("Zażółć gęślą jaźń");
      Assert.AreEqual<string>("bc5348fd7c2dd8bbf411f0b9268265f7c2e0d31ebf314695882b8170c7e1e9d7".ToUpper(), _sha);
    }
  }
}
