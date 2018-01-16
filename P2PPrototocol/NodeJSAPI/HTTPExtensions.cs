using System;
using System.IO;
using System.Net;
using System.Text;

namespace NBlockchain.P2PPrototocol.NodeJSAPI
{
  internal static class HTTPExtensions
  {
    internal static void WriteDocumentContent(this string body, HttpListenerResponse response)
    {
      response.ContentEncoding = System.Text.Encoding.UTF8;
      if (String.IsNullOrEmpty(body))
      {
        response.ContentLength64 = 0;
        return;
      }
      // Get a response stream and write the response to it.
      using (Stream output = response.OutputStream)
      using (StreamWriter _writer = new StreamWriter(output, Encoding.UTF8))
        _writer.Write(body);
    }
    internal static string GetDocumentContent(this HttpListenerRequest Request)
    {
      if (!Request.HasEntityBody)
        return String.Empty;
      using (Stream _content = Request.InputStream)
      using (StreamReader readStream = new StreamReader(_content, Request.ContentEncoding))
        return readStream.ReadToEnd();
    }
  }
}
