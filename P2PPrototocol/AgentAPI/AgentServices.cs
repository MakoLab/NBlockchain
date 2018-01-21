
using System;
using System.Threading.Tasks;
using NBlockchain.P2PPrototocol.Network;
using NBlockchain.P2PPrototocol.NodeJSAPI;
using NBlockchain.P2PPrototocol.Repository;

namespace NBlockchain.P2PPrototocol.AgentAPI
{
  internal class AgentServices: IDisposable
  {

    #region composition
    internal IRepositoryAgentInterface Repository { get; set; }
    internal INetworkAgentAPI Network { get; set; }
    #endregion

    #region API
    public AgentServices(IRepositoryAgentInterface repository, INetworkAgentAPI network, Action<string> log)
    {
      this.Repository = repository;
      this.Network = network;
      this.Log = log;
      initHttpServer();
    }
    internal void initHttpServer()
    {
      m_HttpServer = new HttpServer(http_port, this.Log);
      m_HttpServer.get("/blocks", (req, res) => res.send(Repository.stringify()));
      m_HttpServer.post("/mineBlock", (req, res) =>
        {
          IBlock newBlock = Repository.generateNextBlock(req.body.data);
          //broadcast(responseLatestMsg());
          Log($"block added: {newBlock.stringify()}");
          res.send();
        });
      m_HttpServer.get("/peers", (req, res) =>
        {
          res.send(Network.sockets.map(s => s.ToString()));
        });
      m_HttpServer.post("/addPeer", (req, res) =>
        {
          Network.connectToPeers(req.body.peer);
          res.send();
        });
      Task m_HTTPServerTask = m_HttpServer.Listen(() => Log($"Listening http on port: { http_port}"));
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
    private int http_port { get; set; } = 3001;
    #endregion

  }
}
