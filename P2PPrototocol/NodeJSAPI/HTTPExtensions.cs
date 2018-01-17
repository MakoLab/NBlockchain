using System;
using System.IO;
using System.Net;
using System.Text;

namespace NBlockchain.P2PPrototocol.NodeJSAPI
{
  internal static class HTTPExtensions
  {

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
