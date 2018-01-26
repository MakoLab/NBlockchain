
using System;

namespace NBlockchain.P2PPrototocol.Repository
{
  internal interface IRepositoryNetwork
  {

    event EventHandler<NewBlockEventArgs> Broadcast;
    string getLatestBlock();
    string stringify();
    void handleBlockchainResponse(string data, Action queryAll);

  }
}
