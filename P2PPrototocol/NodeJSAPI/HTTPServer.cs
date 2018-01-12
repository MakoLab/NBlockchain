namespace NBlockchain.P2PPrototocol.NodeJSAPI
{

  internal class HTTPServer
  {
    public System.Action<JavaWebSocket> onConnection { get; internal set; }
  }

}