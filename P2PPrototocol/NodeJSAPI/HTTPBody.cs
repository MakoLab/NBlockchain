
using System.Net;

namespace NBlockchain.P2PPrototocol.NodeJSAPI
{

  /// <summary>
  /// Body of the HTTP mesage
  /// </summary>
  public class HTTPBody
  {
    /// <summary>
    /// data of the HTTP body part
    /// </summary>
    public string data { get; internal set; }
    /// <summary>
    /// IP address of the peer sending the message
    /// </summary>
    public IPAddress[] peer { get; internal set; }
  }

}
