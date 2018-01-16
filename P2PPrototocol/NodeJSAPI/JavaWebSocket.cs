
using System;
using System.Net;

namespace NBlockchain.P2PPrototocol.NodeJSAPI
{
  public class JavaWebSocket
  {

    public class JavaWebSocketDescription
    {
      public string remoteAddress { get; internal set; }
      public string remotePort { get; internal set; }
    }
    public JavaWebSocket(IPAddress peer)
    {
      this.peer = peer;
    }
    internal Uri url { get; set; }
    internal Action<JavaWebSocket> onConnection { private get; set; }
    internal Action<string> onMessage { private get; set; }
    internal Action onClose { private get; set; }
    internal Action onOpen { private get; set; }
    internal Action onError { private get; set; }
    public JavaWebSocketDescription _socket { get; internal set; }
    /// <summary>
    /// Create a new server instance. 
    /// </summary>
    /// <param name="p2p_port">Port number to start listen</param>
    /// <returns></returns>
    internal static JavaWebSocket Server(int p2p_port)
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