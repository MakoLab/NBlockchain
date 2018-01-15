
using System;
using System.Collections.Generic;
using System.Net;

namespace NBlockchain.P2PPrototocol.NodeJSAPI
{
  /// <summary>
  /// It is C# wrapper for Java script Express wrapper.
  /// </summary>
  /// <remarks>
  ///  Express is a minimal and flexible Node.js web application framework that provides a robust set of features for web and mobile applications.
  /// </remarks>
  internal class Express : IDisposable
  {

    private Action m_ParserAction;
    private Dictionary<string, Action<Request, Response>> m_GetHandlers = new Dictionary<string, Action<Request, Response>>();
    public Dictionary<string, Action<Request, Response>> m_PostHandlers = new Dictionary<string, Action<Request, Response>>();
    private HttpListener m_HTTPServer;
    private int m_PortNumber = -1;
    private const string m_PrefixTemplate = @"http://localhost:{0}{1}/";

    public Express(int http_port)
    {
      m_PortNumber = http_port;
    }
    internal class Response
    {
      private HttpListenerResponse response;

      public Response(HttpListenerResponse response)
      {
        this.response = response;
      }

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
      private HttpListenerRequest _request;

      public Request(HttpListenerRequest request)
      {
        _request = request;
      }

      public HTTPBody body { get; internal set; }
    }
    internal void use(Action parser)
    {
      m_ParserAction = parser;
    }
    internal void get(string path, Action<Request, Response> handcallbackler)
    {
      m_HTTPServer.Prefixes.Add(string.Format(m_PrefixTemplate, m_PortNumber, path));
      m_GetHandlers.Add(path, handcallbackler);
    }
    internal void post(string path, Action<Request, Response> handcallbackler)
    {
      m_HTTPServer.Prefixes.Add(string.Format(m_PrefixTemplate, m_PortNumber, path));
      m_PostHandlers.Add(path, handcallbackler);
    }
    internal void Listen(Action callback)
    {
      m_HTTPServer = new HttpListener();
      m_HTTPServer.Start();
      System.Threading.Tasks.Task.Run(() => HhttpAsynchronousHandler(m_HTTPServer));
      callback();
    }
    private void HhttpAsynchronousHandler(HttpListener m_HTTPServer)
    {
      while (true)
      {
        if (IsSupported)
          break;
        HttpListenerContext _context = m_HTTPServer.GetContext();
        HttpListenerRequest _request = _context.Request;
        switch (_request.HttpMethod.ToLower())
        {
          case "get":
            {
              Action<Request, Response> _action;
              if (m_GetHandlers.TryGetValue(_request.Url.ToString(), out _action))
                _action(new Request(_request), new Response(_context.Response));
              break;
            }
          case "post":
            break;
          default:
            break;
        }
      }
    }

    /// <summary>
    /// Gets a value that indicates whether <see cref="Express"/>can be used with the current operating system.
    /// </summary>
    internal static bool IsSupported
    {
      get { return HttpListener.IsSupported; }
    }

    #region IDisposable Support
    private bool disposedValue = false; // To detect redundant calls

    protected virtual void Dispose(bool disposing)
    {
      if (!disposedValue)
      {
        if (disposing)
        {
          if (m_HTTPServer != null && m_HTTPServer.IsListening)
            m_HTTPServer.Stop();
          m_HTTPServer = null;
        }
        disposedValue = true;
      }
    }
    // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
    // ~Express() {
    //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
    //   Dispose(false);
    // }
    // This code added to correctly implement the disposable pattern.
    public void Dispose()
    {
      // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
      Dispose(true);
      // TODO: uncomment the following line if the finalizer is overridden above.
      // GC.SuppressFinalize(this);
    }
    #endregion

  }

}