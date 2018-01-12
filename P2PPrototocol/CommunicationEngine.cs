using System;
using System.Collections.Generic;
using System.Net;
using NBlockchain.P2PPrototocol.NodeJSAPI;
using NBlockchain.P2PPrototocol.Repository;
using static NBlockchain.P2PPrototocol.Message;

namespace NBlockchain.P2PPrototocol
{

  /// <summary>
  /// CommunicationEngine
  /// </summary>
  public class CommunicationEngine
  {
    /// <summary>
    /// Craetes instance of CommunicationEngine
    /// </summary>
    public CommunicationEngine()
    {
    }
    // 'use strict';
    //  FIXME_VAR_TYPE CryptoJS = require("crypto-js");
    //  FIXME_VAR_TYPE express = require("express");
    //  FIXME_VAR_TYPE bodyParser = require('body-parser');
    //  FIXME_VAR_TYPE JavaWebSocket = require("ws");

    private int http_port { get; set; } = 3001;
    private int p2p_port { get; set; } = 6001;
    private IPAddress[] initialPeers { get; set; } = new IPAddress[] { };
    private List<JavaWebSocket> sockets = new List<JavaWebSocket>();
    private void initHttpServer()
    {
      express app = express.GetInstance();
      app.use(() => bodyParser.json());
      app.get("/blocks", (req, res) => res.send(BlockchainStore.Instance().stringify()));
      app.post("/mineBlock", (req, res) =>
          {
            Block newBlock = BlockchainStore.Instance().generateNextBlock(req.body.data);
            broadcast(responseLatestMsg());
            log($"block added: {newBlock.stringify()}");
            res.send();
          });
      app.get("/peers", (req, res) =>
      {
        res.send(sockets.map(s => $"{s._socket.remoteAddress}:{s._socket.remotePort}"));
      });
      app.post("/addPeer", (req, res) =>
      {
        connectToPeers(req.body.peer);
        res.send();
      });
      app.listen(http_port, () => log($"Listening http on port: { http_port}"));
    }
    private void initP2PServer()
    {
      HTTPServer server = JavaWebSocket.Server(p2p_port);
      server.onConnection = ws => initConnection(ws);
      log($"listening websocket p2p port on: { +p2p_port}");
    }
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
    private void connectToPeers(IPAddress[] newPeers)
    {
      foreach (IPAddress peer in newPeers)
      {
        JavaWebSocket ws = new JavaWebSocket(peer);
        ws.onOpen = () => initConnection(ws);
        ws.onError = () => log("connection failed");
      }
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
    /// <summary>
    /// Run the communication machine
    /// </summary>
    public void Run()
    {
      connectToPeers(initialPeers);
      initHttpServer();
      initP2PServer();
    }
    private void log(string v)
    {
      throw new NotImplementedException();
    }

  }

}