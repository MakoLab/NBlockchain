

using System.Threading.Tasks;
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
      Assert.IsTrue(HttpServer.IsSupported);
    }

    [TestMethod]
    public void ListenTestMethod()
    {
      using (HttpServer _eprs = new HttpServer(3002, (x) => { }))
      {
        bool _called = false;
        Task _serverTask = _eprs.Listen(() => _called = true);
        Assert.IsTrue(_called);
      }
    }

  }
}
