
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
    [TestMethod]
    public void ClientTestMethod()
    {
      Task _server = RunServer();
      Thread.Sleep(200);
      if (_server.IsFaulted)
        throw _server.Exception;
      Assert.IsFalse(_server.IsCompleted);
      Assert.IsFalse(_server.IsFaulted);
      Assert.AreEqual<TaskStatus>(TaskStatus.WaitingForActivation, _server.Status);
      JavaWebSocket _newJavaWebSocket = new JavaWebSocket(m_clientURI);
      bool _onConnection = false;
      bool _onError = false;
      bool _onOpen = false;
      List<string> _messages = new List<string>();
      _newJavaWebSocket.onOpen = () => _onOpen = true;
      _newJavaWebSocket.onConnection = () => _onConnection = true;
      _newJavaWebSocket.onError = () => _onError = true;
      _newJavaWebSocket.onMessage = x => _messages.Add(x);
      Task _ClientLoop = _newJavaWebSocket.Connect();
      Thread.Sleep(200);
      if (_ClientLoop.IsFaulted)
        throw _ClientLoop.Exception;
      Assert.IsTrue(_ClientLoop.IsCompleted);
      Assert.IsFalse(_ClientLoop.IsFaulted);
      Assert.AreEqual<TaskStatus>(TaskStatus.RanToCompletion, _ClientLoop.Status);
      Assert.AreEqual<int>(10, _messages.Count);
      Assert.IsTrue(_onOpen);
      Assert.IsFalse(_onConnection);
      Assert.IsFalse(_onError);
    }
    private static async Task RunServer()
    {
      HttpListener _server = new HttpListener();
      _server.Prefixes.Add(m_serverURI.ToString());
      _server.Start();
      HttpListenerContext _hc = await _server.GetContextAsync();
      if (!_hc.Request.IsWebSocketRequest)
      {
        _hc.Response.StatusCode = 400;
        _hc.Response.Close();
        return;
      }
      Task<HttpListenerWebSocketContext> task = _hc.AcceptWebSocketAsync(null);
      task.Wait();
      WebSocket ws = task.Result.WebSocket;
      for (int i = 0; i != 10; ++i)
      {
        // await Task.Delay(20);
        string time = DateTime.Now.ToLongTimeString();
        byte[] buffer = Encoding.UTF8.GetBytes(time);
        ArraySegment<byte> segment = new ArraySegment<byte>(buffer);
        await ws.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
      }
      await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Done", CancellationToken.None);
    }
    private static Uri m_serverURI = new Uri("http://localhost:8000/ws/");
    private static Uri m_clientURI = new Uri("ws://localhost:8000/ws/");

  }
}
