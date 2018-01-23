
using System;
using System.Collections.Generic;

namespace NBlockchain.P2PPrototocol.Repository
{
  internal interface IRepositoryNetwork
  {

    event EventHandler<NewBlockEventArgs> Broadcast;
    int Count { get; }
    string getLatestBlock();
    string stringify();
    void handleBlockchainResponse(string data, Action queryAll);

  }
}
