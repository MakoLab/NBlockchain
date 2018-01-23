
using System;
using System.Net;
using System.Threading.Tasks;
using NBlockchain.P2PPrototocol.Network;
using NBlockchain.P2PPrototocol.NodeJSAPI;
using NBlockchain.P2PPrototocol.Repository;

namespace NBlockchain.P2PPrototocol.AgentAPI
{
  internal class AgentServices : IDisposable
  {

    #region composition
    internal IRepositoryAgentInterface Repository { get; set; }
    internal INetworkAgentAPI Network { get; set; }
    #endregion

    #region API
    public AgentServices(IRepositoryAgentInterface repository, INetworkAgentAPI network, int _AgentHTTPServerPortNumber, Action<string> log)
    {
      this.Log = log ?? throw new ArgumentNullException(nameof(log));
      if (IPEndPoint.MaxPort > _AgentHTTPServerPortNumber && IPEndPoint.MinPort < _AgentHTTPServerPortNumber)
        this.AgentHTTPServerPortNumber = _AgentHTTPServerPortNumber;
      else
        Log($"Wrong port number {_AgentHTTPServerPortNumber}; communication will be started using default port number");
      this.Repository = repository;
      this.Network = network;
      initHttpServer();
    }
    internal void initHttpServer()
    {
      m_HttpServer = new HttpServer(AgentHTTPServerPortNumber, this.Log);
      m_HttpServer.get("/blocks", (req, res) => res.send(Repository.stringify()));
      m_HttpServer.post("/mineBlock", (req, res) =>
        {
          string _newData = DataContract.Parse(req.body).data;
          Log($"adding new block using the data: {_newData}");
          IBlock _newBlock = Repository.generateNextBlock(_newData);
          //broadcast(responseLatestMsg());
          Log($"block added: {_newBlock.stringify()}");
          res.send();
        });
      m_HttpServer.get("/peers", (req, res) =>
        {
          res.send(Network.sockets.map(s => s.ToString()));
        });
      m_HttpServer.post("/addPeer", (req, res) =>
        {
          try
          {
            Uri _peer = PeerContract.Parse(req.body).PeerUri;
            Log($"Adding peer {_peer}");
            Network.connectToPeers(new Uri[] { _peer });
          }
          catch (Exception _ex )
          {
            Log($"Cannot add peer because of exception {_ex}");
          }
          res.send();
        });
      Task m_HTTPServerTask = m_HttpServer.Listen(() => Log($"Listening http on port: { AgentHTTPServerPortNumber}"));
    }
    #endregion

    #region IDisposable
    public void Dispose()
    {
      m_HttpServer?.Dispose();
    }
    #endregion

    #region private
    private HttpServer m_HttpServer = null;
    private Action<string> Log { get; }
    private int AgentHTTPServerPortNumber { get; set; } = 3001;
    #endregion

  }
}
