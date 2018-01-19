
using System;

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
    public Uri[] peer { get; internal set; }
  }

}
