using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NBlockchain.P2PPrototocol.NodeJSAPI
{
  internal class WebSocketClient : JavaWebSocket
  {
    internal WebSocketClient(Uri peer)
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
    internal Action onOpen { private get; set; }
    internal void send(string message)
    {
      throw new NotImplementedException();
    }

    #region private
    private Uri peer;
    private ClientWebSocket m_ClientWebSocket = null;
    private Task m_Task;
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
    #endregion

    #region Object
    public override string ToString()
    {
      return peer.ToString();
    }
    #endregion

  }
}
