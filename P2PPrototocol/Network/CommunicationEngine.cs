
using System;
using System.Collections.Generic;
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

    /// <summary>
    /// Craetes instance of CommunicationEngine
    /// </summary>
    internal CommunicationEngine(IRepositoryNetwork repository, Action<string> log)
    {
      Log = log;
      m_Repository = repository;
      m_Repository.Broadcast += CommunicationEngine_Broadcast;
      connectToPeers(initialPeers);
      Log("CommunicationEngine has been started");
    }

    public void initP2PServer()
    {
      JavaWebSocket server = JavaWebSocket.Server(p2p_port);
      server.onConnection = () => initConnection(server);
      Log($"listening websocket p2p port on: {p2p_port}");
    }

    #region INetworkAgentAPI
    /// <summary>
    /// cockets
    /// </summary>
    public List<JavaWebSocket> sockets { get; private set; } = new List<JavaWebSocket>();
    public void connectToPeers(Uri[] newPeers)
    {
      foreach (Uri peer in newPeers)
      {
        JavaWebSocket ws = new JavaWebSocket(peer);
        ws.onOpen = () => initConnection(ws);
        ws.onError = () => Log("connection failed");
      }
    }
    #endregion

    private void initConnection(JavaWebSocket ws)
    {
      sockets.Add(ws);
      initMessageHandler(ws);
      initErrorHandler(ws);
      write(ws, queryChainLengthMsg());
    }
    private void initMessageHandler(JavaWebSocket ws)
    {
      ws.onMessage = (data) =>
      {
        Message message = Message.parse(data);
        Log($"Received message { message.stringify()}");
        switch (message.type)
        {
          case MessageType.QUERY_LATEST:
            write(ws, responseLatestMsg());
            break;
          case MessageType.QUERY_ALL:
            write(ws, responseChainMsg());
            break;
          case MessageType.RESPONSE_BLOCKCHAIN:
            handleBlockchainResponse(message);
            break;
        };
      };
    }
    private void initErrorHandler(JavaWebSocket ws)
    {
      ws.onClose = () => closeConnection(ws);
      ws.onError = () => closeConnection(ws);
    }
    private void closeConnection(JavaWebSocket _ws)
    {
      Log($"connection failed to peer: {_ws.url}");
      sockets.Remove(_ws);
    }
    private void handleBlockchainResponse(Message message)
    {
      List<Block> receivedBlocks = message.Parse(); // JSON.parse(message.data).sort((b1, b2) => (b1.index - b2.index));
      Block latestBlockReceived = receivedBlocks[receivedBlocks.Count - 1];
      Block latestBlockHeld = m_Repository.getLatestBlock();
      if (latestBlockReceived.index > latestBlockHeld.index)
      {
        Log($"blockchain possibly behind. We got: {latestBlockHeld.index} Peer got: {latestBlockReceived.index}");
        if (latestBlockHeld.hash == latestBlockReceived.previousHash)
        {
          Log("We can append the received block to our chain");
          m_Repository.Add(latestBlockReceived);
          broadcast(responseLatestMsg());
        }
        else if (receivedBlocks.Count == 1)
        {
          Log("We have to query the chain from our peer");
          broadcast(queryAllMsg());
        }
        else
        {
          Log("Received blockchain is longer than current blockchain");
          replaceChain(receivedBlocks);
        }
      }
      else
        Log("received blockchain is not longer than received blockchain. Do nothing");
    }
    private void replaceChain(List<Block> newBlocks)
    {
      if (m_Repository.isValidChain(newBlocks) && newBlocks.Count > m_Repository.Count)
      {
        Log("Received blockchain is valid. Replacing current blockchain with received blockchain");
        m_Repository.replaceChain(newBlocks);
        broadcast(responseLatestMsg());
      }
      else
        Log("Received blockchain invalid");
    }
    private Message queryChainLengthMsg()
    {
      return new Message() { type = MessageType.QUERY_LATEST };
    }
    private Message queryAllMsg()
    {
      return new Message() { type = MessageType.QUERY_ALL };
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
    private void write(JavaWebSocket ws, Message message) { ws.send(message.stringify()); }
    private void broadcast(Message message)
    {
      foreach (var socket in sockets)
        write(socket, message);
    }
    private void CommunicationEngine_Broadcast(object sender, BlockchainStore.NewBlockEventArgs e)
    {
      Message _newMessage = new Message()
      {
        type = MessageType.RESPONSE_BLOCKCHAIN,
        data = e.Block.stringify() // JSON.stringify(getLatestBlock())
      };
      broadcast(_newMessage);
    }
    private Action<string> Log { get; }
    private int p2p_port { get; set; } = 6001;
    private Uri[] initialPeers { get; set; } = new Uri[] { };
    private IRepositoryNetwork m_Repository;

    #region IDisposable
    /// <summary>
    /// <see cref="IDisposable"/> implementation
    /// </summary>
    public void Dispose() { }
    #endregion
  }

}