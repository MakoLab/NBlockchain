
using System;
using System.Collections.Generic;

namespace NBlockchain.P2PPrototocol.Repository
{
  internal interface IRepositoryNetwork
  {

    event EventHandler<NewBlockEventArgs> Broadcast;
    int Count { get; }
    IBlock getLatestBlock();
    void Add(IBlock latestBlockReceived);
    bool isValidChain(IEnumerable<IBlock> newBlocks);
    string stringify();
    void handleBlockchainResponse(string data, Action queryAll);

  }
}
