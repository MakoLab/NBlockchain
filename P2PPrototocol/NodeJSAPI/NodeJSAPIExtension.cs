
using System;
using System.Collections.Generic;

namespace NBlockchain.P2PPrototocol.NodeJSAPI
{
  internal static class NodeJSAPIExtension
  {
    internal static string[] map(this List<WebSocketClient> list, Func<WebSocketClient, string> function)
    {
      List<string> _ret = new List<string>();
      foreach (WebSocketClient _socket in list)
        _ret.Add(function(_socket));
      return _ret.ToArray();
    }
    internal static int Timestamp(this DateTime date) // new Date().getTime() / 1000; - Gets the time value in milliseconds.
    {
      return Convert.ToInt32(date.Ticks / 1000);
    }
  }
}
