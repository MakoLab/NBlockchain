
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBlockchain.P2PPrototocol.NodeJSAPI;

namespace NBlockchain.P2PPrototocol.lUnitTest.NodeJSAPI
{
  [TestClass]
  public class WebSocketClientUnitTest
  {

    [TestMethod]
    public async Task ClientConstructorTestMethod()
    {
      Uri _serverURI = new Uri("http://localhost:8004/ws/");
      Task _server = RunWebSocketServerWriter(_serverURI);
      WebSocketConnection _connection = null;
      bool _onClose = false;
      List<string> _Log = new List<string>();
      await Task.Delay(100);
      await WebSocketClient.Connect(new Uri("ws://localhost:8004/ws/"), x => _connection = x, message => _Log.Add(message));
      Assert.AreEqual<int>(1, _Log.Count);
      Assert.IsNotNull(_connection);
      bool _onError = false;
      _connection.onError = () => _onError = true;
      _connection.onClose = () => _onClose = true;
      await Task.Delay(200);
      Assert.IsTrue(_onClose);
      await Task.Delay(100);
      Assert.IsTrue(_server.IsCompleted);
      Assert.AreEqual<TaskStatus>( TaskStatus.RanToCompletion, _server.Status);
      Assert.IsFalse(_onError);
    }
    [TestMethod]
    public async Task ClientReadingTestMethod()
    {
      Uri _serverURI = new Uri("http://localhost:8003/ws/");
      Uri _clientURI = new Uri("ws://localhost:8003/ws/");
      Task _server = RunWebSocketServerWriter(_serverURI);
      bool _onClose = false;
      List<string> _messages = new List<string>();
      WebSocketConnection _connection = null;
      await Task.Delay(100);
      await WebSocketClient.Connect(_clientURI, x => _connection = x, message => _messages.Add(message));
      Assert.IsNotNull(_connection);
      _connection.onMessage = x => _messages.Add(x);
      bool _onError = false;
      _connection.onError = () => _onError = true;
      _connection.onClose = () => _onClose = true;
      await Task.Delay(200);
      Assert.AreEqual<int>(11, _messages.Count);
      Assert.IsTrue(_onClose);
      await Task.Delay(100);
      Assert.IsTrue(_server.IsCompleted);
      Assert.AreEqual<TaskStatus>(TaskStatus.RanToCompletion, _server.Status);
      Assert.IsFalse(_onError);
      Assert.AreEqual<int>(11, _messages.Count);
    }

    #region instrumentation
    private async Task RunPassiveWebSocketServer(Uri m_serverURI)
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
      HttpListenerWebSocketContext _context = await _hc.AcceptWebSocketAsync(null);
      WebSocket _ws = _context.WebSocket;
      await Task.Delay(100);
      await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Done", CancellationToken.None);
    }
    private async Task RunWebSocketServerWriter(Uri m_serverURI)
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
      HttpListenerWebSocketContext _context= await _hc.AcceptWebSocketAsync(null);
      WebSocket ws = _context.WebSocket;
      await Task.Delay(100);
      for (int i = 0; i != 10; ++i)
      {
        string time = $"Loop counter {i}";
        await ws.SendAsync(time.GetArraySegment(), WebSocketMessageType.Text, true, CancellationToken.None);
      }
      await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Done", CancellationToken.None);
    }
    #endregion

  }
}
