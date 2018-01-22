
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBlockchain.P2PPrototocol.NodeJSAPI;

namespace NBlockchain.P2PPrototocol.lUnitTest.NodeJSAPI
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
    public async Task ListenTestMethod()
    {

      using (HttpServer _eprs = new HttpServer(3003, (x) => { }))
      {
        bool _called = false;
        Task _serverTask = _eprs.Listen(() => _called = true);
        await Task.Delay(100);
        _eprs.get("/blocks", (req, res) => res.send("Test Message"));
        Assert.IsTrue(_called);
        using (HttpClient _client = new HttpClient())
        {
          _client.BaseAddress = new Uri("http://localhost:3003");
          HttpResponseMessage _message = await _client.GetAsync("/blocks");
          Assert.IsNotNull(_message);
          string _body = await _message.Content.ReadAsStringAsync();
          Assert.AreEqual<string>("Test Message", _body);
        }

      }

    }

  }
}
