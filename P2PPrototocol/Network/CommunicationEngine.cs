
using System;
using System.Collections.Generic;
using System.Linq;
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
        this.m_WebSocketP2pPort = webSocketP2pPort;
      else
        Log($"Wrong port number {webSocketP2pPort}; communication will be started using default port number");
      m_Repository.Broadcast += CommunicationEngine_Broadcast;
      ConnectToPeers(initialPeers);
      Log("CommunicationEngine has been started");
    }
    public async Task InitP2PServerAsync()
    {
      Log($"listening websocket p2p port on: {m_WebSocketP2pPort}");
      await WebSocketServer.Server(m_WebSocketP2pPort, async _ws => await InitConnectionAsync(_ws));
    }
    #endregion

    #region INetworkAgentAPI
    public void ConnectToPeers(Uri[] newPeers)
    {
      List<Task> _connectionJobs = new List<Task>();
      foreach (Uri peer in newPeers)
        _connectionJobs.Add(WebSocketClient.Connect(peer, async ws => await InitConnectionAsync(ws), Log));
      Task.WaitAll(_connectionJobs.ToArray());
    }
    public IEnumerable<string> Sockets => m_Sockets.Select<WebSocketConnection, string>(x => x.ToString());
    #endregion

    #region private
    private int m_WebSocketP2pPort = 6001;
    private IRepositoryNetwork m_Repository;
    private List<WebSocketConnection> m_Sockets { get; set; } = new List<WebSocketConnection>();
    private Uri[] initialPeers { get; set; } = new Uri[] { };
    private async Task InitConnectionAsync(WebSocketConnection ws)
    {
      m_Sockets.Add(ws);
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
      Log($"Closing connection to peer: {_ws.ToString()}");
      m_Sockets.Remove(_ws);
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
        data = m_Repository.getLatestBlock()
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
      foreach (var socket in m_Sockets)
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
    #endregion

    #region IDisposable
    /// <summary>
    /// <see cref="IDisposable"/> implementation
    /// </summary>
    public void Dispose()
    {
      Log($"Shuting down the communicatin engine");
      List<Task> _disconnectionTasks = new List<Task>();
      foreach (WebSocketConnection _item in m_Sockets)
        _disconnectionTasks.Add(_item.DisconnectAsync());
      Task.WaitAll(_disconnectionTasks.ToArray());
    }
    #endregion

  }

}