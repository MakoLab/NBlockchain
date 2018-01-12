
using System;

namespace NBlockchain.P2PPrototocol.NodeJSAPI
{
  /// <summary>
  /// It is C# wrapper for Java script Express wrapper.
  /// </summary>
  /// <remarks>
  ///  Express is a minimal and flexible Node.js web application framework that provides a robust set of features for web and mobile applications.
  /// </remarks>
  internal class express
  {
    internal class Response
    {
      internal void send(string[] content)
      {
        throw new NotImplementedException();
      }
      internal void send(string content)
      {
        throw new NotImplementedException();
      }
      internal void send()
      {
        throw new NotImplementedException();
      }
    }
    internal class Request
    {
      public HTTPBody body { get; internal set; }
    }
    internal static express GetInstance()
    {
      return m_Instance;
    }
    internal void use(Action p)
    {
      throw new NotImplementedException();
    }
    internal void get(string path, Action<Request, Response> handcallbackler)
    {
      throw new NotImplementedException();
    }
    internal void post(string path, Action<Request, Response> handcallbackler)
    {
      throw new NotImplementedException();
    }
    internal void listen(int http_port, Action callback)
    {
      throw new NotImplementedException();
      //callback();
    }
    private static express m_Instance { get; } = new express();

  }

}