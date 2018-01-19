using System;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NBlockchain.P2PPrototocol.NodeJSAPI
{

  internal class WebSocketServer : JavaWebSocket
  {

    internal static WebSocketServer Server(int p2p_port)
    {
      Uri _uri = new Uri($@"http://localhost:{p2p_port}/");
      WebSocketServer _socket = new WebSocketServer();
      Task.Factory.StartNew(() => _socket.ServerLoop(_uri));
      return _socket;
    }
    internal Action<WebSocketClient>  onConnection { private get; set; }

    private WebSocketServer() { }
    private void ServerLoop(Uri _uri)
    {
      try
      {
        HttpListener _server = new HttpListener();
        _server.Prefixes.Add(_uri.ToString());
        _server.Start();
        while (true)
        {
          HttpListenerContext _hc = _server.GetContextAsync().Result;
          if (!_hc.Request.IsWebSocketRequest)
          {
            _hc.Response.StatusCode = 400;
            _hc.Response.Close();
          }
          HttpListenerWebSocketContext _context = _hc.AcceptWebSocketAsync(null).Result;
          Task.Factory.StartNew(() => ServerMessageLoop(_context));
        }
      }
      catch (Exception _ex)
      {
        onError?.Invoke();
      }
    }
    private void ServerMessageLoop(HttpListenerWebSocketContext context)
    {
      WebSocket ws = context.WebSocket;
      onConnection?.Invoke(new WebSocketClient(context.RequestUri));
      byte[] buffer = new byte[1024];
      while (true)
      {
        ArraySegment<byte> _segments = new ArraySegment<byte>(buffer);
        WebSocketReceiveResult _receiveResult = ws.ReceiveAsync(_segments, CancellationToken.None).Result;
        if (_receiveResult.MessageType == WebSocketMessageType.Close)
        {
          onClose?.Invoke();
          ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "I am closing", CancellationToken.None);
          return;
        }
        int count = _receiveResult.Count;
        while (!_receiveResult.EndOfMessage)
        {
          if (count >= buffer.Length)
          {
            onClose?.Invoke();
            ws.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "That's too long", CancellationToken.None);
            return;
          }
          _segments = new ArraySegment<byte>(buffer, count, buffer.Length - count);
          _receiveResult = ws.ReceiveAsync(_segments, CancellationToken.None).Result;
          count += _receiveResult.Count;
        }
        string _message = Encoding.UTF8.GetString(buffer, 0, count);
        onMessage?.Invoke(_message);
      }
    }


  }

}
