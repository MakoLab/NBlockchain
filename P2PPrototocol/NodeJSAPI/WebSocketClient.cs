
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NBlockchain.P2PPrototocol.NodeJSAPI
{
  internal static class WebSocketClient
  {

    internal static async Task Connect(Uri peer, Action<WebSocketConnection> onOpen, Action onError)
    {
      ClientWebSocket m_ClientWebSocket = new ClientWebSocket();
      await m_ClientWebSocket.ConnectAsync(peer, CancellationToken.None);
      switch (m_ClientWebSocket.State)
      {
        case WebSocketState.Open:
          WebSocketConnection _socket = new ClintWebSocketConnection(m_ClientWebSocket);
          onOpen?.Invoke(_socket);
          break;
        default:
          onError?.Invoke();
          break;
      }
    }

    #region private
    private class ClintWebSocketConnection : WebSocketConnection
    {

      public ClintWebSocketConnection(ClientWebSocket clientWebSocket)
      {
        m_ClientWebSocket = clientWebSocket;
        Task.Factory.StartNew(() => ClientMessageLoop());
      }

      #region WebSocketConnection
      protected override Task SendTask(string message)
      {
        return m_ClientWebSocket.SendAsync(message.GetArraySegment(), WebSocketMessageType.Text, true, CancellationToken.None); ;
      }
      #endregion

      #region private
      private ClientWebSocket m_ClientWebSocket;
      private void ClientMessageLoop()
      {
        byte[] buffer = new byte[1024];
        while (true)
        {
          ArraySegment<byte> segment = new ArraySegment<byte>(buffer);
          WebSocketReceiveResult result = m_ClientWebSocket.ReceiveAsync(segment, CancellationToken.None).Result;
          if (result.MessageType == WebSocketMessageType.Close)
          {
            onClose?.Invoke();
            m_ClientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "I am closing", CancellationToken.None).Wait();
            return;
          }
          int count = result.Count;
          while (!result.EndOfMessage)
          {
            if (count >= buffer.Length)
            {
              onClose?.Invoke();
              m_ClientWebSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "That's too long", CancellationToken.None).Wait();
              return;
            }
            segment = new ArraySegment<byte>(buffer, count, buffer.Length - count);
            result = m_ClientWebSocket.ReceiveAsync(segment, CancellationToken.None).Result;
            count += result.Count;
          }
          string _message = Encoding.UTF8.GetString(buffer, 0, count);
          onMessage?.Invoke(_message);
        }
      }
      #endregion

    }
    #endregion

  }
}
