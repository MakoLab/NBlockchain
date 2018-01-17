
using System;
using System.Collections.Generic;

namespace NBlockchain.P2PPrototocol.Repository
{
  internal interface IRepositoryNetwork
  {

    int Count { get; }
    event EventHandler<BlockchainStore.NewBlockEventArgs> Broadcast;
    Block getLatestBlock();
    void Add(Block latestBlockReceived);
    bool isValidChain(List<Block> newBlocks);
    void replaceChain(List<Block> newBlocks);
    string stringify();

  }
}
