
using System;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBlockchain.P2PPrototocol.NodeJSAPI;

namespace NBlockchain.P2PPrototocol.UnitTest.NodeJSAPI
{
  [TestClass]
  public class WebSocketServerUnitTest
  {

    [TestMethod]
    public async Task ServerTestMethod()
    {
      int _port = 8001;
      Uri m_clientURI = new Uri($"ws://localhost:{_port}/ws/");
      bool _onConnection = false;
      string _reportedUri = String.Empty;
      WebSocketConnection _socket = null;
      Task _server = WebSocketServer.Server(_port, client => { _socket = client; _onConnection = true; });
      await Task.Delay(100);
      ClientWebSocket m_ClientWebSocket = new ClientWebSocket();
      await m_ClientWebSocket.ConnectAsync(m_clientURI, CancellationToken.None);
      await Task.Delay(20);
      Assert.IsNotNull(_socket);
      Assert.IsTrue(_onConnection);
      IPEndPoint _endPoint = _socket.ToString().ParseIPEndPoint();
      Assert.IsNotNull(_endPoint);
      Assert.AreEqual<string>(_socket.ToString(), _endPoint.ToString());
      Assert.AreEqual<WebSocketState>(WebSocketState.Open, m_ClientWebSocket.State);

      //assign callbacks
      bool _onClose = false;
      _socket.onClose = () => _onClose = true;
      bool _onError = false;
      _socket.onError = () => _onError = true;
      string _onMessage = "";
      _socket.onMessage = message => _onMessage = message;
      //Send nessage
      await m_ClientWebSocket.SendAsync("Test".GetArraySegment(), WebSocketMessageType.Text, true, CancellationToken.None);
      Thread.Sleep(100);
      Assert.AreEqual<WebSocketState>(WebSocketState.Open, m_ClientWebSocket.State);
      Assert.IsFalse(_onError);
      Assert.IsFalse(_onClose);
      Assert.AreEqual<string>("Test", _onMessage);
      await m_ClientWebSocket.CloseAsync(WebSocketCloseStatus.Empty, String.Empty, CancellationToken.None);
      Assert.IsFalse(_onError);
      Assert.IsTrue(_onClose);
      Assert.AreEqual<string>("Test", _onMessage);

    }

  }
}
