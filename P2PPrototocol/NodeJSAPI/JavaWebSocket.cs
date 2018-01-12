
using System;
using System.Net;

namespace NBlockchain.P2PPrototocol.NodeJSAPI
{
  internal class JavaWebSocket
  {

    internal class JavaWebSocketDescription
    {
      public string remoteAddress { get; internal set; }
      public string remotePort { get; internal set; }
    }
    public JavaWebSocket(IPAddress peer)
    {
      this.peer = peer;
    }
    internal Uri url { get; set; }
    internal Action<string> onMessage { private get; set; }
    internal Action onClose { private get; set; }
    internal Action onOpen { private get; set; }
    internal Action onError { private get; set; }
    public JavaWebSocketDescription _socket { get; internal set; }
    internal static HTTPServer Server(int p2p_port)
    {
      throw new NotImplementedException();
    }
    internal void send(string v)
    {
      throw new NotImplementedException();
    }
    private IPAddress peer;
  }

}