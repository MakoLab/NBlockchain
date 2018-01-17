
using System;
using NBlockchain.P2PPrototocol.Network;
using NBlockchain.P2PPrototocol.NodeJSAPI;
using NBlockchain.P2PPrototocol.Repository;

namespace NBlockchain.P2PPrototocol.AgentAPI
{
  internal class AgentServices: IDisposable
  {
    internal IRepositoryAgentInterface Repository { get; set; }
    internal INetworkAgentAPI Network { get; set; }
    private Action<string> Log { get; }
    private int http_port { get; set; } = 3001;
    public AgentServices(IRepositoryAgentInterface repository, INetworkAgentAPI network, Action<string> log)
    {
      this.Repository = repository;
      this.Network = network;
      this.Log = log;
      initHttpServer();
    }
    internal void initHttpServer()
    {
      m_HttpServer = new Express(http_port);
      m_HttpServer.get("/blocks", (req, res) => res.send(Repository.stringify()));
      m_HttpServer.post("/mineBlock", (req, res) =>
      {
        Block newBlock = Repository.generateNextBlock(req.body.data);
        //broadcast(responseLatestMsg());
        Log($"block added: {newBlock.stringify()}");
        res.send();
      });
      m_HttpServer.get("/peers", (req, res) =>
      {
        res.send(Network.sockets.map(s => $"{s._socket.remoteAddress}:{s._socket.remotePort}"));
      });
      m_HttpServer.post("/addPeer", (req, res) =>
      {
        Network.connectToPeers(req.body.peer);
        res.send();
      });
      m_HttpServer.Listen(() => Log($"Listening http on port: { http_port}"));
    }
    public void Dispose()
    {
      m_HttpServer?.Dispose();
    }
    private Express m_HttpServer = null;
  }
}
