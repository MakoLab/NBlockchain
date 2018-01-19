
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

    internal Uri url { get; set; }
    internal protected Action<string> onMessage { protected get; set; }
    internal protected Action onClose { protected get; set; }
    internal protected Action onError { protected get; set; }

    protected JavaWebSocket() { }
  }

}