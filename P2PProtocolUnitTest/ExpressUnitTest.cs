
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBlockchain.P2PPrototocol.NodeJSAPI;

namespace NBlockchain.P2PPrototocol.lUnitTest
{
  [TestClass]
  public class ExpressUnitTest
  {
    [TestMethod]
    public void IsSupportedTestMethod()
    {
      Assert.IsTrue(Express.IsSupported);
    }
    [TestMethod]
    public void ListenTestMethod()
    {
      using (Express _eprs = new Express(3002))
      {
        bool _called = false;
        _eprs.Listen(() => _called = true);
        Assert.IsTrue(_called);
      }

    }
  }
}
