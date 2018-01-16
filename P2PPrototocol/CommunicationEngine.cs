using System;
using System.Collections.Generic;
using System.Net;
using NBlockchain.P2PPrototocol.Network;
using NBlockchain.P2PPrototocol.NodeJSAPI;
using NBlockchain.P2PPrototocol.Repository;
using static NBlockchain.P2PPrototocol.Message;

namespace NBlockchain.P2PPrototocol
{

  /// <summary>
  /// CommunicationEngine
  /// </summary>
  public class CommunicationEngine : IDisposable, INetworkAgentAPI
  {
    /// <summary>
    /// Craetes instance of CommunicationEngine
    /// </summary>
    public CommunicationEngine()
    {
      BlockchainStore.Instance().Broadcast += CommunicationEngine_Broadcast;
      connectToPeers(initialPeers);
    }

    private int p2p_port { get; set; } = 6001;
    private IPAddress[] initialPeers { get; set; } = new IPAddress[] { };

    #region INetworkAgentAPI
    /// <summary>
    /// cockets
    /// </summary>
    public List<JavaWebSocket> sockets { get; private set; } = new List<JavaWebSocket>();
    public void connectToPeers(IPAddress[] newPeers)
    {
      foreach (IPAddress peer in newPeers)
      {
        JavaWebSocket ws = new JavaWebSocket(peer);
        ws.onOpen = () => initConnection(ws);
        ws.onError = () => log("connection failed");
      }
    }
    public void initP2PServer()
    {
      JavaWebSocket server = JavaWebSocket.Server(p2p_port);
      server.onConnection = ws => initConnection(ws);
      log($"listening websocket p2p port on: {p2p_port}");
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
        log($"Received message { message.stringify()}");
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
      Action<JavaWebSocket> closeConnection = (_ws) =>
        {
          log($"connection failed to peer: {_ws.url}");
          sockets.splice(sockets.IndexOf(ws), 1);
        };
      ws.onClose = () => closeConnection(ws);
      ws.onError = () => closeConnection(ws);
    }
    private void handleBlockchainResponse(Message message)
    {
      List<Block> receivedBlocks = message.Parse(); // JSON.parse(message.data).sort((b1, b2) => (b1.index - b2.index));
      Block latestBlockReceived = receivedBlocks[receivedBlocks.Count - 1];
      Block latestBlockHeld = BlockchainStore.Instance().getLatestBlock();
      if (latestBlockReceived.index > latestBlockHeld.index)
      {
        log($"blockchain possibly behind. We got: {latestBlockHeld.index} Peer got: {latestBlockReceived.index}");
        if (latestBlockHeld.hash == latestBlockReceived.previousHash)
        {
          log("We can append the received block to our chain");
          BlockchainStore.Instance().Add(latestBlockReceived);
          broadcast(responseLatestMsg());
        }
        else if (receivedBlocks.Count == 1)
        {
          log("We have to query the chain from our peer");
          broadcast(queryAllMsg());
        }
        else
        {
          log("Received blockchain is longer than current blockchain");
          replaceChain(receivedBlocks);
        }
      }
      else
        log("received blockchain is not longer than received blockchain. Do nothing");
    }
    private void replaceChain(List<Block> newBlocks)
    {
      if (BlockchainStore.Instance().isValidChain(newBlocks) && newBlocks.Count > BlockchainStore.Instance().Count)
      {
        log("Received blockchain is valid. Replacing current blockchain with received blockchain");
        BlockchainStore.Instance().replaceChain(newBlocks);
        broadcast(responseLatestMsg());
      }
      else
        log("Received blockchain invalid");
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
        data = BlockchainStore.Instance().stringify()
      };
    }
    private Message responseLatestMsg()
    {
      return new Message()
      {
        type = MessageType.RESPONSE_BLOCKCHAIN,
        data = BlockchainStore.Instance().getLatestBlock().stringify() // JSON.stringify(getLatestBlock())
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

    private void log(string v)
    {
      throw new NotImplementedException();
    }
    /// <summary>
    /// 
    /// </summary>
    public void Dispose()
    {
    }
  }

}