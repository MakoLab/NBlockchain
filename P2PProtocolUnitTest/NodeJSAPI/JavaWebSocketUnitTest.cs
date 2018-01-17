
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBlockchain.P2PPrototocol.NodeJSAPI;

namespace NBlockchain.P2PPrototocol.lUnitTest.NodeJSAPI
{
  [TestClass]
  public class JavaWebSocketUnitTest
  {
    [TestMethod]
    public void ConstructorTestMethod()
    {
      JavaWebSocket _new = new JavaWebSocket(new Uri("http://localhost:3001"));
      string _description = _new.ToString();
      Assert.IsFalse(String.IsNullOrEmpty(_description));
      Assert.AreEqual<string>("http://localhost:3001/", _description);
    }
  }
}
