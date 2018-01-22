
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace NBlockchain.P2PPrototocol.NodeJSAPI
{

  internal static class NodeJSAPIExtension
  {

    internal static string[] map(this List<WebSocketConnection> list, Func<WebSocketConnection, string> function)
    {
      List<string> _ret = new List<string>();
      foreach (WebSocketConnection _socket in list)
        _ret.Add(function(_socket));
      return _ret.ToArray();
    }
    internal static long Timestamp(this DateTime date) // new Date().getTime() / 1000; - Gets the time value in milliseconds.
    {
      return date.Ticks / 1000;
    }
    internal static ArraySegment<byte> GetArraySegment(this string message)
    {
      byte[] buffer = Encoding.UTF8.GetBytes(message);
      return new ArraySegment<byte>(buffer);
    }
    internal static IPEndPoint ParseIPEndPoint(this string text)
    {
      Uri uri;
      if (Uri.TryCreate(text, UriKind.Absolute, out uri))
        return new IPEndPoint(IPAddress.Parse(uri.Host), uri.Port < 0 ? 0 : uri.Port);
      if (Uri.TryCreate(String.Concat("tcp://", text), UriKind.Absolute, out uri))
        return new IPEndPoint(IPAddress.Parse(uri.Host), uri.Port < 0 ? 0 : uri.Port);
      if (Uri.TryCreate(String.Concat("tcp://", String.Concat("[", text, "]")), UriKind.Absolute, out uri))
        return new IPEndPoint(IPAddress.Parse(uri.Host), uri.Port < 0 ? 0 : uri.Port);
      throw new FormatException("Failed to parse text to IPEndPoint");
    }

  }
}
