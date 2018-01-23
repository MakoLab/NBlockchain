
using System;
using System.Threading.Tasks;

namespace NBlockchain.P2PPrototocol.NodeJSAPI
{
  internal abstract class WebSocketConnection
  {

    public virtual Action<string> onMessage { set; protected get; } = x => { };
    public virtual Action onClose { set; protected get; } = () => { };
    public virtual Action onError { set; protected get; } = () => { };
    internal async Task SendAsync(string message)
    {
      await SendTask(message);
    }
    protected abstract Task SendTask(string message);

  }
}