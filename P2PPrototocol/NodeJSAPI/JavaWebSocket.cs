
using System;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NBlockchain.P2PPrototocol.NodeJSAPI
{

  internal class JavaWebSocket
  {
    public JavaWebSocket(Uri peer)
    {
      this.peer = peer;
    }

    internal async Task Connect()
    {
      m_ClientWebSocket = new ClientWebSocket();
      await m_ClientWebSocket.ConnectAsync(this.peer, CancellationToken.None);
      switch (m_ClientWebSocket.State)
      {
        case WebSocketState.Aborted:
        case WebSocketState.Closed:
        case WebSocketState.CloseReceived:
          onClose?.Invoke();
          break;
        case WebSocketState.CloseSent:
        case WebSocketState.Connecting:
        case WebSocketState.None:
          onError?.Invoke();
          break;
        case WebSocketState.Open:
          onOpen?.Invoke();
          break;
        default:
          break;
      }
      m_Task = ClientMessageLoop();
    }

    private async Task ClientMessageLoop()
    {
      byte[] buffer = new byte[1024];
      while (true)
      {
        ArraySegment<byte> segment = new ArraySegment<byte>(buffer);
        WebSocketReceiveResult result = await m_ClientWebSocket.ReceiveAsync(segment, CancellationToken.None);
        if (result.MessageType == WebSocketMessageType.Close)
        {
          onClose?.Invoke();
          await m_ClientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "I am closing", CancellationToken.None);
          return;
        }
        int count = result.Count;
        while (!result.EndOfMessage)
        {
          if (count >= buffer.Length)
          {
            onClose?.Invoke();
            await m_ClientWebSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "That's too long", CancellationToken.None);
            return;
          }
          segment = new ArraySegment<byte>(buffer, count, buffer.Length - count);
          result = await m_ClientWebSocket.ReceiveAsync(segment, CancellationToken.None);
          count += result.Count;
        }
        string _message = Encoding.UTF8.GetString(buffer, 0, count);
        onMessage?.Invoke(_message);
      }
    }
    internal static JavaWebSocket Server(int p2p_port)
    {
      Uri _uri = new Uri($@"http://localhost:{p2p_port}/");
      JavaWebSocket _socket = new JavaWebSocket(_uri);
      Task.Factory.StartNew(() => _socket.ServerLoop());
      return _socket;
    }
    private void ServerLoop()
    {
      try
      {
        HttpListener _server = new HttpListener();
        _server.Prefixes.Add(peer.ToString());
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
      onConnection?.Invoke();
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
    private Task m_Task;

    internal Uri url { get; set; }
    internal Action onConnection { private get; set; } //TODO move to the sever
    internal Action<string> onMessage { private get; set; }
    internal Action onClose { private get; set; }
    internal Action onOpen { private get; set; }
    internal Action onError { private get; set; }
    internal void send(string message)
    {
      throw new NotImplementedException();
    }

    #region Object
    public override string ToString()
    {
      return peer.ToString();
    }
    #endregion

    #region private
    private ClientWebSocket m_ClientWebSocket = null;
    private Uri peer;
    #endregion

  }

}