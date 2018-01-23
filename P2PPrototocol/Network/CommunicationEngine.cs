
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using NBlockchain.P2PPrototocol.NodeJSAPI;
using NBlockchain.P2PPrototocol.Repository;
using static NBlockchain.P2PPrototocol.Network.Message;

namespace NBlockchain.P2PPrototocol.Network
{

  /// <summary>
  /// CommunicationEngine
  /// </summary>
  internal class CommunicationEngine : IDisposable, INetworkAgentAPI
  {

    #region API
    /// <summary>
    /// Craetes instance of CommunicationEngine
    /// </summary>
    internal CommunicationEngine(IRepositoryNetwork repository, int webSocketP2pPort, Action<string> log)
    {
      m_Repository = repository;
      Log = log;
      if (IPEndPoint.MaxPort > webSocketP2pPort && IPEndPoint.MinPort < webSocketP2pPort)
        this.WebSocketP2pPort = webSocketP2pPort;
      else
        Log($"Wrong port number {webSocketP2pPort}; communication will be started using default port number");
      m_Repository.Broadcast += CommunicationEngine_Broadcast;
      connectToPeers(initialPeers);
      Log("CommunicationEngine has been started");
    }
    public async Task InitP2PServerAsync()
    {
      Log($"listening websocket p2p port on: {WebSocketP2pPort}");
      await WebSocketServer.Server(WebSocketP2pPort, async _ws => await InitConnectionAsync(_ws));
    }
    #endregion

    #region INetworkAgentAPI
    /// <summary>
    /// cockets
    /// </summary>
    public List<WebSocketConnection> sockets { get; private set; } = new List<WebSocketConnection>();
    public void connectToPeers(Uri[] newPeers)
    {
      List<Task> _connectionJobs = new List<Task>();
      foreach (Uri peer in newPeers)
        _connectionJobs.Add(WebSocketClient.Connect(peer, async ws => await InitConnectionAsync(ws), Log));
      Task.WaitAll(_connectionJobs.ToArray());
    }
    #endregion

    #region private
    private async Task InitConnectionAsync(WebSocketConnection ws)
    {
      sockets.Add(ws);
      initMessageHandler(ws);
      initErrorHandler(ws);
      await WriteAsync(ws, Message.queryChainLengthMsg);
    }
    private void initMessageHandler(WebSocketConnection ws)
    {
      ws.onMessage = async (data) =>
      {
        Message message = data.Parse<Message>();
        Log($"Received message { message.Stringify<Message>()}");
        switch (message.type)
        {
          case MessageType.QUERY_LATEST:
            await WriteAsync(ws, responseLatestMsg());
            break;
          case MessageType.QUERY_ALL:
            await WriteAsync(ws, responseChainMsg());
            break;
          case MessageType.RESPONSE_BLOCKCHAIN:
            m_Repository.handleBlockchainResponse(message.data, () => broadcast(responseChainMsg()));
            break;
        };
      };
    }
    private void initErrorHandler(WebSocketConnection ws)
    {
      ws.onClose = () => closeConnection(ws);
      ws.onError = () => closeConnection(ws);
    }
    private void closeConnection(WebSocketConnection _ws)
    {
      Log($"connection failed to peer: {_ws.ToString()}");
      sockets.Remove(_ws);
    }

    private Message responseChainMsg()
    {
      return new Message()
      {
        type = MessageType.RESPONSE_BLOCKCHAIN,
        data = m_Repository.stringify()
      };
    }
    private Message responseLatestMsg()
    {
      return new Message()
      {
        type = MessageType.RESPONSE_BLOCKCHAIN,
        data = m_Repository.getLatestBlock().stringify() // JSON.stringify(getLatestBlock())
      };
    }
    private async Task WriteAsync(WebSocketConnection ws, Message message)
    {
      string _messageAsString = message.Stringify<Message>();
      Log($"Writing message {_messageAsString}");
      await ws.SendAsync(_messageAsString);
    }
    private void broadcast(Message message)
    {
      List<Task> _jobs = new List<Task>();
      foreach (var socket in sockets)
        _jobs.Add(WriteAsync(socket, message));
      Task.WaitAll(_jobs.ToArray());
    }
    private void CommunicationEngine_Broadcast(object sender, NewBlockEventArgs e)
    {
      Message _newMessage = new Message()
      {
        type = MessageType.RESPONSE_BLOCKCHAIN,
        data = e.Block.stringify() // JSON.stringify(getLatestBlock()
      };
      broadcast(_newMessage);
    }
    private Action<string> Log { get; }
    private int WebSocketP2pPort { get; set; } = 6001;
    private Uri[] initialPeers { get; set; } = new Uri[] { };
    private IRepositoryNetwork m_Repository;
    #endregion

    #region IDisposable
    /// <summary>
    /// <see cref="IDisposable"/> implementation
    /// </summary>
    public void Dispose() { }
    #endregion

  }

}