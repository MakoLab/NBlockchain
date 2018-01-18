
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
      _socket.m_Task = _socket.ServerLoop();
      return _socket;
    }
    private async Task ServerLoop()
    {
      HttpListener _server = new HttpListener();
      _server.Prefixes.Add(peer.ToString());
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

    private Task m_Task;

    internal Uri url { get; set; }
    internal Action onConnection { private get; set; } //TODO move to the sever
    internal Action<string> onMessage { private get; set; }
    internal Action onClose { private get; set; }
    internal Action onOpen { private get; set; }
    internal Action onError { private get; set; }
    /// <summary>
    /// Create a new server instance. 
    /// </summary>
    /// <param name="p2p_port">Port number to start listen</param>
    /// <returns></returns>
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